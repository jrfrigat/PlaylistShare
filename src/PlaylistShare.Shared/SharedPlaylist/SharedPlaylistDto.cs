using PlaylistShare.Shared.Auth;
using PlaylistShare.Shared.Enums;
using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.SharedPlaylist;

/// <summary>DTO шеринг-плейлиста (без навигационных свойств).</summary>
public class SharedPlaylistDto
{
    /// <summary>Уникальный идентификатор записи.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Идентификатор пользователя-создателя (владельца).</summary>
    [JsonPropertyName("creatorUserId")]
    public Guid CreatorUserId { get; set; }

    /// <summary>Uuid на яндекс плейлист</summary>
    [JsonPropertyName("yandexPlaylistUuid")]
    public string? YandexPlaylistUuid { get; set; }

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

    /// <summary>URL обложки плейлиста.</summary>
    [JsonPropertyName("coverUrl")]
    public string? CoverUrl { get; set; }

    /// <summary>Дата создания записи.</summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>Дата последнего обновления.</summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>Признак мягкого удаления.</summary>
    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }

    /// <summary>Уникальный токен для публичной ссылки.</summary>
    [JsonPropertyName("shareToken")]
    public string ShareToken { get; set; } = null!;

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

    /// <summary>Информация о создателе (опционально, подгружается отдельно).</summary>
    [JsonPropertyName("creator")]
    public ApplicationUserDto? Creator { get; set; }
}