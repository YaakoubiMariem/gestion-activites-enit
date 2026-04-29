using Microsoft.AspNetCore.Mvc.Rendering;

namespace ENIT.Web.ViewModels.Activities;

public class ActivityIndexViewModel
{
    public List<ActivityListItemViewModel> Activities { get; set; } = [];
    public IEnumerable<SelectListItem> Departments { get; set; } = [];
    public IEnumerable<SelectListItem> Types { get; set; } = [];
    public int? DepartmentId { get; set; }
    public string? Search { get; set; }
    public string? Type { get; set; }
    public DateTime? Date { get; set; }
}
