using Flare.Abstractions;
using Flare.Abstractions.Tokens;

namespace PlaylistShare.Pwa.Theming;

/// <summary>
/// PlaylistShare's own Flare theme ("Deka"), built entirely from the Claude Design mock
/// "Deka Playlist Share.dc.html" - its own violet OKLCH palette, its own rounder shape scale, its own
/// type scale (Onest), its own elevation/motion/state tokens. This does not derive from Material
/// Design 3 or any other Flare theme package (PlaylistShare.Pwa has no PackageReference to any
/// Flare.Theme.* package) - it is the only theme PlaylistShare registers.
/// </summary>
public sealed class DekaTheme : ITheme
{
    /// <summary>The stable theme id - use this constant to switch themes without a magic string.</summary>
    public const string ThemeId = "deka";

    /// <inheritdoc />
    public string Id => ThemeId;

    /// <inheritdoc />
    public string DisplayName => "Deka";

    /// <inheritdoc />
    public DesignTokens Design => DekaTokens.Design;

    /// <inheritdoc />
    public string DefaultPaletteId => DekaPalette.Id;

    /// <inheritdoc />
    public IReadOnlyList<Palette> Palettes => [DekaPalette.Instance];

    /// <inheritdoc />
    public IPaletteGenerator? PaletteGenerator => null;

    /// <inheritdoc />
    public IReadOnlyList<string> StyleAssets =>
    [
        "https://fonts.googleapis.com/css2?family=Onest:wght@400;500;600;700;800&display=swap",
    ];

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string>? ExtendedDarkOverride => new Dictionary<string, string>
    {
        ["--deka-accent-strong"] = "oklch(0.64 0.19 300)",
        ["--deka-glass"] = "oklch(0.2 0.02 300 / 0.72)",
    };
}
