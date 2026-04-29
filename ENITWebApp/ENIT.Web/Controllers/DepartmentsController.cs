using ENIT.BL.Entities;
using ENIT.DAL.Contracts;
using ENIT.Web.Infrastructure;
using ENIT.Web.Services;
using ENIT.Web.ViewModels.Departments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ENIT.Web.Controllers;

[Authorize]
public class DepartmentsController : Controller
{
    private readonly IDepartmentManager _departmentManager;
    private readonly FileStorageService _fileStorageService;

    public DepartmentsController(IDepartmentManager departmentManager, FileStorageService fileStorageService)
    {
        _departmentManager = departmentManager;
        _fileStorageService = fileStorageService;
    }

    public async Task<IActionResult> Index()
    {
        var departments = await _departmentManager.GetAllAsync();
        var model = new DepartmentIndexViewModel
        {
            Departments = departments
                .Select(x => new DepartmentCardViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    Description = x.Description,
                    LogoPath = x.LogoPath,
                    UsersCount = x.Users.Count,
                    ActivitiesCount = x.Activities.Count
                }).ToList()
        };

        return View(model);
    }

    [Authorize(Roles = RoleNames.SuperAdmin)]
    public IActionResult Create()
    {
        return View(new DepartmentFormViewModel());
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.SuperAdmin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DepartmentFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var uploadedLogo = await _fileStorageService.SaveFileAsync(model.LogoFile, "departments");
        await _departmentManager.AddAsync(new Department
        {
            Name = model.Name,
            Code = model.Code,
            Description = model.Description,
            LogoPath = uploadedLogo ?? model.LogoPath ?? "/images/enit-logo.svg"
        });

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.SuperAdmin)]
    public async Task<IActionResult> Edit(int id)
    {
        var department = await _departmentManager.GetByIdAsync(id);
        if (department is null)
        {
            return NotFound();
        }

        return View(new DepartmentFormViewModel
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            Description = department.Description,
            LogoPath = department.LogoPath
        });
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.SuperAdmin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DepartmentFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var department = await _departmentManager.GetByIdAsync(model.Id);
        if (department is null)
        {
            return NotFound();
        }

        department.Name = model.Name;
        department.Code = model.Code;
        department.Description = model.Description;
        var uploadedLogo = await _fileStorageService.SaveFileAsync(model.LogoFile, "departments");
        department.LogoPath = uploadedLogo ?? model.LogoPath ?? department.LogoPath;

        await _departmentManager.UpdateAsync(department);
        return RedirectToAction(nameof(Index));
    }
}
