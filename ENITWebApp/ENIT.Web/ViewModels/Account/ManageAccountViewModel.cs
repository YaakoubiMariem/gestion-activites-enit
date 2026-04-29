using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace ENIT.Web.ViewModels.Account;

public class ManageAccountViewModel
{
    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    [StringLength(50)]
    public string? Matricule { get; set; }

    public int? DepartmentId { get; set; }

    [StringLength(255)]
    public string? ProfilePhotoPath { get; set; }

    public IFormFile? ProfilePhotoFile { get; set; }

    public IEnumerable<SelectListItem> Departments { get; set; } = [];
}
