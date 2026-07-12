using Flare.Abstractions.Tokens;

namespace PlaylistShare.Pwa.Theming;

/// <summary>The single color palette Deka ships - no alternate seeds, this app has one brand look.</summary>
internal static class DekaPalette
{
    public const string Id = "deka";

    public static readonly Palette Instance = new()
    {
        Id = Id,
        Name = "Deka",
        Source = "Deka",
        Light = DekaColors.Light,
        Dark = DekaColors.Dark,
    };
}
