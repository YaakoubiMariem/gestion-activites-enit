using System.ComponentModel.DataAnnotations;
using ENIT.BL.Common;
using ENIT.BL.Enums;

namespace ENIT.BL.Entities;

public class Notification : BaseEntity
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1500)]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.General;
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = string.Empty;
    public int? ActivityId { get; set; }

    public ApplicationUser? User { get; set; }
    public Activity? Activity { get; set; }
}
