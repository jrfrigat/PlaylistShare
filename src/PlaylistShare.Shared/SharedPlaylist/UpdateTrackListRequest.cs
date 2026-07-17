using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.SharedPlaylist;

/// <summary>Запрос на добавление или удаление треков в шеринг-плейлисте.</summary>
public class UpdateTrackListRequest
{
    /// <summary>Идентификаторы треков в Яндекс.Музыке.</summary>
    // MinLength(1): пустой список ничего не меняет, но контроллер всё равно ходит в Яндекс
    // и отвечает "Треки добавлены" - такой запрос честнее отбить на биндинге.
    // Длины самих идентификаторов схема не ограничивает (TrackId в логах это nvarchar(max)).
    [Required]
    [MinLength(1)]
    [JsonPropertyName("trackIds")]
    public List<string> TrackIds { get; set; } = new();
}