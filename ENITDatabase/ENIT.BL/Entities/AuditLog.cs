using System.ComponentModel.DataAnnotations;
using ENIT.BL.Common;

namespace ENIT.BL.Entities;

public class AuditLog : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string EntityName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? EntityId { get; set; }

    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? PerformedById { get; set; }
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser? PerformedBy { get; set; }
}
