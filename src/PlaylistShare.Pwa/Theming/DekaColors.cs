using Flare.Abstractions.Tokens;

namespace PlaylistShare.Pwa.Theming;

/// <summary>
/// Набор цветовых ролей Flare ColorScheme для сервиса PlaylistShare,
/// в точности переведенный из оригинальной палитры Obsidian Wave в формат OKLCH.
/// Сохраняет аутентичные оттенки макетов (глубокий обсидиан, #8b5cf6 неоновый фиолетовый, бирюзовый стриминг).
/// </summary>
internal static class DekaColors
{
    public static readonly ColorScheme Light = BuildLight();
    public static readonly ColorScheme Dark = BuildDark();

    private static ColorScheme BuildDark()
    {
        // Константы первичного акцента (оригинальный фиолетовый #8b5cf6)
        const string primary = "oklch(0.585 0.233 293)";
        const string onPrimary = "oklch(0.985 0 0)";
        const string primaryContainer = "oklch(0.585 0.233 293 / 0.16)";
        const string onPrimaryContainer = "oklch(0.880 0.090 293)";

        // Вторичный акцент (бирюзовый #06b6d4)
        const string secondary = "oklch(0.715 0.143 215)";
        const string onSecondary = "oklch(0.140 0.040 215)";
        const string secondaryContainer = "oklch(0.715 0.143 215 / 0.16)";
        const string onSecondaryContainer = "oklch(0.900 0.060 215)";

        // Третичный акцент (теплый янтарно-коралловый)
        const string tertiary = "oklch(0.720 0.160 38)";
        const string onTertiary = "oklch(0.150 0.040 38)";
        const string tertiaryContainer = "oklch(0.720 0.160 38 / 0.16)";
        const string onTertiaryContainer = "oklch(0.910 0.060 38)";

        // Статусные семантические роли
        const string error = "oklch(0.660 0.210 25)";
        const string onError = "oklch(0.140 0.030 25)";
        const string errorContainer = "oklch(0.660 0.210 25 / 0.16)";
        const string onErrorContainer = "oklch(0.910 0.060 25)";

        const string success = "oklch(0.720 0.160 150)";
        const string onSuccess = "oklch(0.140 0.030 150)";
        const string successContainer = "oklch(0.720 0.160 150 / 0.16)";
        const string onSuccessContainer = "oklch(0.910 0.060 150)";

        const string warning = "oklch(0.780 0.150 80)";
        const string onWarning = "oklch(0.160 0.030 80)";
        const string warningContainer = "oklch(0.780 0.150 80 / 0.16)";
        const string onWarningContainer = "oklch(0.920 0.060 80)";

        return new ColorScheme
        {
            // Primary (фирменный фиолетовый)
            Primary = primary,
            OnPrimary = onPrimary,
            PrimaryContainer = primaryContainer,
            OnPrimaryContainer = onPrimaryContainer,

            // Secondary (Яндекс Музыка / бирюзовый стриминг)
            Secondary = secondary,
            OnSecondary = onSecondary,
            SecondaryContainer = secondaryContainer,
            OnSecondaryContainer = onSecondaryContainer,

            // Tertiary (социальные реакции, огоньки, аватары)
            Tertiary = tertiary,
            OnTertiary = onTertiary,
            TertiaryContainer = tertiaryContainer,
            OnTertiaryContainer = onTertiaryContainer,

            // Семантические роли
            Error = error,
            OnError = onError,
            ErrorContainer = errorContainer,
            OnErrorContainer = onErrorContainer,

            Success = success,
            OnSuccess = onSuccess,
            SuccessContainer = successContainer,
            OnSuccessContainer = onSuccessContainer,

            Warning = warning,
            OnWarning = onWarning,
            WarningContainer = warningContainer,
            OnWarningContainer = onWarningContainer,

            // Info (в соответствии с макетом использует primary-акцент либо вторичный бирюзовый)
            Info = primary,
            OnInfo = onPrimary,
            InfoContainer = primaryContainer,
            OnInfoContainer = primary,

            // Поверхности Dark Obsidian (в точных значениях первой дизайн-системы)
            Background = "oklch(0.160 0.009 286)",             // #0e0e13 (глубокий фон окна/сайдбара)
            OnBackground = "oklch(0.955 0.006 286)",           // #f1f0f7 (основной контрастный текст)
            Surface = "oklch(0.180 0.010 286)",                // #131318 (базовая поверхность экранов)
            OnSurface = "oklch(0.955 0.006 286)",              // #f1f0f7
            SurfaceVariant = "oklch(0.245 0.011 286)",         // #222127 (разделители, вторичные плашки)
            OnSurfaceVariant = "oklch(0.710 0.015 286)",       // #a5a4af (артисты, хронометраж)
            OnSurfaceVariant2 = "oklch(0.540 0.014 286)",      // #74737d (служебный текст, счетчики)

            // Ступенчатые контейнеры поверхностей

            //TODO: добавить токен в flare
            //SurfaceContainerLowest = "oklch(0.160 0.009 286)", // #0e0e13
            SurfaceContainerLow = "oklch(0.215 0.010 286)",    // #1b1b20 (карточки плейлистов в сетке)
            SurfaceContainer = "oklch(0.245 0.011 286)",       // #222127 (строки треков, поисковая строка)
            SurfaceContainerHigh = "oklch(0.285 0.012 286)",   // #2c2b32 (hover/active строки трека)
            SurfaceContainerHighest = "oklch(0.335 0.012 286)",// #39383e (контекстные меню, дропдауны)

            // Обводки и границы
            Outline = "oklch(0.350 0.012 286 / 0.40)",         // Контрастная граница инпутов и карточек
            OutlineVariant = "oklch(1 0 0 / 0.07)",            // Ультратонкие разделители строк списка

            // Инверсивные цвета (для подсказок, тостов)
            InverseSurface = "oklch(0.955 0.006 286)",
            InverseOnSurface = "oklch(0.180 0.010 286)",
            InversePrimary = "oklch(0.520 0.220 293)",

            // Тени и затемнения
            Scrim = "oklch(0 0 0 / 0.65)",
            Shadow = "oklch(0 0 0)",
            ShadowUmbra = "rgba(0, 0, 0, 0.50)",
            ShadowPenumbra = "rgba(0, 0, 0, 0.30)",
        };
    }

