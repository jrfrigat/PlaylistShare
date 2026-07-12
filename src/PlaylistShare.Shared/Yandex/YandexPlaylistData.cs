namespace PlaylistShare.Shared.Yandex;

public class YandexPlaylistData
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public List<YandexTrack> Tracks { get; set; } = new();
}
