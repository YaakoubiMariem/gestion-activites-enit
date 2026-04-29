namespace ENIT.Web.ViewModels.Activities;

public class ActivityListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string? CoverImagePath { get; set; }
    public string? Location { get; set; }
    public bool IsOnline { get; set; }
    public string? Link { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public bool IsUserRegistered { get; set; }
}
