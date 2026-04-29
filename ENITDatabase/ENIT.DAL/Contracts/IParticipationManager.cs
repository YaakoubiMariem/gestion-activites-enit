using ENIT.BL.Entities;

namespace ENIT.DAL.Contracts;

public interface IParticipationManager
{
    Task<IEnumerable<Participation>> GetByActivityAsync(int activityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Participation>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<Participation?> GetAsync(int activityId, string userId, CancellationToken cancellationToken = default);
    Task<Participation> AddAsync(Participation participation, CancellationToken cancellationToken = default);
    Task<Participation> UpdateAsync(Participation participation, CancellationToken cancellationToken = default);
    Task CancelAsync(int activityId, string userId, CancellationToken cancellationToken = default);
}
