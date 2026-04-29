using System.ComponentModel.DataAnnotations;

namespace ENIT.Web.ViewModels.Activities;

public class ActivityCommentViewModel
{
    public int ActivityId { get; set; }

    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    [Range(1, 5)]
    public int? Rating { get; set; }
}
