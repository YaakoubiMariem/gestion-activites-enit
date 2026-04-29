using ENIT.BL.Entities;

namespace ENIT.DAL.Contracts;

public interface IActivityCommentManager
{
    Task<ActivityComment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ActivityComment>> GetByActivityAsync(int activityId, CancellationToken cancellationToken = default);
    Task<ActivityComment> AddAsync(ActivityComment comment, CancellationToken cancellationToken = default);
    Task<ActivityComment> UpdateAsync(ActivityComment comment, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
