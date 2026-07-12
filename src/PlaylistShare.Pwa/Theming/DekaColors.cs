using Flare.Abstractions.Tokens;

namespace PlaylistShare.Pwa.Theming;

/// <summary>
/// Deka's full color role set, built entirely from the Claude Design mock's OKLCH tokens - no base
/// theme (Material Design 3 or otherwise) is referenced. The mock only defines ~15 roles (background,
/// surfaces, accent, outline, text); the remaining semantic roles Flare's <see cref="ColorScheme"/>
/// requires (Secondary/Tertiary/Error/Success/Warning, their containers, Scrim, Inverse*) are original
/// values chosen to sit harmoniously alongside the mock's violet (hue 300) palette - Tertiary borrows the
/// warm hue (20) the mock itself uses for its avatar-initial gradient.
/// </summary>
internal static class DekaColors
{
    public static readonly ColorScheme Light = BuildLight();
    public static readonly ColorScheme Dark = BuildDark();

    private static ColorScheme BuildLight()
    {
        const string primary = "oklch(0.56 0.19 300)";
        const string onPrimary = "oklch(0.99 0.004 300)";
        const string primaryContainer = "oklch(0.56 0.19 300 / 0.12)";

        return new ColorScheme
        {
            Primary = primary,
            OnPrimary = onPrimary,
            PrimaryContainer = primaryContainer,
            OnPrimaryContainer = primary,

            Secondary = "oklch(0.5 0.05 300)",
            OnSecondary = onPrimary,
            SecondaryContainer = "oklch(0.9 0.03 300)",
            OnSecondaryContainer = "oklch(0.3 0.04 300)",

            Tertiary = "oklch(0.55 0.15 20)",
            OnTertiary = "oklch(0.99 0.005 20)",
            TertiaryContainer = "oklch(0.92 0.05 20)",
            OnTertiaryContainer = "oklch(0.3 0.1 20)",

            Error = "oklch(0.5 0.2 25)",
            OnError = "oklch(0.99 0.005 25)",
            ErrorContainer = "oklch(0.92 0.06 25)",
            OnErrorContainer = "oklch(0.3 0.15 25)",

            Success = "oklch(0.5 0.15 145)",
            OnSuccess = "oklch(0.99 0.005 145)",
            SuccessContainer = "oklch(0.92 0.06 145)",
            OnSuccessContainer = "oklch(0.3 0.1 145)",

            Warning = "oklch(0.6 0.16 80)",
            OnWarning = "oklch(0.16 0.02 80)",
            WarningContainer = "oklch(0.92 0.08 80)",
            OnWarningContainer = "oklch(0.3 0.1 80)",

            // The mock uses its accent color for informational icons/hints - no distinct "info" hue.
            Info = primary,
            OnInfo = onPrimary,
            InfoContainer = primaryContainer,
            OnInfoContainer = primary,

            Background = "oklch(0.975 0.005 300)",
            OnBackground = "oklch(0.23 0.02 300)",
            Surface = "oklch(1 0 0)",
            OnSurface = "oklch(0.23 0.02 300)",
            SurfaceVariant = "oklch(0.965 0.008 300)",
            OnSurfaceVariant = "oklch(0.5 0.02 300)",
            // The mock's dimmer "faint" text tone (owner names, counts, footnotes) - a third step below
            // OnSurfaceVariant. Flare 0.1.2 gives this a first-class role, replacing the old --deka-faint var.
            OnSurfaceVariant2 = "oklch(0.62 0.02 300)",
            SurfaceContainer = "oklch(0.955 0.008 300)",
            SurfaceContainerLow = "oklch(0.955 0.008 300)",
            SurfaceContainerHigh = "oklch(0.965 0.008 300)",
            SurfaceContainerHighest = "oklch(0.95 0.01 300)",

            Outline = "oklch(0.3 0.03 300 / 0.12)",
            OutlineVariant = "oklch(0.3 0.03 300 / 0.06)",

            InverseSurface = "oklch(0.23 0.02 300)",
            InverseOnSurface = "oklch(0.975 0.005 300)",
            InversePrimary = "oklch(0.7 0.16 300)",

            Scrim = "oklch(0 0 0)",
            Shadow = "oklch(0 0 0)",
            ShadowUmbra = "rgba(60,40,90,0.28)",
            ShadowPenumbra = "rgba(60,40,90,0.14)",
        };
    }

    private static ColorScheme BuildDark()
    {
        const string primary = "oklch(0.7 0.16 300)";
        const string onPrimary = "oklch(0.16 0.03 300)";
        const string primaryContainer = "oklch(0.7 0.16 300 / 0.16)";

        return new ColorScheme
        {
            Primary = primary,
            OnPrimary = onPrimary,
            PrimaryContainer = primaryContainer,
            OnPrimaryContainer = primary,

            Secondary = "oklch(0.75 0.06 300)",
            OnSecondary = onPrimary,
            SecondaryContainer = "oklch(0.3 0.03 300)",
            OnSecondaryContainer = "oklch(0.85 0.03 300)",

            Tertiary = "oklch(0.7 0.15 20)",
            OnTertiary = "oklch(0.16 0.03 20)",
            TertiaryContainer = "oklch(0.3 0.05 20)",
            OnTertiaryContainer = "oklch(0.85 0.05 20)",

            Error = "oklch(0.7 0.19 25)",
            OnError = "oklch(0.16 0.02 25)",
            ErrorContainer = "oklch(0.3 0.08 25)",
            OnErrorContainer = "oklch(0.85 0.1 25)",

            Success = "oklch(0.7 0.15 145)",
            OnSuccess = "oklch(0.16 0.02 145)",
            SuccessContainer = "oklch(0.3 0.08 145)",
            OnSuccessContainer = "oklch(0.85 0.1 145)",

            Warning = "oklch(0.75 0.15 80)",
            OnWarning = "oklch(0.16 0.02 80)",
            WarningContainer = "oklch(0.3 0.08 80)",
            OnWarningContainer = "oklch(0.85 0.1 80)",

            Info = primary,
            OnInfo = onPrimary,
            InfoContainer = primaryContainer,
            OnInfoContainer = primary,

            Background = "oklch(0.16 0.014 300)",
            OnBackground = "oklch(0.97 0.004 300)",
            Surface = "oklch(0.235 0.018 300)",
            OnSurface = "oklch(0.97 0.004 300)",
            SurfaceVariant = "oklch(0.28 0.02 300)",
            OnSurfaceVariant = "oklch(0.68 0.012 300)",
            OnSurfaceVariant2 = "oklch(0.56 0.012 300)",
            SurfaceContainer = "oklch(0.2 0.016 300)",
            SurfaceContainerLow = "oklch(0.2 0.016 300)",
            SurfaceContainerHigh = "oklch(0.28 0.02 300)",
            SurfaceContainerHighest = "oklch(0.3 0.02 300)",

            Outline = "oklch(1 0 0 / 0.09)",
            OutlineVariant = "oklch(1 0 0 / 0.05)",

            InverseSurface = "oklch(0.97 0.004 300)",
            InverseOnSurface = "oklch(0.16 0.014 300)",
            InversePrimary = "oklch(0.56 0.19 300)",

            Scrim = "oklch(0 0 0)",
            Shadow = "oklch(0 0 0)",
            ShadowUmbra = "rgba(0,0,0,0.55)",
            ShadowPenumbra = "rgba(0,0,0,0.3)",
        };
    }
}
