using ENIT.BL.Entities;
using ENIT.DAL.Context;
using ENIT.Web.Infrastructure;
using ENIT.Web.Services;
using ENIT.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ENIT.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ENITDbContext _context;
    private readonly FileStorageService _fileStorageService;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ENITDbContext context,
        FileStorageService fileStorageService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
        _fileStorageService = fileStorageService;
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            return View(model);
        }

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public async Task<IActionResult> Register()
    {
        ViewBag.Departments = await GetDepartmentsAsync();
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        ViewBag.Departments = await GetDepartmentsAsync();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber,
            DepartmentId = model.DepartmentId,
            Matricule = model.Matricule,
            EmailConfirmed = true,
            ProfilePhotoPath = "/images/avatar-default.svg"
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        await _userManager.AddToRoleAsync(user, RoleNames.Student);
        await _signInManager.SignInAsync(user, false);

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> Manage()
    {
        var user = await _userManager.Users
            .Include(x => x.Department)
            .FirstAsync(x => x.Id == _userManager.GetUserId(User)!);

        var model = new ManageAccountViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Matricule = user.Matricule,
            DepartmentId = user.DepartmentId,
            ProfilePhotoPath = user.ProfilePhotoPath,
            Departments = await GetDepartmentsAsync()
        };

        ViewBag.PasswordModel = new ChangePasswordViewModel();
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Manage(ManageAccountViewModel model)
    {
        model.Departments = await GetDepartmentsAsync();
        ViewBag.PasswordModel = new ChangePasswordViewModel();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User)!);
        if (user is null)
        {
            return NotFound();
        }

        user.FullName = model.FullName;
        user.Email = model.Email;
        user.UserName = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.Matricule = model.Matricule;
        user.DepartmentId = model.DepartmentId;
        var uploadedPhoto = await _fileStorageService.SaveFileAsync(model.ProfilePhotoFile, "profiles");
        user.ProfilePhotoPath = uploadedPhoto ?? (string.IsNullOrWhiteSpace(model.ProfilePhotoPath)
            ? "/images/avatar-default.svg"
            : model.ProfilePhotoPath);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        TempData["SuccessMessage"] = "Le compte a ete mis a jour avec succes.";
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User)!);
        if (user is null)
        {
            return NotFound();
        }

        var profileModel = new ManageAccountViewModel
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Matricule = user.Matricule,
            DepartmentId = user.DepartmentId,
            ProfilePhotoPath = user.ProfilePhotoPath,
            Departments = await GetDepartmentsAsync()
        };

        if (!ModelState.IsValid)
        {
            ViewBag.PasswordModel = model;
            return View("Manage", profileModel);
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.PasswordModel = model;
            return View("Manage", profileModel);
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["SuccessMessage"] = "Le mot de passe a ete modifie avec succes.";
        return RedirectToAction(nameof(Manage));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task<IEnumerable<SelectListItem>> GetDepartmentsAsync()
    {
        return await _context.Departments
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Code} - {x.Name}"
            })
            .ToListAsync();
    }
}
