using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseNpgsql("Host=localhost;Port=5432;Database=LibraryDb;Username=postgres;Password=postgres");

        return new ApplicationDbContext(builder.Options);
    }
}