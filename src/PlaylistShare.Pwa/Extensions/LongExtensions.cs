namespace PlaylistShare.Pwa.Extensions;

public static class LongExtensions
{
    /// <summary>
    /// Преобразует миллисекунды в формат Минуты:Секунды
    /// </summary>
    public static string FormatDuration(this long ms, FormatDurationType format = FormatDurationType.mmss)
    {
        var seconds = ms / 1000;

        var mm = seconds / 60;
        var ss = seconds % 60;

        if (format == FormatDurationType.mmss || mm < 60)
        {
            return $"{mm}:{ss:D2}";
        }
        else if (format == FormatDurationType.hhmmss)
        {
            var hh = mm / 60;
            mm = mm / 60;

            return $"{hh}:{mm:D2}:{ss:D2}";
        }
        else
        {
            return $"{mm}:{ss:D2}";
        }
    }

    public enum FormatDurationType
    {
        mmss,
        hhmmss,
    }
}