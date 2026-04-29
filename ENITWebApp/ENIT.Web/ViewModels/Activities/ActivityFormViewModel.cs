using System.ComponentModel.DataAnnotations;
using ENIT.BL.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ENIT.Web.ViewModels.Activities;

public class ActivityFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public ActivityType Type { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    public bool IsOnline { get; set; }

    [Url]
    public string? Link { get; set; }

    [Range(1, 10000)]
    public int Capacity { get; set; } = 50;

    public ActivityStatus Status { get; set; } = ActivityStatus.Published;
    public int DepartmentId { get; set; }
    public string? CoverImagePath { get; set; }
    public IFormFile? CoverImageFile { get; set; }
    public IEnumerable<SelectListItem> Departments { get; set; } = [];
}
