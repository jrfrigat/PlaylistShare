using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlaylistShare.Api.Data;

/// <summary>
/// Design-time factory для SqlServer-варианта, парная к <see cref="PostgresDbContextFactory"/>.
/// Строка подключения - заглушка: при генерации миграций к базе не подключаются.
/// </summary>
/// <remarks>
/// НЕ УДАЛЯЙТЕ, даже если кажется, что дублирует регистрацию из Program.cs. Без неё
/// "dotnet ef ... --context ApplicationDbContext" МОЛЧА скаффолдит миграцию для Postgres.
///
/// Причина: IDesignTimeDbContextFactory&lt;out TContext&gt; ковариантен, а PostgresDbContext
/// наследует ApplicationDbContext. Поэтому PostgresDbContextFactory подходит под тип
/// IDesignTimeDbContextFactory&lt;ApplicationDbContext&gt;, и EF, подбирая фабрику для
/// ApplicationDbContext перебором с FirstOrDefault, цепляет именно её - вместе с Npgsql.
/// Явная фабрика с точным TContext ложится в таблицу контекстов по своему ключу, и до
/// ковариантного перебора дело уже не доходит.
/// </remarks>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=localhost;Database=playlistshare;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;
        return new ApplicationDbContext(options);
    }
}
