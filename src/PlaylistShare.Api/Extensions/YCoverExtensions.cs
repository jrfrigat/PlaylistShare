using YandexMusic.Models;

namespace PlaylistShare.Api.Extensions;

/// <summary>
/// Helpers for turning a <see cref="Cover"/> from the YandexMusic client into a single URL string.
/// Yandex returns cover URIs with a trailing <c>%%</c> size placeholder; they are returned verbatim,
/// exactly as the previous client did, so the front-end keeps resolving the size it needs.
/// </summary>
public static class YCoverExtensions
{
    /// <summary>Returns a usable cover URL, or an empty string when the cover has none.</summary>
    public static string GetUrl(this Cover? cover)
    {
        if (cover is null)
            return string.Empty;

        if (!string.IsNullOrEmpty(cover.Uri))
            return cover.Uri;

        if (cover.ItemsUri is { Count: > 0 })
            return cover.ItemsUri[0];

        return string.Empty;
    }
}
