using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.SharedPlaylist;

public class UpdateTrackListRequest
{
    [JsonPropertyName("trackIds")]
    public List<string> TrackIds { get; set; } = new();
}