    private static ColorScheme BuildLight()
    {
        const string primary = "oklch(0.520 0.220 293)";
        const string onPrimary = "oklch(0.985 0 0)";
        const string primaryContainer = "oklch(0.935 0.060 293)";
        const string onPrimaryContainer = "oklch(0.220 0.120 293)";

        const string secondary = "oklch(0.510 0.140 215)";
        const string onSecondary = "oklch(0.985 0 0)";
        const string secondaryContainer = "oklch(0.925 0.050 215)";
        const string onSecondaryContainer = "oklch(0.190 0.070 215)";

        const string tertiary = "oklch(0.580 0.150 38)";
        const string onTertiary = "oklch(0.985 0 0)";
        const string tertiaryContainer = "oklch(0.940 0.050 38)";
        const string onTertiaryContainer = "oklch(0.240 0.080 38)";

        const string error = "oklch(0.550 0.200 25)";
        const string onError = "oklch(0.985 0 0)";
        const string errorContainer = "oklch(0.950 0.050 25)";
        const string onErrorContainer = "oklch(0.280 0.100 25)";

        const string success = "oklch(0.500 0.160 150)";
        const string onSuccess = "oklch(0.985 0 0)";
        const string successContainer = "oklch(0.930 0.060 150)";
        const string onSuccessContainer = "oklch(0.220 0.100 150)";

        const string warning = "oklch(0.600 0.150 78)";
        const string onWarning = "oklch(0.985 0 0)";
        const string warningContainer = "oklch(0.940 0.060 78)";
        const string onWarningContainer = "oklch(0.290 0.100 78)";

        return new ColorScheme
        {
            Primary = primary,
            OnPrimary = onPrimary,
            PrimaryContainer = primaryContainer,
            OnPrimaryContainer = onPrimaryContainer,

            Secondary = secondary,
            OnSecondary = onSecondary,
            SecondaryContainer = secondaryContainer,
            OnSecondaryContainer = onSecondaryContainer,

            Tertiary = tertiary,
            OnTertiary = onTertiary,
            TertiaryContainer = tertiaryContainer,
            OnTertiaryContainer = onTertiaryContainer,

            Error = error,
            OnError = onError,
            ErrorContainer = errorContainer,
            OnErrorContainer = onErrorContainer,

            Success = success,
            OnSuccess = onSuccess,
            SuccessContainer = successContainer,
            OnSuccessContainer = onSuccessContainer,

            Warning = warning,
            OnWarning = onWarning,
            WarningContainer = warningContainer,
            OnWarningContainer = onWarningContainer,

            Info = primary,
            OnInfo = onPrimary,
            InfoContainer = primaryContainer,
            OnInfoContainer = primary,

            // Поверхности Light
            Background = "oklch(0.975 0.005 286)",
            OnBackground = "oklch(0.180 0.015 286)",
            Surface = "oklch(1.000 0 0)",
            OnSurface = "oklch(0.180 0.015 286)",
            SurfaceVariant = "oklch(0.955 0.006 286)",
            OnSurfaceVariant = "oklch(0.480 0.015 286)",
            OnSurfaceVariant2 = "oklch(0.620 0.012 286)",

            //TODO: добавить токен в flare
            //SurfaceContainerLowest = "oklch(1.000 0 0)",
            SurfaceContainerLow = "oklch(0.965 0.005 286)",
            SurfaceContainer = "oklch(0.950 0.007 286)",
            SurfaceContainerHigh = "oklch(0.925 0.009 286)",
            SurfaceContainerHighest = "oklch(0.895 0.012 286)",

            Outline = "oklch(0.220 0.020 286 / 0.15)",
            OutlineVariant = "oklch(0.220 0.020 286 / 0.06)",

            InverseSurface = "oklch(0.200 0.015 286)",
            InverseOnSurface = "oklch(0.975 0.005 286)",
            InversePrimary = "oklch(0.650 0.230 293)",

            Scrim = "oklch(0 0 0 / 0.40)",
            Shadow = "oklch(0 0 0)",
            ShadowUmbra = "rgba(45, 30, 65, 0.18)",
            ShadowPenumbra = "rgba(45, 30, 65, 0.09)",
        };
    }
}