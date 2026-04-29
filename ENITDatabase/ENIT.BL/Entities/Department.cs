using System.ComponentModel.DataAnnotations;
using ENIT.BL.Common;

namespace ENIT.BL.Entities;

public class Department : BaseEntity
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? LogoPath { get; set; }

    public ICollection<Activity> Activities { get; set; } = new HashSet<Activity>();
    public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
}
