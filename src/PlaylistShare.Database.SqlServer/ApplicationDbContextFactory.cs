using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlaylistShare.Database.SqlServer;

/// <summary>
/// Design-time фабрика для "dotnet ef ... --project src/PlaylistShare.Database.SqlServer".
/// Строка подключения - заглушка: скаффолдинг миграций к базе не подключается.
/// </summary>
/// <remarks>
/// Провайдер зашит в саму фабрику, а фабрика - в сборку своих миграций. Поэтому перепутать наборы
/// нельзя: раньше PostgresDbContext наследовал ApplicationDbContext, из-за ковариантности
/// IDesignTimeDbContextFactory&lt;out TContext&gt; postgres-фабрика подходила под тип
/// ApplicationDbContext, и EF молча цеплял её первой. Теперь контекст один, а фабрики лежат в
/// разных сборках и вместе не сканируются.
/// </remarks>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseConnection(new DBConnection
        {
            Provider = DBConnectionExtensions.SqlServer,
            ConnectionString = "Server=localhost;Database=playlistshare;Trusted_Connection=True;TrustServerCertificate=True",
        });

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
