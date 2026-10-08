using Flare.Abstractions.Tokens;

namespace PlaylistShare.Pwa.Theming;

/// <summary>
/// Набор цветовых ролей Flare ColorScheme для сервиса PlaylistShare.
/// Значения переведены из палитры дизайн-макета (Material 3 / Tailwind-config в code.html).
/// Ключевые акценты: #d0bcff (primary lavender), #4cd7f6 (secondary cyan),
/// #ffb0cd (tertiary pink), глубокий обсидиановый #131318 (surface).
/// </summary>
internal static class DekaColors
{
    public static readonly ColorScheme Light = BuildLight();
    public static readonly ColorScheme Dark = BuildDark();

    private static ColorScheme BuildDark()
    {
        // ── Primary (фирменный светло-лавандовый #d0bcff) ─────────────────
        const string primary = "oklch(0.82 0.110 293)"; // #d0bcff
        const string onPrimary = "oklch(0.32 0.200 295)"; // #3c0091
        const string primaryContainer = "oklch(0.62 0.180 292)"; // #a078ff
        const string onPrimaryContainer = "oklch(0.28 0.180 295)"; // #340080

        // ── Secondary (бирюзовый стриминг #4cd7f6) ────────────────────────
        const string secondary = "oklch(0.82 0.130 210)"; // #4cd7f6
        const string onSecondary = "oklch(0.30 0.045 210)"; // #003640
        const string secondaryContainer = "oklch(0.70 0.130 210)"; // #03b5d3
        const string onSecondaryContainer = "oklch(0.35 0.050 210)"; // #00424e

        // ── Tertiary (розовый #ffb0cd) ────────────────────────────────────
        const string tertiary = "oklch(0.82 0.100 355)"; // #ffb0cd
        const string onTertiary = "oklch(0.30 0.130 355)"; // #640039
        const string tertiaryContainer = "oklch(0.65 0.220 355)"; // #f751a1
        const string onTertiaryContainer = "oklch(0.28 0.130 355)"; // #570032

        // ── Error (#ffb4ab / #93000a) ─────────────────────────────────────
        const string error = "oklch(0.82 0.110 30)";  // #ffb4ab
        const string onError = "oklch(0.28 0.130 25)";  // #690005
        const string errorContainer = "oklch(0.38 0.160 25)";  // #93000a
        const string onErrorContainer = "oklch(0.92 0.045 25)";  // #ffdad6

        // ── Success / Warning — в макете не заданы, выведены в тон палитры ─
        const string success = "oklch(0.75 0.150 155)";
        const string onSuccess = "oklch(0.22 0.060 155)";
        const string successContainer = "oklch(0.50 0.130 155)";
        const string onSuccessContainer = "oklch(0.92 0.060 155)";

        const string warning = "oklch(0.80 0.150 80)";
        const string onWarning = "oklch(0.25 0.050 80)";
        const string warningContainer = "oklch(0.55 0.140 80)";
        const string onWarningContainer = "oklch(0.93 0.060 80)";

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

            // ── Поверхности Dark Obsidian ────────────────────────────────
            Background = "oklch(0.16 0.008 286)", // #0e0e13 (surface-container-lowest)
            OnBackground = "oklch(0.92 0.008 293)", // #e4e1e9
            Surface = "oklch(0.18 0.008 286)", // #131318
            OnSurface = "oklch(0.92 0.008 293)", // #e4e1e9
            SurfaceVariant = "oklch(0.33 0.011 286)", // #35343a
            OnSurfaceVariant = "oklch(0.82 0.020 293)", // #cbc3d7
            OnSurfaceVariant2 = "oklch(0.65 0.022 293)", // #958ea0

            // ── Ступенчатые контейнеры поверхностей ─────────────────────
            SurfaceContainerLowest = "oklch(0.16 0.008 286)", // #0e0e13
            SurfaceContainerLow = "oklch(0.21 0.008 286)", // #1b1b20
            SurfaceContainer = "oklch(0.24 0.009 286)", // #1f1f25
            SurfaceContainerHigh = "oklch(0.28 0.010 286)", // #2a292f
            SurfaceContainerHighest = "oklch(0.33 0.011 286)", // #35343a

            // ── Flare 0.38+: яркость/тусклость поверхности ──────────────
            SurfaceBright = "oklch(0.33 0.011 286)", // #35343a
            SurfaceDim = "oklch(0.18 0.008 286)",    // #131318

            // ── Flare 0.42: фиксированные роли (один цвет в обеих схемах) ─
            PrimaryFixed = "oklch(0.82 0.110 293)",  // #d0bcff
            PrimaryFixedDim = "oklch(0.72 0.140 293)",
            OnPrimaryFixed = "oklch(0.28 0.150 293)",
            OnPrimaryFixedVariant = "oklch(0.48 0.200 293)",
            SecondaryFixed = "oklch(0.82 0.130 210)", // #4cd7f6
            SecondaryFixedDim = "oklch(0.72 0.140 210)",
            OnSecondaryFixed = "oklch(0.24 0.050 210)",
            OnSecondaryFixedVariant = "oklch(0.42 0.070 210)",
            TertiaryFixed = "oklch(0.82 0.100 355)", // #ffb0cd
            TertiaryFixedDim = "oklch(0.72 0.140 355)",
            OnTertiaryFixed = "oklch(0.28 0.130 355)",
            OnTertiaryFixedVariant = "oklch(0.45 0.160 355)",

            // ── Обводки ─────────────────────────────────────────────────
            Outline = "oklch(0.65 0.022 293)",  // #958ea0
            OutlineVariant = "oklch(0.38 0.025 293)",  // #494454

            // ── Инверсивные ─────────────────────────────────────────────
            InverseSurface = "oklch(0.92 0.008 293)", // #e4e1e9
            InverseOnSurface = "oklch(0.30 0.008 286)", // #303036
            InversePrimary = "oklch(0.50 0.220 293)", // #6d3bd7

            // ── Тени ─────────────────────────────────────────────────────
            Scrim = "oklch(0 0 0 / 0.65)",
            Shadow = "oklch(0 0 0)",
            ShadowUmbra = "rgba(0, 0, 0, 0.50)",
            ShadowPenumbra = "rgba(0, 0, 0, 0.30)",
        };
    }

    private static ColorScheme BuildLight()
    {
        // Light-вариант в макете отсутствует — собран инверсией dark
        // с сохранением тех же оттенков-акцентов (H=293/210/355/25).

        const string primary = "oklch(0.50 0.220 293)"; // #6d3bd7
        const string onPrimary = "oklch(0.99 0 0)";
        const string primaryContainer = "oklch(0.91 0.055 293)"; // #e9ddff
        const string onPrimaryContainer = "oklch(0.24 0.150 293)"; // #23005c

        const string secondary = "oklch(0.50 0.100 210)"; // ~#006876
        const string onSecondary = "oklch(0.99 0 0)";
        const string secondaryContainer = "oklch(0.90 0.090 210)"; // #acedff
        const string onSecondaryContainer = "oklch(0.22 0.035 210)"; // #001f26

        const string tertiary = "oklch(0.48 0.200 355)"; // ~#b4005f
        const string onTertiary = "oklch(0.99 0 0)";
        const string tertiaryContainer = "oklch(0.92 0.045 355)"; // #ffd9e4
        const string onTertiaryContainer = "oklch(0.25 0.140 355)"; // #3e0022

        const string error = "oklch(0.55 0.200 25)";  // #ba1a1a
        const string onError = "oklch(0.99 0 0)";
        const string errorContainer = "oklch(0.92 0.045 25)";  // #ffdad6
        const string onErrorContainer = "oklch(0.24 0.130 25)";  // #410002

        const string success = "oklch(0.50 0.150 155)";
        const string onSuccess = "oklch(0.99 0 0)";
        const string successContainer = "oklch(0.93 0.050 155)";
        const string onSuccessContainer = "oklch(0.22 0.080 155)";

        const string warning = "oklch(0.62 0.150 80)";
        const string onWarning = "oklch(0.99 0 0)";
        const string warningContainer = "oklch(0.94 0.055 80)";
        const string onWarningContainer = "oklch(0.28 0.090 80)";

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

            // ── Поверхности Light ────────────────────────────────────────
            Background = "oklch(0.98 0.005 293)",
            OnBackground = "oklch(0.20 0.010 286)",
            Surface = "oklch(1.00 0 0)",
            OnSurface = "oklch(0.20 0.010 286)",
            SurfaceVariant = "oklch(0.92 0.012 293)",
            OnSurfaceVariant = "oklch(0.38 0.025 293)",
            OnSurfaceVariant2 = "oklch(0.55 0.018 293)",

            // TODO: добавлен токен SurfaceContainerLowest (Flare 0.38+)
            SurfaceContainerLowest = "oklch(1.00 0 0)",
            SurfaceContainerLow = "oklch(0.97 0.006 293)",
            SurfaceContainer = "oklch(0.94 0.008 293)",
            SurfaceContainerHigh = "oklch(0.92 0.010 293)",
            SurfaceContainerHighest = "oklch(0.90 0.012 293)",

            SurfaceBright = "oklch(1.00 0 0)",
            SurfaceDim = "oklch(0.90 0.012 293)",

            // Flare 0.42: фиксированные роли (один цвет в обеих схемах)
            PrimaryFixed = "oklch(0.82 0.110 293)",  // #d0bcff
            PrimaryFixedDim = "oklch(0.72 0.140 293)",
            OnPrimaryFixed = "oklch(0.28 0.150 293)",
            OnPrimaryFixedVariant = "oklch(0.48 0.200 293)",
            SecondaryFixed = "oklch(0.82 0.130 210)", // #4cd7f6
            SecondaryFixedDim = "oklch(0.72 0.140 210)",
            OnSecondaryFixed = "oklch(0.24 0.050 210)",
            OnSecondaryFixedVariant = "oklch(0.42 0.070 210)",
            TertiaryFixed = "oklch(0.82 0.100 355)", // #ffb0cd
            TertiaryFixedDim = "oklch(0.72 0.140 355)",
            OnTertiaryFixed = "oklch(0.28 0.130 355)",
            OnTertiaryFixedVariant = "oklch(0.45 0.160 355)",

            Outline = "oklch(0.55 0.020 293)",
            OutlineVariant = "oklch(0.82 0.020 293)",

            InverseSurface = "oklch(0.20 0.010 286)",
            InverseOnSurface = "oklch(0.98 0.005 293)",
            InversePrimary = "oklch(0.82 0.110 293)", // #d0bcff

            Scrim = "oklch(0 0 0 / 0.40)",
            Shadow = "oklch(0 0 0)",
            ShadowUmbra = "rgba(45, 30, 65, 0.18)",
            ShadowPenumbra = "rgba(45, 30, 65, 0.09)",
        };
    }
}