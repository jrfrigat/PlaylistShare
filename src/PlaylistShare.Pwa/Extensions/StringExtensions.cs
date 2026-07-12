namespace PlaylistShare.Pwa.Extensions;

public static class StringExtensions
{
    /// <summary>Максимальный размер обложки, который запрашиваем у Яндекс.Музыки.</summary>
    private const int YandexMaxCoverSize = 200;

    /// <summary>
    /// Допустимые размеры аватарок Яндекс.Музыки (get-music-content). Обложки отдаются ТОЛЬКО в этих
    /// дискретных размерах - произвольные (напр. 90x90, 108x108) возвращают 404 и картинка не грузится.
    /// </summary>
    private static readonly int[] YandexCoverSizes = { 30, 50, 75, 100, 150, 200 };

    /// <summary>
    /// Преобразует шаблон URL обложки Яндекс.Музыки в полный URL с указанным размером. Запрошенный
    /// (экранный) размер округляется ВВЕРХ до ближайшего валидного размера Яндекса и ограничивается
    /// <see cref="YandexMaxCoverSize"/>. До нужного экранного размера обложка "растягивается" уже на
    /// клиенте контейнером фиксированного размера (object-fit / background-size: cover).
    /// </summary>
    /// <param name="coverUri">Шаблон URL (например, "avatars.yandex.net/get-music-content/.../%%")</param>
    /// <param name="width">Желаемая ширина обложки (по умолчанию 200)</param>
    /// <param name="height">Желаемая высота обложки (по умолчанию 200)</param>
    /// <returns>Полный URL обложки или пустую строку, если входная строка null или пуста.</returns>
    public static string FormatCoverUrl(this string? coverUri, int width = 200, int height = 200)
    {
        if (string.IsNullOrEmpty(coverUri))
            return string.Empty;

        return "https://" + coverUri.Replace("%%", $"{ResolveCoverSize(width)}x{ResolveCoverSize(height)}");
    }

    /// <summary>Округляет запрошенный размер вверх до ближайшего валидного размера обложки Яндекса (макс. 200).</summary>
    private static int ResolveCoverSize(int size)
    {
        foreach (var valid in YandexCoverSizes)
            if (size <= valid)
                return valid;

        return YandexMaxCoverSize;
    }
}
