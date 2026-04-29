using System.ComponentModel.DataAnnotations;
using ENIT.BL.Common;
using ENIT.BL.Enums;

namespace ENIT.BL.Entities;

public class Activity : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public ActivityType Type { get; set; } = ActivityType.Other;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    public bool IsOnline { get; set; }

    [StringLength(255)]
    public string? Link { get; set; }

    public int Capacity { get; set; }
    public ActivityStatus Status { get; set; } = ActivityStatus.Draft;

    [StringLength(255)]
    public string? CoverImagePath { get; set; }

    public int DepartmentId { get; set; }

    [Required]
    public string CreatedById { get; set; } = string.Empty;

    public Department? Department { get; set; }
    public ApplicationUser? CreatedBy { get; set; }
    public ICollection<Participation> Participations { get; set; } = new HashSet<Participation>();
    public ICollection<ActivityComment> Comments { get; set; } = new HashSet<ActivityComment>();
    public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
}
