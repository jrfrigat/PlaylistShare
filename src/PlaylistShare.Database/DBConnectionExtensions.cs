using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PlaylistShare.Database;

/// <summary>
/// Единственное место, где имя провайдера превращается в настроенный DbContextOptions.
/// </summary>
public static class DBConnectionExtensions
{
    /// <summary>Провайдер по умолчанию: так вели себя старые развёртывания без Database:Provider.</summary>
    public const string SqlServer = "SqlServer";
    public const string Postgres = "Postgres";

    // Сборки миграций адресуются строкой, а не typeof: это они ссылаются сюда, а не наоборот.
    // Ошибка в имени всплывёт на первом же Migrate() как FileNotFoundException, поэтому имена
    // держим здесь, а не размазываем по фабрикам и Program.cs.
    public const string SqlServerMigrationsAssembly = "PlaylistShare.Database.SqlServer";
    public const string PostgresMigrationsAssembly = "PlaylistShare.Database.Postgres";

    /// <summary>
    /// Читает настройки подключения. Имена ключей менять нельзя: на них завязаны рабочие .env и
    /// docker-compose (Database__Provider, ConnectionStrings__DefaultConnection).
    /// </summary>
    public static DBConnection GetDBConnection(this IConfiguration configuration)
    {
        var database = configuration.GetSection("Database");

        return new DBConnection
        {
            Provider = database.GetValue<string>("Provider") ?? SqlServer,
            ConnectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Строка подключения ConnectionStrings:DefaultConnection не задана "
                    + "(в Docker это переменная ConnectionStrings__DefaultConnection, см. .env)."),
            // 0 - не повторять: ровно то поведение, что было до появления этих ключей.
            MaxRetryCount = database.GetValue("MaxRetryCount", 0),
            MaxRetryDelay = database.GetValue("MaxRetryDelay", 30),
        };
    }

    /// <summary>
    /// Настраивает провайдера и его набор миграций. Неизвестный провайдер - это ошибка конфигурации,
    /// а не повод молча взять SqlServer: опечатка в Database:Provider иначе увела бы прод в другую
    /// СУБД и в чужой набор миграций.
    /// </summary>
    public static DbContextOptionsBuilder UseConnection(
        this DbContextOptionsBuilder optionsBuilder,
        DBConnection connection)
    {
        var provider = connection.Provider.Trim();

        return provider switch
        {
            _ when Is(provider, SqlServer) => optionsBuilder.UseSqlServer(connection.ConnectionString, opt =>
            {
                opt.MigrationsAssembly(SqlServerMigrationsAssembly);
                // EnableRetryOnFailure у каждого провайдера свой, в общем предке его нет, поэтому
                // условие приходится повторять в обеих ветках.
                if (connection.MaxRetryCount > 0)
                {
                    opt.EnableRetryOnFailure(
                        connection.MaxRetryCount,
                        TimeSpan.FromSeconds(connection.MaxRetryDelay),
                        null);
                }
            }),

            _ when Is(provider, Postgres) => optionsBuilder.UseNpgsql(connection.ConnectionString, opt =>
            {
                opt.MigrationsAssembly(PostgresMigrationsAssembly);
                if (connection.MaxRetryCount > 0)
                {
                    opt.EnableRetryOnFailure(
                        connection.MaxRetryCount,
                        TimeSpan.FromSeconds(connection.MaxRetryDelay),
                        null);
                }
            }),

            _ => throw UnsupportedProvider(provider),
        };
    }

    /// <summary>
    /// Тот же текст ошибки для всех, кто ветвится по провайдеру (например, выбор session-кэша в
    /// Program.cs): список поддерживаемых значений не должен разъезжаться по копиям.
    /// </summary>
    public static InvalidOperationException UnsupportedProvider(string? provider) =>
        new($"Database:Provider = '{provider}' не поддерживается. "
            + $"Допустимые значения: {SqlServer}, {Postgres}.");

    /// <summary>Регистр значения не считаем опечаткой: "postgres" в .env должен работать.</summary>
    public static bool Is(string? provider, string expected) =>
        string.Equals(provider?.Trim(), expected, StringComparison.OrdinalIgnoreCase);
}
