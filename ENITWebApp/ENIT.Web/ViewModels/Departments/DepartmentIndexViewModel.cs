namespace ENIT.Web.ViewModels.Departments;

public class DepartmentIndexViewModel
{
    public List<DepartmentCardViewModel> Departments { get; set; } = [];
}

public class DepartmentCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoPath { get; set; }
    public int UsersCount { get; set; }
    public int ActivitiesCount { get; set; }
}
