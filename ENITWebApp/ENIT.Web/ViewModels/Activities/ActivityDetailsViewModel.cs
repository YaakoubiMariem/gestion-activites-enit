namespace ENIT.Web.ViewModels.Activities;

public class ActivityDetailsViewModel
{
    public ActivityListItemViewModel Activity { get; set; } = new();
    public List<CommentDisplayViewModel> Comments { get; set; } = [];
    public ActivityCommentViewModel NewComment { get; set; } = new();
    public List<ParticipantDisplayViewModel> Participants { get; set; } = [];
}

public class CommentDisplayViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ParticipantDisplayViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string? Matricule { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
}
