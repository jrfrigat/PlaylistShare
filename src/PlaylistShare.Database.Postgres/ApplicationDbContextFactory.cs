using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlaylistShare.Database.Postgres;

/// <summary>
/// Design-time фабрика для "dotnet ef ... --project src/PlaylistShare.Database.Postgres".
/// Парная к фабрике в PlaylistShare.Database.SqlServer - см. комментарий там.
/// Строка подключения - заглушка: скаффолдинг миграций к базе не подключается.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseConnection(new DBConnection
        {
            Provider = DBConnectionExtensions.Postgres,
            ConnectionString = "Host=localhost;Database=playlistshare;Username=postgres;Password=postgres",
        });

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
