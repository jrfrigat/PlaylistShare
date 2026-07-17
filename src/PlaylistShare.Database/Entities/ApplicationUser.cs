using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PlaylistShare.Database.Entities;

/// <summary>Пользователь приложения (расширяет IdentityUser).</summary>
[Index(nameof(RefreshToken))]
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>Идентификатор пользователя в Яндексе (если привязан).</summary>
    public string? YandexId { get; set; }

    /// <summary>Access токен Яндекс.Музыки (зашифрованный).</summary>
    public string? YandexAccessToken { get; set; }

    /// <summary>Refresh токен Яндекс.Музыки (зашифрованный).</summary>
    public string? YandexRefreshToken { get; set; }

    /// <summary>Время истечения access токена Яндекса.</summary>
    public DateTime YandexTokenExpiryUtc { get; set; }

    /// <summary>Refresh токен для JWT (хранится в БД).</summary>
    public string? RefreshToken { get; set; }

    /// <summary>Время истечения refresh токена JWT.</summary>
    public DateTime RefreshTokenExpiryUtc { get; set; }

    /// <summary>Плейлисты, созданные пользователем.</summary>
    public ICollection<SharedPlaylist> OwnedPlaylists { get; set; } = new List<SharedPlaylist>();

    /// <summary>Избранные плейлисты.</summary>
    public ICollection<FavoritePlaylist> FavoritePlaylists { get; set; } = new List<FavoritePlaylist>();
}
