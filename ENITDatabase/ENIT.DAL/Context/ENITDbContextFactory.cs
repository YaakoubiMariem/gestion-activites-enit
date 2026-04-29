using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ENIT.DAL.Context;

public class ENITDbContextFactory : IDesignTimeDbContextFactory<ENITDbContext>
{
    public ENITDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ENITDbContext>();
        const string connectionString =
            "Server=(localdb)\\mssqllocaldb;Database=ENITActivitiesHubDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString);

        return new ENITDbContext(optionsBuilder.Options);
    }
}
