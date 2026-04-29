using ENIT.BL.Common;
using ENIT.BL.Enums;

namespace ENIT.BL.Entities;

public class Participation : BaseEntity
{
    public int ActivityId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public ParticipationStatus Status { get; set; } = ParticipationStatus.Confirmed;

    public Activity? Activity { get; set; }
    public ApplicationUser? User { get; set; }
}
