using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlaylistShare.Api.Data;

/// <summary>
/// Design-time factory so <c>dotnet ef ... --context PostgresDbContext</c> can build the context to
/// scaffold migrations without a running app or a live database. The connection string is a
/// placeholder - migration scaffolding never opens a connection.
/// </summary>
public class PostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
{
    public PostgresDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PostgresDbContext>()
            .UseNpgsql("Host=localhost;Database=playlistshare;Username=postgres;Password=postgres")
            .Options;
        return new PostgresDbContext(options);
    }
}
