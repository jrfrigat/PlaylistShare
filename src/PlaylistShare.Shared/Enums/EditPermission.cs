using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Enums;

/// <summary>Кто может выполнять действие (добавление/удаление).</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EditPermission
{
    /// <summary>Все, включая неавторизованных (но для выполнения действия нужна авторизация, так как API требует токен).</summary>
    Everyone,

    /// <summary>Только авторизованные пользователи.</summary>
    AuthorizedOnly,

    /// <summary>Никто, кроме создателя.</summary>
    Nobody,

    /// <summary>Только тот пользователь, который добавил трек (актуально для удаления).</summary>
    AddedByUserOnly,
}
