using PlaylistShare.Shared.Enums;
using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.SharedPlaylist;

/// <summary>Запрос на создание нового шеринг-плейлиста.</summary>
public class SharePlaylistDto
{
    /// <summary>Идентификатор плейлиста в Яндекс.Музыке (guid).</summary>
    [JsonPropertyName("yandexPlaylistId")]
    public string YandexPlaylistUuid { get; set; } = null!;

    /// <summary>Идентификатор плейлиста в Яндекс.Музыке (kind).</summary>
    [JsonPropertyName("yandexPlaylistKind")]
    public string YandexPlaylistKind { get; set; } = null!;

    /// <summary>Идентификатор владельца плейлиста в Яндекс.Музыке (uid).</summary>
    [JsonPropertyName("yandexPlaylistOwnerUid")]
    public string YandexPlaylistOwnerUid { get; set; } = null!;

    /// <summary>Название плейлиста.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    /// <summary>Описание плейлиста.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Ссылка на обложку.</summary>
    [JsonPropertyName("coverUrl")]
    public string? CoverUrl { get; set; }

    /// <summary>Дата создания плейлиста.</summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>Токен для расшаривания плейлиста.</summary>
    [JsonPropertyName("shareToken")]
    public string ShareToken { get; set; } = string.Empty;

    /// <summary>Права на просмотр.</summary>
    [JsonPropertyName("viewPermission")]
    public ViewPermission ViewPermission { get; set; }

    /// <summary>Права на воспроизведение.</summary>
    [JsonPropertyName("playPermission")]
    public ViewPermission PlayPermission { get; set; }

    /// <summary>Права на добавление треков.</summary>
    [JsonPropertyName("addPermission")]
    public EditPermission AddPermission { get; set; }

    /// <summary>Права на удаление треков.</summary>
    [JsonPropertyName("removePermission")]
    public EditPermission RemovePermission { get; set; }
}