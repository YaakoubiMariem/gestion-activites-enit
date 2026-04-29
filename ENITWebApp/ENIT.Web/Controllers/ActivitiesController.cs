using ENIT.BL.Entities;
using ENIT.BL.Enums;
using ENIT.DAL.Context;
using ENIT.DAL.Contracts;
using ENIT.Web.Infrastructure;
using ENIT.Web.Services;
using ENIT.Web.ViewModels.Activities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ENIT.Web.Controllers;

public class ActivitiesController : Controller
{
    private readonly IActivityManager _activityManager;
    private readonly IDepartmentManager _departmentManager;
    private readonly IParticipationManager _participationManager;
    private readonly IActivityCommentManager _commentManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ENITDbContext _context;
    private readonly FileStorageService _fileStorageService;

    public ActivitiesController(
        IActivityManager activityManager,
        IDepartmentManager departmentManager,
        IParticipationManager participationManager,
        IActivityCommentManager commentManager,
        UserManager<ApplicationUser> userManager,
        ENITDbContext context,
        FileStorageService fileStorageService)
    {
        _activityManager = activityManager;
        _departmentManager = departmentManager;
        _participationManager = participationManager;
        _commentManager = commentManager;
        _userManager = userManager;
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<IActionResult> Index(int? departmentId, string? search, string? type, DateTime? date)
    {
        var activities = await _context.Activities
            .Include(x => x.Department)
            .Include(x => x.Participations)
            .OrderBy(x => x.StartDate)
            .ToListAsync();

        if (departmentId.HasValue)
        {
            activities = activities.Where(x => x.DepartmentId == departmentId.Value).ToList();
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            activities = activities.Where(x =>
                x.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<ActivityType>(type, out var activityType))
        {
            activities = activities.Where(x => x.Type == activityType).ToList();
        }

        if (date.HasValue)
        {
            activities = activities.Where(x => x.StartDate.Date == date.Value.Date).ToList();
        }

        var currentUserId = _userManager.GetUserId(User);
        var model = new ActivityIndexViewModel
        {
            DepartmentId = departmentId,
            Search = search,
            Type = type,
            Date = date,
            Departments = (await _departmentManager.GetAllAsync())
                .Select(x => new SelectListItem(x.Name, x.Id.ToString())),
            Types = Enum.GetValues<ActivityType>()
                .Select(x => new SelectListItem(x.ToString(), x.ToString())),
            Activities = activities.Select(x => new ActivityListItemViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Type = x.Type.ToString(),
                Status = x.Status.ToString(),
                DepartmentName = x.Department?.Name ?? string.Empty,
                CoverImagePath = x.CoverImagePath,
                Location = x.Location,
                IsOnline = x.IsOnline,
                Link = x.Link,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Capacity = x.Capacity,
                RegisteredCount = x.Participations.Count(p => p.Status == ParticipationStatus.Confirmed),
                IsUserRegistered = currentUserId is not null && x.Participations.Any(p => p.UserId == currentUserId)
            }).ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var activity = await _context.Activities
            .Include(x => x.Department)
            .Include(x => x.Participations).ThenInclude(x => x.User)
            .Include(x => x.Comments).ThenInclude(x => x.User).ThenInclude(x => x!.Department)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (activity is null)
        {
            return NotFound();
        }

        var currentUserId = _userManager.GetUserId(User);
        var model = new ActivityDetailsViewModel
        {
            Activity = new ActivityListItemViewModel
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                Type = activity.Type.ToString(),
                Status = activity.Status.ToString(),
                DepartmentName = activity.Department?.Name ?? string.Empty,
                CoverImagePath = activity.CoverImagePath,
                Location = activity.Location,
                IsOnline = activity.IsOnline,
                Link = activity.Link,
                StartDate = activity.StartDate,
                EndDate = activity.EndDate,
                Capacity = activity.Capacity,
                RegisteredCount = activity.Participations.Count(x => x.Status == ParticipationStatus.Confirmed),
                IsUserRegistered = currentUserId is not null && activity.Participations.Any(x => x.UserId == currentUserId)
            },
            NewComment = new ActivityCommentViewModel { ActivityId = activity.Id },
            Comments = activity.Comments
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new CommentDisplayViewModel
                {
                    UserName = x.User?.FullName ?? "Utilisateur",
                    DepartmentName = x.User?.Department?.Code,
                    Content = x.Content,
                    Rating = x.Rating,
                    CreatedAt = x.CreatedAt
                }).ToList(),
            Participants = activity.Participations
                .OrderBy(x => x.RegistrationDate)
                .Select(x => new ParticipantDisplayViewModel
                {
                    FullName = x.User?.FullName ?? string.Empty,
                    Matricule = x.User?.Matricule,
                    Status = x.Status.ToString(),
                    RegistrationDate = x.RegistrationDate
                }).ToList()
        };

        return View(model);
    }

