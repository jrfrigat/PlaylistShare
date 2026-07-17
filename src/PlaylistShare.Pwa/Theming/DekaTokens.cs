using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;

namespace PlaylistShare.Pwa.Theming;

/// <summary>
/// Deka's full non-color design tokens - typography, shape, elevation, motion, state, spacing and every
/// component's geometry - authored entirely from scratch from the Claude Design mock. This does NOT derive
/// from Material Design 3 or any other Flare theme package: since Flare 0.1.x the core ships zero token
/// defaults (every <see cref="DesignTokens"/> field and every component-token field is <c>required</c>),
/// so a theme must supply every value itself. This file does exactly that - the mock's own values where
/// it specifies one (buttons, cards, inputs, nav, sheets, switch, tabs, mini-player), and neutral values
/// that reference Deka's semantic <c>--flare-*</c> roles for the components the app never renders.
/// </summary>
internal static class DekaTokens
{
    private const string FontStack = "'Onest', system-ui, -apple-system, sans-serif";

    // ----- core scales -------------------------------------------------------------------------------

    // Sizes/weights measured off the mock's actual inline styles.
    private static readonly TypographyTokens Typography = new()
    {
        DisplayLarge = T("2.5rem", "800", "1.05", "-0.025em"),      // 40px - playlist/detail hero title
        DisplayMedium = T("2rem", "800", "1.1", "-0.02em"),         // 32px - "Мои плейлисты"
        DisplaySmall = T("1.75rem", "800", "1.15", "-0.02em"),      // 28px - search/settings headings
        HeadlineLarge = T("1.625rem", "800", "1.2", "-0.02em"),     // 26px - auth welcome title
        HeadlineMedium = T("1.4375rem", "800", "1.25", "-0.01em"),  // 23px - connect screen title
        HeadlineSmall = T("1.3125rem", "800", "1.3", "-0.01em"),    // 21px - section headings
        TitleLarge = T("1.1875rem", "700", "1.3", "0em"),           // 19px - share sheet title, dialog titles
        TitleMedium = T("1rem", "700", "1.4", "0em"),               // 16px - card titles
        TitleSmall = T("0.9375rem", "700", "1.4", "0em"),           // 15px - playlist/track titles
        BodyLarge = T("1rem", "500", "1.5", "0em"),                 // 16px - buttons, primary body text
        BodyMedium = T("0.9375rem", "500", "1.5", "0em"),           // 15px - secondary text
        BodySmall = T("0.875rem", "500", "1.4", "0em"),             // 14px - captions, hints
        LabelLarge = T("0.8125rem", "700", "1.3", "0.04em"),        // 13px - eyebrows, tab labels
        LabelMedium = T("0.8125rem", "600", "1.3", "0em"),          // 13px - chips, nav items
        LabelSmall = T("0.6875rem", "600", "1.3", "0.02em"),        // 11px - badges, micro-labels
    };

    // Rounder than a typical MD baseline, matching the mock's 10-26px card/button/sheet radii.
    private static readonly ShapeTokens Shape = new()
    {
        None = "0px",
        ExtraSmall = "10px",
        Small = "14px",
        Medium = "18px",
        Large = "22px",
        ExtraLarge = "26px",
        Full = "9999px",
    };

    // Soft, diffuse, large-blur shadows with a big negative spread - matching the mock's card/sheet/
    // full-player shadow style, not a typical multi-layer MD shadow.
    private static readonly ElevationTokens Elevation = new()
    {
        Level0 = "none",
        Level1 = "0 2px 8px -4px rgba(0,0,0,.35), 0 1px 3px rgba(0,0,0,.15)",
        Level2 = "0 6px 20px -14px rgba(0,0,0,.5)",
        Level3 = "0 10px 28px -12px rgba(0,0,0,.55)",
        Level4 = "0 16px 40px -12px rgba(0,0,0,.6)",
        Level5 = "0 20px 50px -12px rgba(0,0,0,.7)",
    };

    private static readonly MotionTokens Motion = new()
    {
        DurationShort1 = "100ms",
        DurationShort2 = "150ms",
        DurationShort3 = "175ms",
        DurationShort4 = "200ms",
        DurationMedium1 = "200ms",
        DurationMedium2 = "250ms",
        DurationLong1 = "300ms",
        DurationLong2 = "400ms",
        EasingStandard = "cubic-bezier(0.2, 0.8, 0.2, 1)",
        EasingDecelerate = "cubic-bezier(0, 0, 0, 1)",
        EasingAccelerate = "cubic-bezier(0.3, 0, 1, 1)",
        EasingEmphasized = "cubic-bezier(0.2, 0.8, 0.2, 1)",
    };

