using Microsoft.EntityFrameworkCore;

namespace PlaylistShare.Api.Data;

/// <summary>
/// PostgreSQL variant of <see cref="ApplicationDbContext"/>. It exists purely to own a separate set of
/// EF Core migrations (Data/Migrations/Postgres) with its own model snapshot, so SQL Server and
/// PostgreSQL each keep provider-specific migration history within the same assembly. The model,
/// entities and configuration are inherited from <see cref="ApplicationDbContext"/> unchanged, and the
/// app injects <see cref="ApplicationDbContext"/> everywhere - this type is only wired up at startup
/// when Database:Provider is "Postgres".
/// </summary>
public class PostgresDbContext : ApplicationDbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options) { }
}
