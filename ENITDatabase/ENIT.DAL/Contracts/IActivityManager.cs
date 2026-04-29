using ENIT.BL.Entities;

namespace ENIT.DAL.Contracts;

public interface IActivityManager
{
    Task<IEnumerable<Activity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Activity>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<Activity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Activity> AddAsync(Activity activity, CancellationToken cancellationToken = default);
    Task<Activity> UpdateAsync(Activity activity, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