    // Subtler than a typical MD state-layer scale - the mock's interactions are simple scale/color-shifts.
    private static readonly StateTokens State = new()
    {
        HoverOpacity = "0.06",
        SelectedOpacity = "0.12",
        FocusOpacity = "0.10",
        PressedOpacity = "0.10",
        DraggedOpacity = "0.14",
        DisabledOpacity = "0.38",
        DisabledContainerOpacity = "0.12",
        // State-layer paint (Flare 0.2.0): translucent currentColor wash at each state's opacity (Material model).
        HoverLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        FocusLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        PressedLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-pressed-opacity) * 100%), transparent)",
        DraggedLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-dragged-opacity) * 100%), transparent)",
    };

    // 2px-base spacing ramp (0/2/4/6/8/10/12/16/20/24/32/48/64px).
    private static readonly SpacingTokens Spacing = new()
    {
        S0 = "0",
        S1 = "0.125rem",
        S2 = "0.25rem",
        S3 = "0.375rem",
        S4 = "0.5rem",
        S5 = "0.625rem",
        S6 = "0.75rem",
        S8 = "1rem",
        S10 = "1.25rem",
        S12 = "1.5rem",
        S16 = "2rem",
        S24 = "3rem",
        S32 = "4rem",
    };

    // ----- component tokens the mock actively styles -------------------------------------------------

    private static readonly ButtonTokens Button = new()
    {
        LoadingOpacity = "0.8",
        ContainerRadius = "var(--flare-shape-small)",
        TextPaddingInline = "0.75rem",
        GapXs = "0.5rem",
        GapSm = "0.5rem",
        GapMd = "0.5rem",
        GapLg = "0.5rem",
        GapXl = "0.75rem",
        HeightXs = "2rem",
        HeightSm = "2.5rem",
        HeightMd = "3rem",
        HeightLg = "3.25rem",
        HeightXl = "3.5rem",
        PaddingInlineXs = "0.875rem",
        PaddingInlineSm = "1.125rem",
        PaddingInlineMd = "1.5rem",
        PaddingInlineLg = "1.75rem",
        PaddingInlineXl = "2rem",
        // Rounded-rectangle (the mock's CTAs are ~15px radius), not a full capsule.
        RadiusXs = CornerRadiusTokens.All("var(--flare-shape-small)"),
        RadiusSm = CornerRadiusTokens.All("var(--flare-shape-small)"),
        RadiusMd = CornerRadiusTokens.All("var(--flare-shape-medium)"),
        RadiusLg = CornerRadiusTokens.All("var(--flare-shape-medium)"),
        RadiusXl = CornerRadiusTokens.All("var(--flare-shape-medium)"),
        FocusOutline = "2px solid var(--flare-color-primary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        FilledHoverShadow = "0 10px 24px -10px var(--deka-accent-strong)",
        IconSizeXs = "1.125rem",
        IconSizeSm = "1.125rem",
        IconSizeMd = "1.25rem",
        IconSizeLg = "1.375rem",
        IconSizeXl = "1.5rem",
        LabelXs = T("0.8125rem", "700", "1.1", "0em"),
        LabelSm = T("0.875rem", "700", "1.2", "0em"),
        LabelMd = T("1rem", "700", "1.3", "0em"),
        LabelLg = T("1rem", "700", "1.3", "0em"),
        LabelXl = T("1.0625rem", "700", "1.3", "0em"),
    };

    private static readonly CardTokens Card = new()
    {
        ElevatedBg = "var(--flare-color-surface)",
        FilledBg = "var(--flare-color-surface-container-low)",
        FilledBorder = "none",
        OutlinedBg = "var(--flare-color-surface)",
        OutlinedBorder = "1px solid var(--flare-color-outline-variant)",
        TonalBg = "var(--flare-color-primary-container)",
        TonalColor = "var(--flare-color-on-surface)",
        TextColor = "var(--flare-color-on-surface)",
        Radius = "var(--flare-shape-large)",
        Elevation = "0 6px 20px -14px rgba(0,0,0,.5)",
        ElevationHover = "0 10px 28px -12px rgba(0,0,0,.55)",
        SelectedBorder = "2px solid var(--flare-color-primary)",
        SelectedBg = "var(--flare-color-primary-container)",
        StateLayer = "var(--flare-color-on-surface)",
        PaddingTop = "12px",
        PaddingRight = "12px",
        PaddingBottom = "12px",
        PaddingLeft = "12px",
        ContentPadding = "12px",
        HeaderPadding = "16px 16px 0 16px",
        FooterPadding = "8px 16px 16px 16px",
        ActionsPadding = "8px 16px 16px 16px",
        ActionsGap = "8px",
        MediaRadius = "0",
        TitleColor = "var(--flare-color-on-surface)",
        TitleFontFamily = "var(--flare-typescale-title-medium-font)",
        TitleFontSize = "var(--flare-typescale-title-medium-size)",
        SubtitleColor = "var(--flare-color-on-surface-variant)",
        SubtitleFontFamily = "var(--flare-typescale-body-medium-font)",
        SubtitleFontSize = "var(--flare-typescale-body-medium-size)",
        TransitionDuration = "var(--flare-motion-duration-short2)",
        TransitionEasing = "var(--flare-motion-easing-standard)",
    };

    private static readonly InputTokens Input = new()
    {
        FilledBg = "var(--flare-color-surface-container-low)",
        OutlinedBorder = "1px solid var(--flare-color-outline)",
        OutlinedRadius = "var(--flare-shape-small)",
        FilledBorderBottom = "1px solid var(--flare-color-outline)",
        // Focus indicator (Flare 0.2.0, drawn by input.css on :focus-within as a layout-neutral box-shadow):
        // a full 1px primary ring inside the field well, matching the Deka look where the edge tints primary.
        FocusRing = "inset 0 0 0 1px var(--fc-main, var(--flare-color-primary))",
        FocusOutline = "none",
        FocusOutlineOffset = "0",
        // Deka fields are uniformly bordered - hover must NOT recolor just the bottom edge (Material
        // filled-underline behavior), so match the resting outline. Only focus tints (the FocusRing above).
        HoverBorderBottom = "1px solid var(--flare-color-outline)",
        HoverStateLayer = "transparent",
        Padding = "0.875rem 1rem",
        PlaceholderColor = "var(--flare-color-on-surface-variant)",
        DisabledBg = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)",
        DisabledIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)",
        ErrorHoverIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 8%, var(--flare-color-error))",
    };

    // Sidebar nav chips: rounded, active = primary-container fill with primary text.
    private static readonly NavTokens Nav = new()
    {
        ActiveWeight = "600",
        BadgeWeight = "700",
        RailLabelLineHeight = "1.15",
        ItemRadius = "var(--flare-shape-small)",
        IndicatorRadius = "var(--flare-shape-small)",
        ActiveIndicator = "var(--flare-color-primary-container)",
        ActiveLeftBar = "none",
    };

    // Mobile bottom nav: low-container bar, primary active tint, faint inactive.
    private static readonly BottomNavTokens BottomNav = new()
    {
        BarHeight = "4rem",
        BarBg = "var(--flare-color-surface-container-low)",
        BorderColor = "var(--flare-color-outline)",
        SafeAreaPadding = "env(safe-area-inset-bottom, 0px)",
        InactiveColor = "var(--flare-color-on-surface-variant2)",
        ActiveColor = "var(--flare-color-primary)",
        IconSize = "1.4375rem",
        LabelFontSize = "0.6875rem",
        LabelFontWeight = "600",
        LabelFontWeightActive = "700",
        IndicatorBg = "transparent",
        IndicatorRadius = "var(--flare-shape-small)",
        IndicatorSize = "0",
    };

    // Bottom sheets (share / add-to-playlist) + dialogs use the extra-large 26px radius.
    private static readonly DialogTokens Dialog = new()
    {
        Radius = "var(--flare-shape-extra-large)",
        IconSize = "1.5rem",
    };

    private static readonly DrawerTokens Drawer = new()
    {
        Width = "360px",
        MiniWidth = "72px",
    };

    private static readonly MenuTokens Menu = new()
    {
        GroupDivider = "none",
        PanelRadius = "var(--flare-shape-medium)",
        PanelMinWidth = "8rem",
        PanelShadow = "var(--flare-elevation-4)",
        PanelPaddingInline = "0.375rem",
        PanelPaddingBlock = "0.375rem",
        ItemHeight = "2.75rem",
        ItemPaddingBlock = "0.5rem",
        ItemGapBetween = "0.125rem",
        ItemRadius = "var(--flare-shape-small)",
        ItemRadiusEnd = "var(--flare-shape-small)",
        GroupRadius = "var(--flare-shape-small)",
        GroupPadding = "0.25rem",
        GroupBg = "var(--flare-color-surface-container-high)",
        GroupGap = "0.5rem",
        GroupShadow = "none",
        GroupedPanelBg = "transparent",
        GroupedPanelShadow = "none",
        ItemLabelFont = "var(--flare-typescale-label-medium-font)",
        ItemLabelWeight = "var(--flare-typescale-label-medium-weight)",
        ItemLabelSize = "var(--flare-typescale-label-medium-size)",
        ItemLabelHeight = "var(--flare-typescale-label-medium-height)",
        ItemLabelSpacing = "var(--flare-typescale-label-medium-spacing)",
        EnterAnimation = "flare-menu-in",
        PanelBg = "var(--flare-color-surface-container)",
        ItemPaddingInline = "0.875rem",
        ItemGap = "0.75rem",
        ItemIconSize = "1.25rem",
        ItemFocusRingColor = "var(--flare-color-primary)",
        ItemFocusRingThickness = "2px",
        ItemFocusRingOffset = "-2px",
    };

    private static readonly ChipTokens Chip = new()
    {
        Radius = "var(--flare-shape-medium)",
        Height = "2rem",
    };

    // Search filter tabs: pill track, active = solid accent with on-primary text.
    private static readonly TabsTokens Tabs = new()
    {
        ActiveWeight = "700",
        CloseOpacity = "0.6",
        LabelFont = "var(--flare-typescale-label-large-font)",
        LabelSize = "var(--flare-typescale-label-large-size)",
        LabelWeight = "var(--flare-typescale-label-large-weight)",
        ScrollShadowOpacity = "35%",
        IndicatorThickness = "3px",
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "var(--flare-color-on-surface-variant)",
        DividerColor = "var(--flare-color-outline-variant)",
        SelectedBg = "var(--flare-color-primary-container)",
        SelectedFg = "var(--flare-color-primary)",
        FilledBg = "var(--flare-color-primary)",
        FilledFg = "var(--flare-color-on-primary)",
        TrackBg = "transparent",
        PillRadius = "var(--flare-shape-full)",
    };

    // Collaboration toggle: accent track when on.
    private static readonly SwitchTokens Switch = new()
    {
        TrackWidth = "46px",
        TrackHeight = "27px",
        TrackOffBg = "var(--flare-color-outline)",
        TrackOnBg = "var(--fc-main, var(--flare-color-primary))",
        TrackBorder = "none",
        TrackHoverBorderColor = "transparent",
        ThumbOffSize = "1.3125rem",
        ThumbOnSize = "1.3125rem",
        ThumbPressedOffSize = "1.4375rem",
        ThumbPressedOnSize = "1.4375rem",
        ThumbOffLeft = "0.1875rem",
        ThumbOnLeft = "calc(100% - 1.5rem)",
        ThumbOffColor = "#fff",
        ThumbOnColor = "#fff",
        ThumbStateOffColor = "#fff",
        ThumbStateOnColor = "#fff",
        IconSize = "0",
        IconOffColor = "transparent",
        IconOnColor = "transparent",
        FocusOutline = "2px solid var(--flare-color-primary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        TrackHoverOffBg = "var(--flare-switch-track-off-bg)",
        TrackHoverOnBg = "var(--flare-switch-track-on-bg)",
        HoverShadowOff = "none",
        HoverShadowOn = "none",
        DisabledTrackBg = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)",
        DisabledTrackBorder = "transparent",
        DisabledHandleBg = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)",
    };

    private static readonly SnackbarTokens Snackbar = new()
    {
        Radius = "var(--flare-shape-small)",
        MinHeight = "3rem",
        PaddingBlock = "0.75rem",
        ProviderInset = "1.5rem",
        CloseOpacity = "0.75",
    };

    private static readonly AlertTokens Alert = new()
    {
        BodyOpacity = "0.9",
        CloseOpacity = "0.7",
        Radius = "var(--flare-shape-medium)",
        BorderWidth = "0",
        Padding = "0.875rem 1rem",
        Gap = "0.75rem",
    };

    private static readonly BadgeTokens Badge = new()
    {
        Radius = "var(--flare-shape-full)",
        MinWidth = "1rem",
        Height = "1rem",
        DotSize = "0.375rem",
        PaddingX = "0.25rem",
        Offset = "0.375rem",
        DotOffset = "0",
    };

    private static readonly AvatarTokens Avatar = new()
    {
        GroupSpacing = "-0.75rem",
        GroupBorderWidth = "2px",
        GroupBorderColor = "var(--flare-color-surface)",
        OverflowBg = "var(--flare-color-surface-container-highest)",
        OverflowColor = "var(--flare-color-on-surface-variant)",
    };

    private static readonly ProgressTokens Progress = new()
    {
        // Ramp по размерам (Flare 0.6.0): раньше это были одиночные LinearHeight/CircularSize/
        // CircularWidth, а размер прогресса задавался пикселями через Size="40". Md держим ровно
        // тем, что тема рисовала до 0.6.0 (4px / 40px / 4px), чтобы по умолчанию ничего не сдвинулось;
        // шкала расходится в обе стороны - мелкий спиннер в строке нужен не реже крупного.
        LinearHeightXs = "2px",
        LinearHeightSm = "3px",
        LinearHeightMd = "4px",
        LinearHeightLg = "6px",
        LinearHeightXl = "8px",
        TrackRadius = "var(--flare-shape-full)",
        Gap = "4px",
        StopSize = "4px",
        StopInset = "0px",
        StopColor = "var(--fc-main, var(--flare-color-primary))",
        BufferOpacity = "30%",
        // Xs и Sm подобраны под живые вызовы (спиннер в строке списка просил 16px, экран привязки
        // Яндекса - 24px), чтобы переход на шкалу не изменил ни один существующий размер.
        CircularSizeXs = "16px",
        CircularSizeSm = "24px",
        CircularSizeMd = "40px",
        CircularSizeLg = "48px",
        CircularSizeXl = "56px",
        // Толщина кольца растёт вместе с диаметром, иначе крупный спиннер выглядит проволочным.
        CircularWidthXs = "2px",
        CircularWidthSm = "3px",
        CircularWidthMd = "4px",
        CircularWidthLg = "5px",
        CircularWidthXl = "6px",
        CircularCap = "round",
        CircularGap = "4px",
        WavyEnabled = "0",
        WavyHeight = "10px",
        WaveLength = "40px",
        WaveAmplitude = "3px",
        WaveSpeed = "1s",
        RingWaves = "8",
        RingWaveAmplitude = "1.6",
    };

    private static readonly FabTokens Fab = new()
    {
        PaddingSm = "0.5rem",
        PaddingMd = "1rem",
        PaddingLg = "1.75rem",
        RadiusSm = "var(--flare-shape-medium)",
        RadiusMd = "var(--flare-shape-large)",
        RadiusLg = "var(--flare-shape-extra-large)",
        Gap = "0.75rem",
        Shadow = "var(--flare-elevation-3)",
        HoverShadow = "var(--flare-elevation-4)",
        AnchorOffset = "1.5rem",
    };

    private static readonly TooltipTokens Tooltip = new()
    {
        MaxWidth = "18rem",
        Offset = "0.5rem",
    };

    private static readonly PopoverTokens Popover = new()
    {
        Radius = "var(--flare-shape-medium)",
    };

    // ----- component tokens the app doesn't render: neutral, on-brand via --flare-* roles ------------

    private static readonly ButtonGroupTokens ButtonGroup = new()
    {
        Gap = "0",
        Overlap = "-1px",
        OuterRadius = "var(--flare-shape-small)",
        InnerRadius = "0",
        ZActive = "1",
    };

    private static readonly SplitButtonTokens SplitButton = new()
    {
        Gap = "0.125rem",
        TriggerWidth = "var(--_flare-btn-height, var(--flare-btn-height-md, 3rem))",
        CaretSizeXs = "var(--flare-btn-icon-size-xs)",
        CaretSizeSm = "var(--flare-btn-icon-size-sm)",
        CaretSizeMd = "var(--flare-btn-icon-size-md)",
        CaretSizeLg = "var(--flare-btn-icon-size-lg)",
        CaretSizeXl = "var(--flare-btn-icon-size-xl)",
        MainRadiusXs = new() { TopLeft = "var(--flare-btn-radius-xs-top-left)", BottomLeft = "var(--flare-btn-radius-xs-bottom-left)", TopRight = "0.25rem", BottomRight = "0.25rem" },
        MainRadiusSm = new() { TopLeft = "var(--flare-btn-radius-sm-top-left)", BottomLeft = "var(--flare-btn-radius-sm-bottom-left)", TopRight = "0.25rem", BottomRight = "0.25rem" },
        MainRadiusMd = new() { TopLeft = "var(--flare-btn-radius-md-top-left)", BottomLeft = "var(--flare-btn-radius-md-bottom-left)", TopRight = "0.25rem", BottomRight = "0.25rem" },
        MainRadiusLg = new() { TopLeft = "var(--flare-btn-radius-lg-top-left)", BottomLeft = "var(--flare-btn-radius-lg-bottom-left)", TopRight = "0.5rem", BottomRight = "0.5rem" },
        MainRadiusXl = new() { TopLeft = "var(--flare-btn-radius-xl-top-left)", BottomLeft = "var(--flare-btn-radius-xl-bottom-left)", TopRight = "0.75rem", BottomRight = "0.75rem" },
        TriggerRadiusXs = new() { TopLeft = "0.25rem", BottomLeft = "0.25rem", TopRight = "var(--flare-btn-radius-xs-top-right)", BottomRight = "var(--flare-btn-radius-xs-bottom-right)" },
        TriggerRadiusSm = new() { TopLeft = "0.25rem", BottomLeft = "0.25rem", TopRight = "var(--flare-btn-radius-sm-top-right)", BottomRight = "var(--flare-btn-radius-sm-bottom-right)" },
        TriggerRadiusMd = new() { TopLeft = "0.25rem", BottomLeft = "0.25rem", TopRight = "var(--flare-btn-radius-md-top-right)", BottomRight = "var(--flare-btn-radius-md-bottom-right)" },
        TriggerRadiusLg = new() { TopLeft = "0.5rem", BottomLeft = "0.5rem", TopRight = "var(--flare-btn-radius-lg-top-right)", BottomRight = "var(--flare-btn-radius-lg-bottom-right)" },
        TriggerRadiusXl = new() { TopLeft = "0.75rem", BottomLeft = "0.75rem", TopRight = "var(--flare-btn-radius-xl-top-right)", BottomRight = "var(--flare-btn-radius-xl-bottom-right)" },
    };

    private static readonly ToggleButtonTokens ToggleButton = new()
    {
        HeightXs = "1.75rem",
        HeightSm = "2rem",
        HeightMd = "2.5rem",
        HeightLg = "3rem",
        HeightXl = "3.5rem",
        PaddingXs = "0.5rem",
        PaddingSm = "0.75rem",
        PaddingMd = "1rem",
        PaddingLg = "1.5rem",
        PaddingXl = "2rem",
        Gap = "0.375rem",
        Radius = "var(--flare-shape-small)",
        RadiusSelectedSm = "var(--flare-shape-small)",
        RadiusSelectedMd = "var(--flare-shape-medium)",
        RadiusSelectedLg = "var(--flare-shape-medium)",
        RestBg = "var(--flare-color-surface-container-high)",
        RestColor = "var(--flare-color-on-surface-variant)",
        SelectedBg = "var(--flare-color-primary-container)",
        SelectedColor = "var(--flare-color-primary)",
        GroupBorder = "1px solid var(--flare-color-outline)",
        GroupRadius = "var(--flare-shape-small)",
        GroupRadiusVertical = "var(--flare-shape-small)",
        GroupDivider = "var(--flare-color-outline)",
    };

    private static readonly CheckboxTokens Checkbox = new()
    {
        Size = "1.125rem",
        BorderWidth = "2px",
        Radius = "5px",
        StateLayerHover = "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)",
        StateLayerHoverChecked = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
        FocusOutline = "2px solid var(--flare-color-primary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
    };

    private static readonly RadioTokens Radio = new()
    {
        Size = "1.25rem",
        StateLayerHover = "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)",
        StateLayerHoverChecked = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
    };

    private static readonly SliderTokens Slider = new()
    {
        // Силуэт - классическая полоса музыкального плеера, а не MD3 Expressive (дорожка 16px +
        // ползунок-палка 4x44px с разрывом 6px): в мини-плеере она занимала почти всю высоту бара.
        // Xs достаётся мини-плееру, Sm - полному; ramp по дорожке даёт мини-плееру более тонкую
        // линию. До Flare 0.4.0 такой ramp был невыразим - геометрия была одним токеном на все
        // размеры, и Size на неё не влиял.
        TrackHeightXs = "3px",
        TrackHeightSm = "4px",
        TrackHeightMd = "4px",
        TrackHeightLg = "6px",
        TrackHeightXl = "8px",
        TrackRadiusXs = "var(--flare-shape-full)",
        TrackRadiusSm = "var(--flare-shape-full)",
        TrackRadiusMd = "var(--flare-shape-full)",
        TrackRadiusLg = "var(--flare-shape-full)",
        TrackRadiusXl = "var(--flare-shape-full)",
        // Разрыв дорожки вокруг ползунка - приём MD3 Expressive; музыкальному плееру он чужд,
        // заливка должна доходить до ползунка вплотную.
        // Ноль ОБЯЗАН быть с единицей. FlareSlider ставит сегментам инлайновый
        // right: calc(100% - 46.79% + var(--_gap)), а внутри calc() голый 0 - это число, а не длина:
        // "процент плюс число" невалидно, декларация отбрасывается целиком, и заливка с дорожкой
        // схлопываются в нулевую ширину (виден только нативный ползунок, он на calc не завязан).
        GapRadius = "0px",
        Gap = "0px",
        // Круглая "таблетка" вместо вертикальной палки. Высота обязана совпадать с шириной, а
        // ширина - одна на все размеры (ramp есть только у высоты), поэтому и высота везде 12px:
        // разведи их - и кружок станет овалом. Так же поступают все темы с круглой ручкой.
        HandleHeightXs = "12px",
        HandleHeightSm = "12px",
        HandleHeightMd = "12px",
        HandleHeightLg = "12px",
        HandleHeightXl = "12px",
        HandleWidth = "12px",
        // Expressive сплющивает ползунок при нажатии до 2px - здесь это выглядело бы дёрганьем.
        HandlePressedWidth = "12px",
        HandleRadius = "var(--flare-shape-full)",
        HandleClipPath = "none",
        HandleBorderWidth = "0px",
        // Иконки по краям дорожки: мы их не используем, значения - как у эталонных тем.
        IconSizeXs = "20px",
        IconSizeSm = "22px",
        IconSizeMd = "24px",
        IconSizeLg = "24px",
        IconSizeXl = "32px",
        // Длина вертикального слайдера по умолчанию; вертикальных у нас нет.
        Length = "12rem",
        // Следуем за локальным акцентом инстанса (Color="..." на самом слайдере).
        HandleFill = "var(--fc-main, var(--flare-color-primary))",
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "var(--flare-color-outline)",
        // Ореол под курсором соразмерен ползунку: при 40px вокруг 12px-точки вздувался пузырь.
        StateLayerSize = "28px",
        StateHoverOpacity = "0.08",
        StatePressedOpacity = "0.10",
        StopColor = "var(--flare-color-on-primary)",
        StopColorSelected = "var(--flare-color-on-primary)",
        StopSize = "4px",
        ValueBg = "var(--flare-color-inverse-surface)",
        ValueColor = "var(--flare-color-inverse-on-surface)",
    };

    private static readonly DataGridTokens DataGrid = new()
    {
        SortIconSize = "1.125rem",
        SortPrioritySize = "0.625rem",
        FilterIconSize = "1.125rem",
        BoolIconSize = "1.25rem",
        BtnIconSize = "1rem",
        CloseIconSize = "1.125rem",
        ChevronSize = "1.25rem",
        DetailIconSize = "1.25rem",
        TreeToggleSize = "1.25rem",
        CompositeLabelSize = "0.6875rem",
        ResizeHandleWidth = "4px",
        RecordDividerWidth = "2px",
        AggregateDividerWidth = "2px",
        FilterGroupRail = "3px",
        ActiveCellOutline = "2px solid var(--flare-color-primary)",
        ColumnPickerMinWidth = "160px",
        RowSelectedHoverPct = "18%",
        RowEditingPct = "6%",
        LoadingVeilPct = "55%",
        LoadingDim = "0.6",
    };

    // Rating и Pagination в приложении пока не используются, но токены обязаны нести реальный
    // ramp: "initial" (стояло здесь раньше) означает "не переопределяю, возьми значение
    // компонента", а после 0.2.0 брать стало нечего - звезда и кнопки схлопывались. Значения - как
    // у эталонных тем.
    private static readonly RatingTokens Rating = new()
    {
        SizeXs = "1rem",
        SizeSm = "1.25rem",
        SizeMd = "1.5rem",
        SizeLg = "2rem",
        SizeXl = "2.5rem",
        EmptyColor = "var(--flare-color-outline-variant)",
        FilledColor = "var(--flare-color-primary)",
        HoverScale = "1.15",
    };

    private static readonly PaginationTokens Pagination = new()
    {
        SizeXs = "1.5rem",
        SizeSm = "1.75rem",
        SizeMd = "2.25rem",
        SizeLg = "2.75rem",
        SizeXl = "3.25rem",
        Radius = "var(--flare-shape-small)",
        BorderColor = "var(--flare-color-outline-variant)",
        ActiveColor = "var(--flare-color-primary)",
    };

    private static readonly TimelineTokens Timeline = new()
    {
        DotSize = "1.25rem",
        DotBg = "var(--flare-color-surface)",
        DotBorderWidth = "2px",
        DotIconSize = "0.75rem",
        LineWidth = "2px",
        LineColor = "var(--flare-color-outline-variant)",
        ConnectorWidth = "2rem",
    };

    private static readonly StepperTokens Stepper = new()
    {
        FocusRingThickness = "2px",
        FocusRingColor = "var(--flare-color-primary)",
        CircleSize = "2rem",
        CircleBorderWidth = "2px",
        CircleIconSize = "1.125rem",
        ConnectorThickness = "2px",
        ConnectorMinLength = "1.5rem",
        ConnectorColor = "var(--flare-color-outline-variant)",
        ConnectorActiveColor = "var(--flare-color-primary)",
        StepMinWidth = "5rem",
    };

    private static readonly TreeTokens Tree = new()
    {
        ToggleHoverBg = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)",
        DropInsideBg = "color-mix(in srgb, var(--flare-color-primary) 12%, transparent)",
        Indent = "var(--flare-spacing-12)",
        ToggleSize = "1.5rem",
        IconSize = "1.25rem",
        SelectedBg = "color-mix(in srgb, var(--flare-color-primary) 16%, transparent)",
        SelectedColor = "var(--flare-color-primary)",
        DropIndicatorColor = "var(--flare-color-primary)",
    };

    private static readonly CalendarTokens Calendar = new()
    {
        EventPadY = "0.0625rem",
        MaxWidth = "400px",
        MonthMinWidth = "16rem",
        NavBtnSize = "2rem",
        CellMinHeight = "3rem",
        DayNumSize = "1.5rem",
        TodayBg = "var(--flare-color-primary)",
        TodayColor = "var(--flare-color-on-primary)",
        SelectedBg = "color-mix(in srgb, var(--flare-color-primary) 16%, var(--flare-color-surface))",
        OtherMonthOpacity = "0.3",
    };

    private static readonly TableOfContentsTokens TableOfContents = new()
    {
        ActiveWeight = "600",
        HoverBgOpacity = "40%",
        LineHeight = "1.4",
        TitleTracking = "0.05em",
        TitleWeight = "600",
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "var(--flare-color-on-surface-variant)",
        TitleColor = "var(--flare-color-on-surface-variant)",
        RailColor = "var(--flare-color-outline-variant)",
        RailWidth = "1px",
        ActiveBg = "var(--flare-color-primary-container)",
        ActiveRadius = "var(--flare-shape-full)",
        // С единицей: значение уходит в calc(-1 * var(--flare-toc-marker-width)), а голый ноль там
        // остаётся числом, и свойство-длина его отвергнет (та же мина, что была у слайдера).
        MarkerWidth = "0px",
        LinkPadX = "0.75rem",
        Indent = "0.75rem",
    };

    private static readonly ColorPickerTokens ColorPicker = new()
    {
        CheckerColor = "var(--flare-color-outline-variant)",
        ThumbBg = "var(--flare-color-surface)",
        ThumbBorderColor = "var(--flare-color-outline)",
    };

    private static readonly AppBarTokens AppBar = new()
    {
        Gap = "0.25rem",
        Height = "4rem",
        HeightDense = "3rem",
        PaddingX = "0.5rem",
        TitlePaddingX = "0.75rem",
    };

    private static readonly BreadcrumbTokens Breadcrumb = new()
    {
        LinkHoverOpacity = "0.8",
        SeparatorOpacity = "0.5",
    };

    private static readonly DateTimePickerTokens DateTimePicker = new()
    {
        PanelGap = "1rem",
    };

    private static readonly DropzoneTokens Dropzone = new()
    {
        BorderWidth = "2px",
        HoverBg = "color-mix(in srgb, var(--flare-color-primary) 5%, var(--flare-color-surface))",
        DraggingBg = "color-mix(in srgb, var(--flare-color-primary) 10%, var(--flare-color-surface))",
        DraggingRingWidth = "2px",
        IconSize = "2.5rem",
    };

    private static readonly FormTokens Form = new()
    {
        HorizontalColumns = "auto 1fr",
    };

    private static readonly LayoutTokens Layout = new()
    {
        AppBarHeight = "64px",
        ContentPadding = "34px 40px",
        ContentPaddingMobile = "20px 18px",
        DrawerRailWidth = "3.5rem",
        DrawerWidth = "15.5rem",
    };

    private static readonly LinkTokens Link = new()
    {
        FocusRingWidth = "2px",
        HoverOpacity = "0.8",
    };

    private static readonly OtpTokens Otp = new()
    {
        BorderWidth = "1px",
        CellHeight = "3rem",
        CellWidth = "2.75rem",
        FocusRingWidth = "2px",
        FontSize = "1.25rem",
        FontWeight = "700",
    };

    private static readonly PickerTokens Picker = new()
    {
        OutsideOpacity = "0.4",
        DisabledOpacity = "0.3",
        WeekNumberOpacity = "0.7",
    };

    private static readonly ScrimTokens Scrim = new()
    {
        Opacity = "0.5",
    };

    private static readonly ScrollTopTokens ScrollTop = new()
    {
        TopInset = "1.5rem",
        TopSize = "2.75rem",
    };

    private static readonly SkeletonTokens Skeleton = new()
    {
        PulseMinOpacity = "0.4",
        WaveOpacity = "12%",
    };

    private static readonly TableTokens Table = new()
    {
        CellPaddingH = "1rem",
        CellPaddingV = "0.75rem",
        StripeOpacity = "4%",
    };

    private static readonly TimePickerTokens TimePicker = new()
    {
        ColumnsSepSize = "1.5rem",
        DisplaySize = "2.75rem",
        HeadlineTracking = "0.05em",
        PanelRadius = "var(--flare-shape-extra-large)",
        TimeSepSize = "2.5rem",
    };

    // ----- composition -------------------------------------------------------------------------------

    public static readonly DesignTokens Design = new()
    {
        FocusRing = "2px solid var(--flare-color-primary)",
        Typography = Typography,
        Shape = Shape,
        Elevation = Elevation,
        Motion = Motion,
        State = State,
        Spacing = Spacing,

        Badge = Badge,
        Alert = Alert,
        Button = Button,
        ButtonGroup = ButtonGroup,
        SplitButton = SplitButton,
        ToggleButton = ToggleButton,
        Fab = Fab,
        Menu = Menu,
        Checkbox = Checkbox,
        Radio = Radio,
        Chip = Chip,
        Tabs = Tabs,
        Slider = Slider,
        Input = Input,
        Dialog = Dialog,
        Drawer = Drawer,
        Snackbar = Snackbar,
        Tooltip = Tooltip,
        Popover = Popover,
        DataGrid = DataGrid,
        Card = Card,
        Avatar = Avatar,
        Progress = Progress,
        Rating = Rating,
        Pagination = Pagination,
        Timeline = Timeline,
        Stepper = Stepper,
        Tree = Tree,
        Calendar = Calendar,
        Switch = Switch,
        Nav = Nav,
        BottomNav = BottomNav,
        TableOfContents = TableOfContents,
        ColorPicker = ColorPicker,
        AppBar = AppBar,
        Breadcrumb = Breadcrumb,
        DateTimePicker = DateTimePicker,
        Dropzone = Dropzone,
        Form = Form,
        Layout = Layout,
        Link = Link,
        Otp = Otp,
        Picker = Picker,
        Scrim = Scrim,
        ScrollTop = ScrollTop,
        Skeleton = Skeleton,
        Table = Table,
        TimePicker = TimePicker,

        // Custom vars for surfaces the standard role set doesn't cover: the frosted mini-player/full-player
        // bar ("glass") and the gradient logo mark / avatar swatches ("accent-strong" as the gradient's
        // second stop). The mock's dimmer "faint" text tone is now the first-class OnSurfaceVariant2 role
        // (Flare 0.1.2), so it's no longer an --deka-* var. Consumed via var(--deka-*) in scoped CSS.
        Extended = new Dictionary<string, string>
        {
            ["--deka-accent-strong"] = "oklch(0.5 0.21 300)",
            ["--deka-glass"] = "oklch(1 0 0 / 0.72)",
            ["--deka-gradient-accent"] = "linear-gradient(150deg, var(--flare-color-primary), var(--deka-accent-strong))",
        },
    };

    private static TypeStyle T(string size, string weight, string lineHeight, string spacing) => new()
    {
        FontFamily = FontStack,
        FontWeight = weight,
        FontSize = size,
        LineHeight = lineHeight,
        LetterSpacing = spacing,
    };
}
