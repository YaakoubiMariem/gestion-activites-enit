using System.ComponentModel.DataAnnotations;
using ENIT.BL.Common;

namespace ENIT.BL.Entities;

public class ActivityComment : BaseEntity
{
    public int ActivityId { get; set; }
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    [Range(1, 5)]
    public int? Rating { get; set; }

    public Activity? Activity { get; set; }
    public ApplicationUser? User { get; set; }
}
