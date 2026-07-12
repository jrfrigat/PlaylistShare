using PlaylistShare.Shared.Yandex;

namespace PlaylistShare.Pwa.Services;

/// <summary>
/// Глобальный сервис управления аудиоплеером.
/// Позволяет управлять воспроизведением из любого компонента.
/// </summary>
public interface IAudioPlayerService
{
    #region Состояние плеера (для чтения)
    /// <summary>ID текущего воспроизводимого трека (null, если ничего не играет).</summary>
    string? CurrentTrackId { get; }

    /// <summary>Очередь треков.</summary>
    IReadOnlyList<YandexTrack> CurrentQueue { get; }

    /// <summary>Есть ли следующий трек в очереди.</summary>
    bool HasNext { get; }

    /// <summary>Есть ли предыдущий трек в очереди.</summary>
    bool HasPrevious { get; }

    /// <summary>Играет ли в данный момент (true) или приостановлен (false).</summary>
    bool IsPlaying { get; }

    /// <summary>Текущая громкость (0-100).</summary>
    double CurrentVolume { get; set; }

    /// <summary>Прогресс воспроизведения в процентах (0-100).</summary>
    double CurrentProgress { get; }

    /// <summary>Текущее время в секундах.</summary>
    double CurrentTime { get; }

    /// <summary>Общая длительность в секундах</summary>
    double TotalTime { get; }

    /// <summary>Отформатированное текущее время (мм:сс).</summary>
    string CurrentTimeString { get; }

    /// <summary>Отформатированная общая длительность (мм:сс).</summary>
    string TotalTimeString { get; }

    /// <summary>Текущий трек.</summary>
    YandexTrack? CurrentTrack { get; }
    #endregion

    #region Команды управления (вызываются из компонентов)
    /// <summary>Загрузить и начать воспроизведение трека.</summary>
    /// <param name="trackId">ID трека.</param>
    /// <param name="accessToken">Опциональный access-токен (если не указан, будет взят из хранилища).</param>
    /// <param name="sharedPlaylistId">ID расшаренного плейлиста (для неавторизованного доступа).</param>
    /// <param name="title">Название трека. (Если не передано, вызывает api для получения)</param>
    /// <param name="coverUrl">URL обложки трека. (Если не передано, вызывает api для получения)</param>
    Task LoadAndPlayAsync(string trackId, string? accessToken = null, string? playlistShareToken = null, YandexTrack? track = null);

    /// <summary>Воспроизвести (если трек загружен и на паузе).</summary>
    Task PlayAsync();

    /// <summary>Поставить на паузу.</summary>
    Task PauseAsync();

    /// <summary>Перемотать на секунды.</summary>
    Task SeekToAsync(double second);

    /// <summary>Установить громкость (0-100).</summary>
    Task SetVolumeAsync(double volume);

    /// <summary>Установить очередь треков и начать воспроизведение с указанного индекса.</summary>
    void SetQueue(IEnumerable<YandexTrack> tracks, int startIndex = 0, string? shareToken = null);

    /// <summary>Перейти к следующему треку в очереди.</summary>
    Task PlayNextAsync();

    /// <summary>Перейти к предыдущему треку в очереди.</summary>
    Task PlayPreviousAsync();
    #endregion

    #region События для подписки на изменения состояния
    /// <summary>
    /// Событие, возникающее при любом изменении состояния плеера:
    /// смена трека, старт/пауза/стоп, обновление прогресса, изменение громкости, окончание трека.
    /// Подписывайтесь на него, чтобы перерисовывать UI (например, иконку "пауза/плей").
    /// </summary>
    event Action? OnStateChanged;

    event Action? OnStartedTrack;

    event Action? OnEndedTrack;
    #endregion

    #region События для связи с реальным компонентом AudioPlayer (Эти события вызываются сервисом)
    /// <summary>Запрос на загрузку и воспроизведение трека.</summary>
    event Func<string, string?, string?, Task>? OnLoadAndPlayRequested;

    /// <summary>Запрос на воспроизведение (снять с паузы).</summary>
    event Func<Task>? OnPlayRequested;

    /// <summary>Запрос на паузу.</summary>
    event Func<Task>? OnPauseRequested;

    /// <summary>Запрос на перемотку (секунды).</summary>
    event Func<double, Task>? OnSeekRequested;

    /// <summary>Запрос на изменение громкости (0-100).</summary>
    event Func<double, Task>? OnVolumeChangeRequested;
    #endregion

    #region Методы для обновления состояния из AudioPlayer (Вызываются компонентом AudioPlayer, когда реальный аудиоэлемент меняет своё состояние.)
    /// <summary>Уведомить сервис о том, что трек начал или прекратил играть.</summary>
    void SetPlayingState(bool isPlaying);

    /// <summary>Установить ID текущего трека.</summary>
    void SetCurrentTrack(string? trackId);

    /// <summary>Обновить прогресс и отображаемое время.</summary>
    void UpdateProgress(double currentTime, double totalTime);

    /// <summary>Уведомить об окончании трека.</summary>
    void NotifyTrackEnded();
    #endregion
}