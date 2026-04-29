using ENIT.DAL.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ENIT.Web.Controllers;

[Authorize]
public class CalendarController : Controller
{
    private readonly ENITDbContext _context;

    public CalendarController(ENITDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? departmentId)
    {
        ViewBag.Departments = await _context.Departments
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, Label = $"{x.Code} - {x.Name}" })
            .ToListAsync();

        ViewBag.DepartmentId = departmentId;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Events(int? departmentId)
    {
        var query = _context.Activities
            .Include(x => x.Department)
            .AsQueryable();

        if (departmentId.HasValue)
        {
            query = query.Where(x => x.DepartmentId == departmentId.Value);
        }

        var events = await query
            .OrderBy(x => x.StartDate)
            .Select(x => new
            {
                id = x.Id,
                title = x.Title,
                start = x.StartDate,
                end = x.EndDate,
                url = Url.Action("Details", "Activities", new { id = x.Id }),
                backgroundColor = x.Department!.Code == "INFO" ? "#003087" :
                                  x.Department.Code == "GC" ? "#20b486" :
                                  x.Department.Code == "GH" ? "#4aa5ff" :
                                  "#3f62cf",
                borderColor = "transparent",
                extendedProps = new
                {
                    department = x.Department!.Name,
                    location = x.Location,
                    type = x.Type.ToString()
                }
            })
            .ToListAsync();

        return Json(events);
    }
}
