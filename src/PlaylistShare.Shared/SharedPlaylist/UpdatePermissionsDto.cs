using PlaylistShare.Shared.Enums;
using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.SharedPlaylist;

/// <summary>Запрос на обновление прав доступа шеринг-плейлиста.</summary>
public class UpdatePermissionsDto
{
    /// <summary>Права на просмотр.</summary>
    [JsonPropertyName("viewPermission")]
    public ViewPermission ViewPermission { get; set; }

    /// <summary>Права на воспроизведение треков.</summary>
    [JsonPropertyName("playPermission")]
    public ViewPermission PlayPermission { get; set; }

    /// <summary>Права на добавление треков.</summary>
    [JsonPropertyName("addPermission")]
    public EditPermission AddPermission { get; set; }

    /// <summary>Права на удаление треков.</summary>
    [JsonPropertyName("removePermission")]
    public EditPermission RemovePermission { get; set; }
}