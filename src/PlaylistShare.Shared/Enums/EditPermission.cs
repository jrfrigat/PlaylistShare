using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.Enums;

/// <summary>Кто может выполнять действие (добавление/удаление).</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EditPermission
{
    /// <summary>
    /// Все, включая неавторизованных: эндпоинты правки намеренно без [Authorize], иначе это значение
    /// ничем не отличалось бы от <see cref="AuthorizedOnly"/>. Раньше здесь было написано, что
    /// авторизация всё равно нужна - это неправда, и из-за неё дыра долго выглядела задумкой.
    /// Аноним всё равно обязан иметь право на просмотр: правка его требует.
    /// </summary>
    Everyone,

    /// <summary>Только авторизованные пользователи.</summary>
    AuthorizedOnly,

    /// <summary>Никто, кроме создателя.</summary>
    Nobody,

    /// <summary>Только тот пользователь, который добавил трек (актуально для удаления).</summary>
    AddedByUserOnly,
}