    [Authorize(Roles = $"{RoleNames.DepartmentAdmin},{RoleNames.SuperAdmin}")]
    public async Task<IActionResult> Create()
    {
        return View(await BuildFormViewModelAsync(new ActivityFormViewModel
        {
            StartDate = DateTime.Today.AddDays(1).AddHours(9),
            EndDate = DateTime.Today.AddDays(1).AddHours(12)
        }));
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.DepartmentAdmin},{RoleNames.SuperAdmin}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActivityFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(await BuildFormViewModelAsync(model));
        }

        var user = await _userManager.Users.FirstAsync(x => x.Id == _userManager.GetUserId(User)!);
        if (User.IsInRole(RoleNames.DepartmentAdmin) && user.DepartmentId.HasValue)
        {
            model.DepartmentId = user.DepartmentId.Value;
        }

        var uploadedCover = await _fileStorageService.SaveFileAsync(model.CoverImageFile, "activities");
        var activity = new Activity
        {
            Title = model.Title,
            Description = model.Description,
            Type = model.Type,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Location = model.Location,
            IsOnline = model.IsOnline,
            Link = model.Link,
            Capacity = model.Capacity,
            Status = model.Status,
            DepartmentId = model.DepartmentId,
            CoverImagePath = uploadedCover ?? model.CoverImagePath ?? "/images/activity-campus.svg",
            CreatedById = user.Id
        };

        await _activityManager.AddAsync(activity);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = $"{RoleNames.DepartmentAdmin},{RoleNames.SuperAdmin}")]
    public async Task<IActionResult> Edit(int id)
    {
        var activity = await _activityManager.GetByIdAsync(id);
        if (activity is null)
        {
            return NotFound();
        }

        var model = new ActivityFormViewModel
        {
            Id = activity.Id,
            Title = activity.Title,
            Description = activity.Description,
            Type = activity.Type,
            StartDate = activity.StartDate,
            EndDate = activity.EndDate,
            Location = activity.Location,
            IsOnline = activity.IsOnline,
            Link = activity.Link,
            Capacity = activity.Capacity,
            Status = activity.Status,
            DepartmentId = activity.DepartmentId,
            CoverImagePath = activity.CoverImagePath
        };

        return View(await BuildFormViewModelAsync(model));
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.DepartmentAdmin},{RoleNames.SuperAdmin}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ActivityFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(await BuildFormViewModelAsync(model));
        }

        var activity = await _activityManager.GetByIdAsync(model.Id);
        if (activity is null)
        {
            return NotFound();
        }

        activity.Title = model.Title;
        activity.Description = model.Description;
        activity.Type = model.Type;
        activity.StartDate = model.StartDate;
        activity.EndDate = model.EndDate;
        activity.Location = model.Location;
        activity.IsOnline = model.IsOnline;
        activity.Link = model.Link;
        activity.Capacity = model.Capacity;
        activity.Status = model.Status;
        activity.DepartmentId = model.DepartmentId;
        var uploadedCover = await _fileStorageService.SaveFileAsync(model.CoverImageFile, "activities");
        activity.CoverImagePath = uploadedCover ?? model.CoverImagePath ?? activity.CoverImagePath;

        await _activityManager.UpdateAsync(activity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.DepartmentAdmin},{RoleNames.SuperAdmin}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _activityManager.SoftDeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var existing = await _participationManager.GetAsync(id, userId);
        if (existing is null)
        {
            await _participationManager.AddAsync(new Participation
            {
                ActivityId = id,
                UserId = userId
            });
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unregister(int id)
    {
        await _participationManager.CancelAsync(id, _userManager.GetUserId(User)!);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(ActivityCommentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Details), new { id = model.ActivityId });
        }

        await _commentManager.AddAsync(new ActivityComment
        {
            ActivityId = model.ActivityId,
            UserId = _userManager.GetUserId(User)!,
            Content = model.Content,
            Rating = model.Rating
        });

        return RedirectToAction(nameof(Details), new { id = model.ActivityId });
    }

    private async Task<ActivityFormViewModel> BuildFormViewModelAsync(ActivityFormViewModel model)
    {
        var departments = await _departmentManager.GetAllAsync();
        var user = User.Identity?.IsAuthenticated == true
            ? await _userManager.Users.FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User))
            : null;

        var filteredDepartments = departments;
        if (User.IsInRole(RoleNames.DepartmentAdmin) && user?.DepartmentId is not null)
        {
            filteredDepartments = departments.Where(x => x.Id == user.DepartmentId.Value);
            model.DepartmentId = user.DepartmentId.Value;
        }

        model.Departments = filteredDepartments.Select(x => new SelectListItem($"{x.Code} - {x.Name}", x.Id.ToString()));
        return model;
    }
}
