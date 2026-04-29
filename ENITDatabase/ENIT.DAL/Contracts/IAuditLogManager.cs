using ENIT.BL.Entities;

namespace ENIT.DAL.Contracts;

public interface IAuditLogManager
{
    Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuditLog> AddAsync(AuditLog log, CancellationToken cancellationToken = default);
}
