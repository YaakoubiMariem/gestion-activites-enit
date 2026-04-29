namespace ENIT.Web.ViewModels.Participations;

public class MyRegistrationItemViewModel
{
    public int ActivityId { get; set; }
    public string ActivityTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
}
