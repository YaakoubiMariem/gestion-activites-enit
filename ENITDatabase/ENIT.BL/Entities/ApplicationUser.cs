using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ENIT.BL.Entities;

public class ApplicationUser : IdentityUser
{
    public int? DepartmentId { get; set; }

    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Matricule { get; set; }

    [StringLength(255)]
    public string? ProfilePhotoPath { get; set; }

    public bool IsActive { get; set; } = true;

    public Department? Department { get; set; }
    public ICollection<Activity> CreatedActivities { get; set; } = new HashSet<Activity>();
    public ICollection<Participation> Participations { get; set; } = new HashSet<Participation>();
    public ICollection<ActivityComment> Comments { get; set; } = new HashSet<ActivityComment>();
    public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new HashSet<AuditLog>();
}
