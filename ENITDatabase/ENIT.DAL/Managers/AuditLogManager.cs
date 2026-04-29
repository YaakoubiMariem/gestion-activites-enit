using ENIT.BL.Entities;
using ENIT.DAL.Context;
using ENIT.DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ENIT.DAL.Managers;

public class AuditLogManager : IAuditLogManager
{
    private readonly ENITDbContext _context;

    public AuditLogManager(ENITDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Include(x => x.PerformedBy)
            .OrderByDescending(x => x.PerformedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<AuditLog> AddAsync(AuditLog log, CancellationToken cancellationToken = default)
    {
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);
        return log;
    }
}
