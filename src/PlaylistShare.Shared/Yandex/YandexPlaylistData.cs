namespace PlaylistShare.Shared.Yandex;

public class YandexPlaylistData
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public List<YandexTrack> Tracks { get; set; } = new();

    /// <summary>
    /// Id треков из Tracks, которые текущий пользователь вправе удалить. Заполняет сервер
    /// (SharedPlaylistService.GetRemovableTrackIdsAsync), интерфейс только рисует по этому набору -
    /// правила доступа живут в одном месте.
    ///
    /// Признак лежит здесь, а не на YandexTrack, потому что YandexTrack - зеркало данных
    /// Яндекс.Музыки и переиспользуется там, где никакого плейлиста нет (выдача поиска, очередь
    /// плеера); право на удаление же существует только в контексте расшаренного плейлиста, то есть
    /// ровно в YandexPlaylistData, которую отдаёт только GET {token}/tracks.
    ///
    /// Это подсказка интерфейсу, а не авторизация: remove-tracks проверяет права сам.
    /// </summary>
    public HashSet<string> RemovableTrackIds { get; set; } = new();
}
