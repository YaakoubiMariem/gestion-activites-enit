using ENIT.BL.Entities;
using ENIT.BL.Enums;
using ENIT.DAL.Context;
using ENIT.Web.ViewModels.Participations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ENIT.Web.Controllers;

[Authorize]
public class ParticipationsController : Controller
{
    private readonly ENITDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ParticipationsController(ENITDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> MyRegistrations()
    {
        var userId = _userManager.GetUserId(User)!;
        var participations = await _context.Participations
            .Include(x => x.Activity)
            .ThenInclude(x => x!.Department)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.RegistrationDate)
            .ToListAsync();

        var model = participations.Select(x => new MyRegistrationItemViewModel
        {
            ActivityId = x.ActivityId,
            ActivityTitle = x.Activity!.Title,
            Department = x.Activity.Department!.Code,
            RegistrationDate = x.RegistrationDate,
            Status = x.Status == ParticipationStatus.Confirmed ? "Confirmée" : "Liste d'attente",
            StartDate = x.Activity.StartDate
        }).ToList();

        return View(model);
    }
}
