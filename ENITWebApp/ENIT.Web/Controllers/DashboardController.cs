using ENIT.BL.Entities;
using ENIT.BL.Enums;
using ENIT.DAL.Context;
using ENIT.Web.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ENIT.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ENITDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(ENITDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.Users
            .Include(x => x.Department)
            .FirstAsync(x => x.Id == _userManager.GetUserId(User)!);

        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User";
        var activitiesQuery = _context.Activities
            .Include(x => x.Department)
            .Include(x => x.Participations)
            .AsQueryable();

        if (role == "DepartmentAdmin" && user.DepartmentId.HasValue)
        {
            activitiesQuery = activitiesQuery.Where(x => x.DepartmentId == user.DepartmentId.Value);
        }

        var activities = await activitiesQuery
            .OrderBy(x => x.StartDate)
            .ToListAsync();

        var myParticipationsCount = await _context.Participations.CountAsync(x => x.UserId == user.Id);
        var notifications = await _context.Notifications
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.SentAt)
            .Take(4)
            .ToListAsync();

        var departments = await _context.Departments
            .Include(x => x.Users)
            .Include(x => x.Activities)
            .OrderBy(x => x.Name)
            .ToListAsync();

        var model = new DashboardViewModel
        {
            UserName = user.FullName,
            RoleName = role,
            DepartmentName = user.Department?.Name,
            Matricule = user.Matricule,
            AcademicSession = $"{DateTime.Today.Year}-{DateTime.Today.Year + 1}",
            Stats =
            [
                new StatCardViewModel { Label = "Activites publiees", Value = activities.Count.ToString(), Accent = "primary", Icon = "calendar-event" },
                new StatCardViewModel { Label = "Mes inscriptions", Value = myParticipationsCount.ToString(), Accent = "success", Icon = "user-check" },
                new StatCardViewModel { Label = "Departements actifs", Value = departments.Count.ToString(), Accent = "warning", Icon = "building" },
                new StatCardViewModel { Label = "Activites a venir", Value = activities.Count(x => x.StartDate >= DateTime.Today).ToString(), Accent = "info", Icon = "sparkles" }
            ],
            FeaturedActivities = activities
                .Take(3)
                .Select(x => new ActivitySnapshotViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    DepartmentName = x.Department?.Name ?? string.Empty,
                    StartDate = x.StartDate,
                    Type = x.Type.ToString(),
                    Status = x.Status.ToString(),
                    CoverImagePath = x.CoverImagePath,
                    RegisteredCount = x.Participations.Count(p => p.Status == ParticipationStatus.Confirmed)
                })
                .ToList(),
            UpcomingActivities = activities
                .Where(x => x.StartDate >= DateTime.Today)
                .Take(6)
                .Select(x => new ActivitySnapshotViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    DepartmentName = x.Department?.Name ?? string.Empty,
                    StartDate = x.StartDate,
                    Type = x.Type.ToString(),
                    Status = x.Status.ToString(),
                    CoverImagePath = x.CoverImagePath,
                    RegisteredCount = x.Participations.Count(p => p.Status == ParticipationStatus.Confirmed)
                })
                .ToList(),
            Notifications = notifications
                .Select(x => new NotificationViewModel
                {
                    Title = x.Title,
                    Message = x.Message,
                    Type = x.Type.ToString(),
                    SentAt = x.SentAt
                })
                .ToList(),
            Departments = departments
                .Select(x => new DepartmentSummaryViewModel
                {
                    Name = x.Name,
                    Code = x.Code,
                    ActivityCount = x.Activities.Count,
                    UserCount = x.Users.Count
                })
                .ToList(),
            CalendarEvents = activities
                .Where(x => x.StartDate >= DateTime.Today)
                .Take(8)
                .Select(x => new CalendarEventViewModel
                {
                    Title = x.Title,
                    DepartmentName = x.Department?.Code ?? string.Empty,
                    StartDate = x.StartDate
                })
                .ToList()
        };

        return View(model);
    }
}
