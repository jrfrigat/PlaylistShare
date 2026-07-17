namespace PlaylistShare.Database;

/// <summary>
/// Настройки подключения к БД: всё, что нужно, чтобы собрать DbContextOptions, одним значением.
/// Читается из секции Database + ConnectionStrings:DefaultConnection (см. <see cref="DBConnectionExtensions"/>).
/// </summary>
public sealed class DBConnection
{
    /// <summary>Имя провайдера. Поддерживаемые значения перечислены в <see cref="DBConnectionExtensions"/>.</summary>
    public required string Provider { get; init; }

    public required string ConnectionString { get; init; }

    /// <summary>
    /// Сколько раз повторять запрос, упавший с транзиентной ошибкой (0 - не повторять).
    /// Это НЕ про ожидание старта БД: стратегия работает поверх уже открытого соединения и спасает
    /// живой API от кратковременных обрывов, а не первый Migrate() от ещё не поднятого контейнера.
    /// </summary>
    public int MaxRetryCount { get; init; }

    /// <summary>Максимальная пауза между такими повторами, в секундах.</summary>
    public int MaxRetryDelay { get; init; }
}
