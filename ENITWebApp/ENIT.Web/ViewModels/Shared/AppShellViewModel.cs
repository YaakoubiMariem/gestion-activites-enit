namespace ENIT.Web.ViewModels.Shared;

public class AppShellViewModel
{
    public bool IsAuthenticated { get; set; }
    public string? FullName { get; set; }
    public string? DepartmentName { get; set; }
    public string? RoleName { get; set; }
    public string? ProfilePhotoPath { get; set; }
}
