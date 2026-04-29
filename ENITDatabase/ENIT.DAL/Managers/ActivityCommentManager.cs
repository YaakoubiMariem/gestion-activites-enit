using ENIT.BL.Entities;
using ENIT.DAL.Context;
using ENIT.DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ENIT.DAL.Managers;

public class ActivityCommentManager : IActivityCommentManager
{
    private readonly ENITDbContext _context;

    public ActivityCommentManager(ENITDbContext context)
    {
        _context = context;
    }

    public async Task<ActivityComment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ActivityComments
            .Include(x => x.User)
            .Include(x => x.Activity)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ActivityComment>> GetByActivityAsync(int activityId, CancellationToken cancellationToken = default)
    {
        return await _context.ActivityComments
            .Include(x => x.User)
            .Where(x => x.ActivityId == activityId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ActivityComment> AddAsync(ActivityComment comment, CancellationToken cancellationToken = default)
    {
        _context.ActivityComments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);
        return comment;
    }

    public async Task<ActivityComment> UpdateAsync(ActivityComment comment, CancellationToken cancellationToken = default)
    {
        _context.ActivityComments.Update(comment);
        await _context.SaveChangesAsync(cancellationToken);
        return comment;
    }

    public async Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ActivityComments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
