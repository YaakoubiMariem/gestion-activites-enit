using ENIT.BL.Entities;
using ENIT.DAL.Context;
using ENIT.DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ENIT.DAL.Managers;

public class DepartmentManager : IDepartmentManager
{
    private readonly ENITDbContext _context;

    public DepartmentManager(ENITDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .Include(x => x.Activities)
            .Include(x => x.Users)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .Include(x => x.Activities)
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<Department> AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);
        return department;
    }

    public async Task<Department> UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync(cancellationToken);
        return department;
    }

    public async Task SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
