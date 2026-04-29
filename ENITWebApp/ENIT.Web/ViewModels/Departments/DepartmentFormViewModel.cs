using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ENIT.Web.ViewModels.Departments;

public class DepartmentFormViewModel
{
    public int Id { get; set; }

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

    public IFormFile? LogoFile { get; set; }
}
