using ENIT.BL.Entities;
using ENIT.BL.Enums;
using ENIT.DAL.Context;
using ENIT.DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ENIT.DAL.Managers;

public class ParticipationManager : IParticipationManager
{
    private readonly ENITDbContext _context;

    public ParticipationManager(ENITDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Participation>> GetByActivityAsync(int activityId, CancellationToken cancellationToken = default)
    {
        return await _context.Participations
            .Include(x => x.User)
            .Where(x => x.ActivityId == activityId)
            .OrderBy(x => x.RegistrationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Participation>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Participations
            .Include(x => x.Activity)
            .ThenInclude(x => x!.Department)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.RegistrationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Participation?> GetAsync(int activityId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Participations
            .FirstOrDefaultAsync(x => x.ActivityId == activityId && x.UserId == userId, cancellationToken);
    }

    public async Task<Participation> AddAsync(Participation participation, CancellationToken cancellationToken = default)
    {
        var confirmedCount = await _context.Participations
            .CountAsync(x => x.ActivityId == participation.ActivityId && x.Status == ParticipationStatus.Confirmed, cancellationToken);

        var activity = await _context.Activities.FirstAsync(x => x.Id == participation.ActivityId, cancellationToken);
        participation.Status = confirmedCount < activity.Capacity
            ? ParticipationStatus.Confirmed
            : ParticipationStatus.Waitlist;

        _context.Participations.Add(participation);
        await _context.SaveChangesAsync(cancellationToken);
        return participation;
    }

    public async Task<Participation> UpdateAsync(Participation participation, CancellationToken cancellationToken = default)
    {
        _context.Participations.Update(participation);
        await _context.SaveChangesAsync(cancellationToken);
        return participation;
    }

    public async Task CancelAsync(int activityId, string userId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Participations
            .FirstOrDefaultAsync(x => x.ActivityId == activityId && x.UserId == userId, cancellationToken);

        if (entity is null)
        {
            return;
        }

        entity.Status = ParticipationStatus.Cancelled;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
