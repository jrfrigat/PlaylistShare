using PlaylistShare.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlaylistShare.Shared.SharedPlaylist;

/// <summary>Запрос на обновление прав доступа шеринг-плейлиста.</summary>
// EnumDataType на каждом поле не для красоты: JsonStringEnumConverter принимает не только
// имена вариантов, но и числа, поэтому {"viewPermission": 99} без проверки замапится
// в несуществующее значение enum и уедет в колонку int как 99.
public class UpdatePermissionsDto
{
    /// <summary>Права на просмотр.</summary>
    [EnumDataType(typeof(ViewPermission))]
    [JsonPropertyName("viewPermission")]
    public ViewPermission ViewPermission { get; set; }

    /// <summary>Права на воспроизведение треков.</summary>
    [EnumDataType(typeof(ViewPermission))]
    [JsonPropertyName("playPermission")]
    public ViewPermission PlayPermission { get; set; }

    /// <summary>Права на добавление треков.</summary>
    [EnumDataType(typeof(EditPermission))]
    [JsonPropertyName("addPermission")]
    public EditPermission AddPermission { get; set; }

    /// <summary>Права на удаление треков.</summary>
    [EnumDataType(typeof(EditPermission))]
    [JsonPropertyName("removePermission")]
    public EditPermission RemovePermission { get; set; }
}