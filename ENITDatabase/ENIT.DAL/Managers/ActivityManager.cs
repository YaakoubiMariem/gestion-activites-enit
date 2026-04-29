using ENIT.BL.Entities;
using ENIT.DAL.Context;
using ENIT.DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ENIT.DAL.Managers;

public class ActivityManager : IActivityManager
{
    private readonly ENITDbContext _context;

    public ActivityManager(ENITDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Activity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Activities
            .Include(x => x.Department)
            .Include(x => x.CreatedBy)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Activity>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Activities
            .Include(x => x.Department)
            .Include(x => x.CreatedBy)
            .Where(x => x.DepartmentId == departmentId)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Activity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Activities
            .Include(x => x.Department)
            .Include(x => x.CreatedBy)
            .Include(x => x.Participations)
            .Include(x => x.Comments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Activity> AddAsync(Activity activity, CancellationToken cancellationToken = default)
    {
        _context.Activities.Add(activity);
        await _context.SaveChangesAsync(cancellationToken);
        return activity;
    }

    public async Task<Activity> UpdateAsync(Activity activity, CancellationToken cancellationToken = default)
    {
        _context.Activities.Update(activity);
        await _context.SaveChangesAsync(cancellationToken);
        return activity;
    }

    public async Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Activities.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
