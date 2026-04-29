namespace ENIT.Web.ViewModels.Dashboard;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public string? Matricule { get; set; }
    public string AcademicSession { get; set; } = string.Empty;
    public List<StatCardViewModel> Stats { get; set; } = [];
    public List<ActivitySnapshotViewModel> FeaturedActivities { get; set; } = [];
    public List<ActivitySnapshotViewModel> UpcomingActivities { get; set; } = [];
    public List<NotificationViewModel> Notifications { get; set; } = [];
    public List<DepartmentSummaryViewModel> Departments { get; set; } = [];
    public List<CalendarEventViewModel> CalendarEvents { get; set; } = [];
}

public class StatCardViewModel
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Accent { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class ActivitySnapshotViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? CoverImagePath { get; set; }
    public int RegisteredCount { get; set; }
}

public class NotificationViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

public class DepartmentSummaryViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
    public int UserCount { get; set; }
}

public class CalendarEventViewModel
{
    public string Title { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
}
