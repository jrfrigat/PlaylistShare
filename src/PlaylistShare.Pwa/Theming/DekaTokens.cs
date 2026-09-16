using Flare.Abstractions.Tokens;
using Flare.Abstractions.Tokens.Components;

namespace PlaylistShare.Pwa.Theming;

/// <summary>
/// Полный набор нецветовых дизайн-токенов Flare для PlaylistShare (Deka):
/// типографика Onest, скругления (10-26px), диффузные мягкие тени, пружинная анимация,
/// состояния интерактивности, сетка отступов и полная геометрия компонентов макета Obsidian Wave.
/// Все поля обязательны (Flare 0.1+ / 0.35+).
/// </summary>
internal static class DekaTokens
{
    private const string JakartaStack = "'Plus Jakarta Sans', system-ui, -apple-system, sans-serif";
    private const string InterStack = "'Inter', system-ui, -apple-system, sans-serif";

    // ----- Базовые шкалы (Core Scales) -----------------------------------------------------------------

    private static readonly TypographyTokens Typography = new()
    {
        // ── Plus Jakarta Sans ─────────────────────────────────────────────
        // display-hero (48px / 56px / -0.03em / 800)
        DisplayLarge = T("3rem", "800", "1.17", "-0.03em"),
        // промежуточный вариант, в макете не встречается — на всякий случай
        DisplayMedium = T("2.25rem", "800", "1.20", "-0.025em"),
        // headline-lg (32px / 40px / -0.02em / 700)
        DisplaySmall = T("2rem", "700", "1.25", "-0.02em"),

        // headline-lg (32px / 40px / -0.02em / 700)
        HeadlineLarge = T("2rem", "700", "1.25", "-0.02em"),
        // headline-lg-mobile (26px / 32px / -0.015em / 700)
        HeadlineMedium = T("1.625rem", "700", "1.23", "-0.015em"),
        // headline-md (22px / 28px / -0.01em / 600)
        HeadlineSmall = T("1.375rem", "600", "1.27", "-0.01em"),

        // headline-sm (18px / 24px / 600)
        TitleLarge = T("1.125rem", "600", "1.33", "0em"),
        TitleMedium = T("1rem", "600", "1.40", "0em"),
        TitleSmall = T("0.875rem", "600", "1.40", "0em"),

        // body-lg (16px / 24px / 400)
        BodyLarge = T("1rem", "400", "1.50", "0em"),
        // body-md (14px / 20px / 400)
        BodyMedium = T("0.875rem", "400", "1.43", "0em"),
        // body-sm (12px / 16px / 400)
        BodySmall = T("0.75rem", "400", "1.33", "0em"),

        // ── Inter ─────────────────────────────────────────────────────────
        // label-md (13px / 18px / 0.01em / 600) — в макете используется Inter
        LabelLarge = T("0.8125rem", "700", "1.38", "0.01em", InterStack),
        // label-md baseline
        LabelMedium = T("0.8125rem", "600", "1.38", "0.01em", InterStack),
        // label-sm (11px / 14px / 0.04em / 500)
        LabelSmall = T("0.6875rem", "500", "1.27", "0.04em", InterStack),
    };

    // Мягкие скругления макета (10px для мелких элементов, 18-22px для карточек, 26px для шторки/плеера)
    private static readonly ShapeTokens Shape = new()
    {
        None = "0px",
        ExtraSmall = "0.125rem",
        Small = "0.375rem",
        Medium = "0.5rem",
        Large = "0.75rem",
        ExtraLarge = "1rem",
        Full = "9999px",
        MorphDuration = "var(--flare-motion-duration-spring-fast)",
        MorphEasing = "var(--flare-motion-easing-spring-fast)",
    };

    // Диффузные тёмные тени Obsidian Wave с глубоким рассеиванием
    private static readonly ElevationTokens Elevation = new()
    {
        Level0 = "none",
        // shadow-sm (чипы, мелкие бейджи)
        Level1 = "0 1px 2px 0 rgba(0,0,0,0.05)",
        // shadow (сайдбар, хедер, footer)
        Level2 = "0 1px 8px rgba(0,0,0,0.5)",
        // shadow-md (кнопки, hover-элементы)
        Level3 = "0 4px 6px -1px rgba(0,0,0,0.1), 0 2px 4px -2px rgba(0,0,0,0.1)",
        // карточки / hover карточек
        Level4 = "0 8px 30px rgba(0,0,0,0.5)",
        // главный баннер интеграции
        Level5 = "0 8px 32px rgba(0,0,0,0.5)",
    };

    // Плавная музыкальная моторика с пружинами
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
        EasingSpring = "cubic-bezier(0.32, 1.4, 0.5, 1)",
        EasingSpringFast = "cubic-bezier(0.34, 1.56, 0.64, 1)",
        EasingSpringSlow = "cubic-bezier(0.3, 1.25, 0.45, 1)",
        DurationSpring = "300ms",
        DurationSpringFast = "250ms",
        DurationSpringSlow = "400ms",
    };

    // Полупрозрачные наложения интерактивных состояний
    private static readonly StateTokens State = new()
    {
        HoverOpacity = "0.06",
        SelectedOpacity = "0.14",
        FocusOpacity = "0.10",
        PressedOpacity = "0.12",
        DraggedOpacity = "0.16",
        DisabledOpacity = "0.38",
        DisabledContainerOpacity = "0.12",
        HoverLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-hover-opacity) * 100%), transparent)",
        FocusLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-focus-opacity) * 100%), transparent)",
        PressedLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-pressed-opacity) * 100%), transparent)",
        DraggedLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-dragged-opacity) * 100%), transparent)",
        SelectedLayer = "color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-selected-opacity) * 100%), transparent)",
        SelectedHoverLayer = "color-mix(in srgb, var(--flare-color-primary) 18%, transparent)",
        FocusHoverLayer = "color-mix(in srgb, currentColor calc(var(--flare-state-focus-opacity) * 100%), transparent)",
    };

    // 2px-базовый ритм отступов
    private static readonly SpacingTokens Spacing = new()
    {
        S0 = "0",
        S1 = "0.125rem",  // 2px
        S2 = "0.25rem",   // 4px
        S3 = "0.375rem",  // 6px
        S4 = "0.5rem",    // 8px
        S5 = "0.625rem",  // 10px
        S6 = "0.75rem",   // 12px
        S8 = "1rem",      // 16px
        S10 = "1.25rem",  // 20px
        S12 = "1.5rem",   // 24px
        S16 = "2rem",     // 32px
        S24 = "3rem",     // 48px
        S32 = "4rem",     // 64px
    };

    private static readonly BorderTokens Border = new()
    {
        Width = "1px",
        WidthEmphasis = "2px",
        Style = "solid",
        Divider = "var(--flare-border-width) var(--flare-border-style) var(--flare-color-outline-variant)",
        Outline = "var(--flare-border-width) var(--flare-border-style) var(--flare-color-outline)",
    };

    private static readonly ScrollbarTokens Scrollbar = new()
    {
        Width = "thin",
        Size = "8px",
        Thumb = "color-mix(in srgb, var(--flare-color-on-surface) 24%, transparent)",
        ThumbHover = "color-mix(in srgb, var(--flare-color-on-surface) 40%, transparent)",
        Track = "transparent",
        Radius = "var(--flare-shape-full)",
    };

    private static readonly IconTokens Icon = new()
    {
        MorphDuration = "var(--flare-motion-duration-spring-fast)",
        MorphEasing = "var(--flare-motion-easing-spring-fast)",
        MorphScale = "0.6",
        MorphRotate = "90deg",
    };

    private static readonly DragTokens Drag = new()
    {
        SourceOpacity = "0.4",
        PreviewElevation = "var(--flare-elevation-4)",
        PreviewOpacity = "1",
        ZoneActiveBackground = "color-mix(in srgb, var(--flare-color-primary) calc(var(--flare-state-dragged-opacity) * 100%), transparent)",
        ZoneActiveOutline = "var(--flare-color-primary)",
        IndicatorColor = "var(--flare-color-primary)",
    };

    private static readonly OverlayTokens Overlay = new()
    {
        ViewportInset = "2rem",
        ViewportInsetCompact = "1.5rem",
        PanelMaxBlockSize = "75dvh",
    };

    private static readonly ChartTokens Chart = new()
    {
        Series1 = "var(--flare-color-primary)",
        Series2 = "var(--flare-color-tertiary)",
        Series3 = "var(--flare-color-secondary)",
        Series4 = "color-mix(in srgb, var(--flare-color-primary) 50%, var(--flare-color-tertiary))",
        Series5 = "color-mix(in srgb, var(--flare-color-secondary) 50%, var(--flare-color-tertiary))",
        Series6 = "color-mix(in srgb, var(--flare-color-primary) 50%, var(--flare-color-secondary))",
        Series7 = "color-mix(in srgb, var(--flare-color-primary) 65%, var(--flare-color-on-surface))",
        Series8 = "color-mix(in srgb, var(--flare-color-tertiary) 65%, var(--flare-color-on-surface))",
        Series9 = "color-mix(in srgb, var(--flare-color-secondary) 65%, var(--flare-color-on-surface))",
        Series10 = "color-mix(in srgb, var(--flare-color-primary) 55%, var(--flare-color-surface))",
        Series11 = "color-mix(in srgb, var(--flare-color-tertiary) 55%, var(--flare-color-surface))",
        Series12 = "color-mix(in srgb, var(--flare-color-secondary) 55%, var(--flare-color-surface))",
        LineWidth = "2",
        LineCap = "round",
        PointRadius = "2.5",
        PointOpacity = "0.6",
        BubbleMinRadius = "4",
        BubbleMaxRadius = "24",
        BarRadius = "2",
        AreaOpacity = "0.35",
        RadarFillOpacity = "0.2",
        WedgeOpacity = "0.7",
        SliceStrokeColor = "var(--flare-color-surface)",
        SliceStrokeWidth = "1.5",
        RampColor = "var(--flare-color-primary)",
        RampMinOpacity = "0.12",
        RampMaxOpacity = "1",
        CellRadius = "2",
        CellGap = "2",
        CellHoverOpacity = "0.85",
        GridColor = "var(--flare-color-outline-variant)",
        GridWidth = "0.5",
        GridDash = "none",
        GridMinorColor = "color-mix(in srgb, var(--flare-color-outline-variant) 45%, transparent)",
        GridMinorWidth = "0.5",
        LabelColor = "var(--flare-color-on-surface-variant)",
        LabelSize = "9px",
        ValueColor = "var(--flare-color-on-surface-variant)",
        ValueOnFillColor = "var(--flare-color-surface)",
        ValueSize = "8px",
        AxisTitleColor = "var(--flare-color-on-surface-variant)",
        AxisTitleSize = "10px",
        Surface = "var(--flare-color-surface)",
        Radius = "var(--flare-shape-medium)",
        BorderWidth = "1px",
        BorderColor = "var(--flare-color-outline-variant)",
        Padding = "var(--flare-spacing-8)",
        Gap = "var(--flare-spacing-4)",
        LegendGap = "var(--flare-spacing-6)",
        LegendItemGap = "var(--flare-spacing-3)",
        LegendDotSize = "0.625rem",
        LegendDotRadius = "var(--flare-shape-full)",
        LegendSize = "var(--flare-typescale-label-small-size)",
        LegendColor = "var(--flare-color-on-surface-variant)",
        LegendOffOpacity = "0.4",
        TrendWidth = "1.5",
        TrendDash = "5 4",
        TrendOpacity = "0.7",
        AnnotationColor = "var(--flare-color-error)",
        AnnotationWidth = "1.5",
        AnnotationDash = "4 3",
        AnnotationBandOpacity = "0.12",
        AnnotationArrowSize = "7",
        AnnotationPointRadius = "3.5",
        LineDashDashed = "calc(var(--flare-chart-line-width) * 2) calc(var(--flare-chart-line-width) * 3)",
        LineDashDotted = "0.1 calc(var(--flare-chart-line-width) * 3)",
        LineDashDashDot = "calc(var(--flare-chart-line-width) * 2) calc(var(--flare-chart-line-width) * 3) 0.1 calc(var(--flare-chart-line-width) * 3)",
        ZoomSelectionFill = "color-mix(in srgb, var(--flare-color-primary) 16%, transparent)",
        ZoomSelectionStroke = "var(--flare-color-primary)",
    };

    private static readonly GaugeTokens Gauge = new()
    {
        TrackColor = "var(--flare-color-surface-container-highest)",
        TrackWidth = "14",
        TrackCap = "round",
        FillColor = "var(--flare-color-primary)",
        FillWidth = "14",
        NeedleColor = "var(--flare-color-on-surface)",
        NeedleWidth = "3",
        NeedleLength = "0.78",
        PivotColor = "var(--flare-color-on-surface)",
        PivotRadius = "5",
        TickColor = "var(--flare-color-outline)",
        TickWidth = "1.5",
        TickLength = "7",
        TickMinorColor = "var(--flare-color-outline-variant)",
        TickMinorWidth = "1",
        TickMinorLength = "4",
        TickGap = "5",
        LabelColor = "var(--flare-color-on-surface-variant)",
        LabelSize = "9",
        ValueColor = "var(--flare-color-on-surface)",
        ValueSize = "var(--flare-typescale-headline-small-size)",
        ValueWeight = "500",
        BandOpacity = "0.55",
        TargetColor = "var(--flare-color-on-surface)",
        TargetWidth = "2.5",
    };

    // ----- Токены компонентов макета (Component Tokens) ------------------------------------------------

    private static readonly ButtonTokens Button = new()
    {
        LoadingOpacity = "0.8",
        ContainerRadius = "var(--flare-shape-small)",
        TextPaddingInlineLg = "0.75rem",
        TextPaddingInlineMd = "0.75rem",
        TextPaddingInlineSm = "0.75rem",
        TextPaddingInlineXl = "0.75rem",
        TextPaddingInlineXs = "0.75rem",
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
        RadiusXs = CornerRadiusTokens.All("var(--flare-shape-small)"),
        RadiusSm = CornerRadiusTokens.All("var(--flare-shape-small)"),
        RadiusMd = CornerRadiusTokens.All("var(--flare-shape-medium)"),
        RadiusLg = CornerRadiusTokens.All("var(--flare-shape-medium)"),
        RadiusXl = CornerRadiusTokens.All("var(--flare-shape-medium)"),
        FocusOutline = "2px solid var(--flare-color-primary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        FilledHoverShadow = "0 4px 20px rgba(160,120,255,0.35)",
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
        DisabledLayer = "transparent",
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
        OutlineWidthXs = "1px",
        OutlineWidthSm = "1px",
        OutlineWidthMd = "1px",
        OutlineWidthLg = "1px",
        OutlineWidthXl = "1px",
        SelectedRadiusXs = "var(--flare-shape-small)",
        SelectedRadiusSm = "var(--flare-shape-small)",
        SelectedRadiusMd = "var(--flare-shape-medium)",
        SelectedRadiusLg = "var(--flare-shape-medium)",
        SelectedRadiusXl = "var(--flare-shape-medium)",
        SelectedRadiusSquare = "var(--flare-shape-extra-small)",
        SelectedBg = "var(--flare-color-primary-container)",
        SelectedColor = "var(--flare-color-primary)",
        ElevatedSelectedBg = "var(--flare-color-primary-container)",
        ElevatedSelectedColor = "var(--flare-color-primary)",
        FilledSelectedBg = "var(--flare-color-primary)",
        FilledSelectedColor = "var(--flare-color-on-primary)",
        TonalSelectedBg = "var(--flare-color-primary)",
        TonalSelectedColor = "var(--flare-color-on-primary)",
        OutlinedSelectedBg = "var(--flare-color-primary-container)",
        OutlinedSelectedColor = "var(--flare-color-primary)",
        FilledUnselectedBg = "var(--flare-color-surface-container-high)",
        FilledUnselectedColor = "var(--flare-color-on-surface-variant)",
    };

    // Исправлены заполнители "xxx" на валидные токены Elevation
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
        StateLayer = "var(--flare-state-hover-layer)",
        PaddingTop = "var(--flare-spacing-4)",
        PaddingRight = "var(--flare-spacing-4)",
        PaddingBottom = "var(--flare-spacing-4)",
        PaddingLeft = "var(--flare-spacing-4)",
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
        FilledElevation = "var(--flare-elevation-0)",
        OutlinedElevation = "var(--flare-elevation-0)",
        TextElevation = "var(--flare-elevation-0)",
        TonalElevation = "var(--flare-elevation-0)",
    };

    private static readonly InputTokens Input = new()
    {
        IconSize = "1.25rem",
        FilledBg = "var(--flare-color-surface-container-low)",
        BorderColor = "var(--flare-color-outline)",
        OutlinedRadius = "var(--flare-shape-small)",
        BorderBottomColor = "var(--flare-color-outline)",
        FocusRing = "inset 0 0 0 1px var(--fc-main, var(--flare-color-primary))",
        FocusOutline = "none",
        FocusOutlineOffset = "0",
        HoverBorderBottomColor = "var(--flare-color-outline)",
        HoverStateLayer = "transparent",
        PaddingXs = "0.1875rem 0.5rem",
        PaddingSm = "0.375rem 0.625rem",
        PaddingMd = "0.875rem 1rem",
        PaddingLg = "1.125rem 1.125rem",
        PaddingXl = "1.375rem 1.25rem",
        HeightXs = "1.875rem",
        HeightSm = "2.25rem",
        HeightMd = "3.25rem",
        HeightLg = "3.75rem",
        HeightXl = "4.25rem",
        PlaceholderColor = "var(--flare-color-on-surface-variant)",
        DisabledBg = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)",
        DisabledIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 38%, transparent)",
        ErrorHoverIndicator = "color-mix(in srgb, var(--flare-color-on-surface) 8%, var(--flare-color-error))",
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    private static readonly NavTokens Nav = new()
    {
        ActiveWeight = "600",
        BadgeWeight = "600",
        RailLabelLineHeight = "1.15",
        ItemRadius = "var(--flare-shape-medium)",
        IndicatorRadius = "var(--flare-shape-medium)",
        ActiveIndicator = "var(--flare-color-primary-container)",
        ActiveLeftBar = "none",
        LinkDisabledOpacity = "var(--flare-state-disabled-opacity)",
        IconSize = "1.5rem",
        ItemHeight = "3rem",
    };

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
        IndicatorWidth = "0",
        ItemDisabledOpacity = "var(--flare-state-disabled-opacity)",
        ZIndex = "1100",
        ItemGap = "var(--flare-spacing-2)",
    };

    private static readonly DialogTokens Dialog = new()
    {
        Radius = "var(--flare-shape-extra-large)",
        IconSize = "1.5rem",
    };

    private static readonly DrawerTokens Drawer = new()
    {
        Width = "360px",
        MiniWidth = "72px",
        Border = "none",
        SectionBorder = "none",
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
        ItemPaddingBlockDense = "0.375rem",
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
        ItemGapDense = "0.5rem",
        ItemIconSize = "1.25rem",
        ItemFocusRingColor = "var(--flare-color-primary)",
        ItemFocusRingThickness = "2px",
        ItemFocusRingOffset = "-2px",
        ItemDisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    private static readonly ChipTokens Chip = new()
    {
        Radius = "var(--flare-shape-medium)",
        Height = "2rem",
        FilledBg = "var(--flare-color-surface-container-high)",
        ElevatedBg = "var(--flare-color-surface-container-low)",
        IconSizeXs = "0.875rem",
        IconSizeSm = "1rem",
        IconSizeMd = "1.125rem",
        IconSizeLg = "1.25rem",
        IconSizeXl = "1.5rem",
        AvatarSizeXs = "1rem",
        AvatarSizeSm = "1.125rem",
        AvatarSizeMd = "1.5rem",
        AvatarSizeLg = "1.875rem",
        AvatarSizeXl = "2.25rem",
        PaddingInlineXs = "var(--flare-spacing-4)",
        PaddingInlineSm = "var(--flare-spacing-5)",
        PaddingInlineMd = "var(--flare-spacing-8)",
        PaddingInlineLg = "var(--flare-spacing-10)",
        PaddingInlineXl = "var(--flare-spacing-12)",

    };

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
        SecondaryIndicatorThickness = "2px",
        SecondaryActiveColor = "var(--flare-color-on-surface)",
        TabDisabledOpacity = "var(--flare-state-disabled-opacity)",
        ScrollDisabledOpacity = "var(--flare-state-disabled-opacity)",
        TabHeight = "auto",
        TabMinWidth = "0",
        TabPaddingInline = "var(--flare-spacing-10)",
        IconSize = "1.5rem",
        LabelSpacing = "var(--flare-typescale-label-large-spacing)",

    };

    private static readonly SwitchTokens Switch = new()
    {
        TrackWidthXs = "36px",
        TrackWidthSm = "41px",
        TrackWidthMd = "46px",
        TrackWidthLg = "52px",
        TrackWidthXl = "58px",
        TrackHeightXs = "20px",
        TrackHeightSm = "24px",
        TrackHeightMd = "27px",
        TrackHeightLg = "31px",
        TrackHeightXl = "34px",
        TrackOffBg = "var(--flare-color-outline)",
        TrackOnBg = "var(--fc-main, var(--flare-color-primary))",
        TrackBorder = "none",
        TrackHoverBorderColor = "transparent",
        ThumbOffSizeXs = "1rem",
        ThumbOffSizeSm = "1.1875rem",
        ThumbOffSizeMd = "1.3125rem",
        ThumbOffSizeLg = "1.5rem",
        ThumbOffSizeXl = "1.625rem",
        ThumbOnSizeXs = "1rem",
        ThumbOnSizeSm = "1.1875rem",
        ThumbOnSizeMd = "1.3125rem",
        ThumbOnSizeLg = "1.5rem",
        ThumbOnSizeXl = "1.625rem",
        ThumbPressedOffSizeXs = "1.125rem",
        ThumbPressedOffSizeSm = "1.3125rem",
        ThumbPressedOffSizeMd = "1.4375rem",
        ThumbPressedOffSizeLg = "1.625rem",
        ThumbPressedOffSizeXl = "1.75rem",
        ThumbPressedOnSizeXs = "1.125rem",
        ThumbPressedOnSizeSm = "1.3125rem",
        ThumbPressedOnSizeMd = "1.4375rem",
        ThumbPressedOnSizeLg = "1.625rem",
        ThumbPressedOnSizeXl = "1.75rem",
        ThumbOffLeftXs = "0.125rem",
        ThumbOffLeftSm = "0.1875rem",
        ThumbOffLeftMd = "0.1875rem",
        ThumbOffLeftLg = "0.1875rem",
        ThumbOffLeftXl = "0.25rem",
        ThumbOnLeftXs = "calc(100% - 1.125rem)",
        ThumbOnLeftSm = "calc(100% - 1.375rem)",
        ThumbOnLeftMd = "calc(100% - 1.5rem)",
        ThumbOnLeftLg = "calc(100% - 1.6875rem)",
        ThumbOnLeftXl = "calc(100% - 1.875rem)",
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
        FocusShadowOff = "none",
        FocusShadowOn = "none",
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
        MinWidth = "20rem",
        MaxWidth = "36rem",
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
        MinWidthXs = "0.75rem",
        MinWidthSm = "0.875rem",
        MinWidthMd = "1rem",
        MinWidthLg = "1.25rem",
        MinWidthXl = "1.5rem",
        HeightXs = "0.75rem",
        HeightSm = "0.875rem",
        HeightMd = "1rem",
        HeightLg = "1.25rem",
        HeightXl = "1.5rem",
        DotSizeXs = "0.25rem",
        DotSizeSm = "0.3125rem",
        DotSizeMd = "0.375rem",
        DotSizeLg = "0.5rem",
        DotSizeXl = "0.625rem",
        PaddingXXs = "0.1875rem",
        PaddingXSm = "0.25rem",
        PaddingXMd = "0.25rem",
        PaddingXLg = "0.3125rem",
        PaddingXXl = "0.375rem",
        LabelSizeXs = "0.5625rem",
        LabelSizeSm = "0.625rem",
        LabelSizeMd = "0.6875rem",
        LabelSizeLg = "0.75rem",
        LabelSizeXl = "0.875rem",
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
        CircularSizeXs = "16px",
        CircularSizeSm = "24px",
        CircularSizeMd = "40px",
        CircularSizeLg = "48px",
        CircularSizeXl = "56px",
        CircularWidthXs = "2px",
        CircularWidthSm = "3px",
        CircularWidthMd = "4px",
        CircularWidthLg = "5px",
        CircularWidthXl = "6px",
        CircularCap = "round",
        CircularGap = "4px",
        LinearIndeterminateDuration = "1500ms",
        LinearIndeterminateEasing = "var(--flare-motion-easing-standard)",
        CircularIndeterminateRotationDuration = "1400ms",
        CircularIndeterminateProgressDuration = "1400ms",
        ContentColor = "var(--flare-color-on-surface)",
        ContentSizeXs = "0.5rem",
        ContentSizeSm = "0.625rem",
        ContentSizeMd = "0.75rem",
        ContentSizeLg = "1rem",
        ContentSizeXl = "1.25rem",
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
        IconSizeSm = "1.5rem",    // 24dp -> 40dp container
        IconSizeMd = "1.5rem",    // 24dp -> 56dp container
        IconSizeLg = "2.25rem",   // 36dp -> 96dp container
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

    private static readonly ButtonGroupTokens ButtonGroup = new()
    {
        StandardGapXs = "var(--flare-spacing-4)",
        StandardGapSm = "var(--flare-spacing-4)",
        StandardGapMd = "var(--flare-spacing-4)",
        StandardGapLg = "var(--flare-spacing-4)",
        StandardGapXl = "var(--flare-spacing-4)",
        ConnectedGap = "0",
        ConnectedOverlap = "-1px",
        ConnectedOuterRadius = "var(--flare-shape-small)",
        ConnectedSelectedRadius = "0",
        ConnectedInnerRadiusXs = "0",
        ConnectedInnerRadiusSm = "0",
        ConnectedInnerRadiusMd = "0",
        ConnectedInnerRadiusLg = "0",
        ConnectedInnerRadiusXl = "0",
        ConnectedPressedRadiusXs = "0",
        ConnectedPressedRadiusSm = "0",
        ConnectedPressedRadiusMd = "0",
        ConnectedPressedRadiusLg = "0",
        ConnectedPressedRadiusXl = "0",
        ZActive = "1",
    };

    private static readonly SplitButtonTokens SplitButton = new()
    {
        Gap = "0.125rem",
        TriggerWidthXs = "auto",
        TriggerWidthSm = "auto",
        TriggerWidthMd = "auto",
        TriggerWidthLg = "auto",
        TriggerWidthXl = "auto",
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
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    private static readonly RadioTokens Radio = new()
    {
        Size = "1.25rem",
        StateLayerHover = "color-mix(in srgb, var(--flare-color-on-surface) 8%, transparent)",
        StateLayerHoverChecked = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
        FocusOutline = "2px solid var(--flare-color-primary)",
        FocusOutlineOffset = "2px",
        FocusShadow = "none",
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
    };

    private static readonly SliderTokens Slider = new()
    {
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
        GapRadius = "0px",
        Gap = "0px",
        HandleHeightXs = "12px",
        HandleHeightSm = "12px",
        HandleHeightMd = "12px",
        HandleHeightLg = "12px",
        HandleHeightXl = "12px",
        HandleWidth = "12px",
        HandlePressedWidth = "12px",
        HandleRadius = "var(--flare-shape-full)",
        HandleClipPath = "none",
        HandleBorderWidth = "0px",
        IconSizeXs = "20px",
        IconSizeSm = "22px",
        IconSizeMd = "24px",
        IconSizeLg = "24px",
        IconSizeXl = "32px",
        Length = "12rem",
        HandleFill = "var(--fc-main, var(--flare-color-primary))",
        ActiveColor = "var(--flare-color-primary)",
        InactiveColor = "var(--flare-color-outline)",
        StateLayerSize = "28px",
        StateHoverOpacity = "0.08",
        StatePressedOpacity = "0.10",
        StopColor = "var(--flare-color-on-primary)",
        StopColorSelected = "var(--flare-color-on-primary)",
        StopSize = "4px",
        ValueBg = "var(--flare-color-inverse-surface)",
        ValueColor = "var(--flare-color-inverse-on-surface)",
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
        DisabledActiveColor = "color-mix(in srgb, var(--flare-color-on-surface) calc(var(--flare-state-disabled-opacity) * 100%), transparent)",
        DisabledInactiveColor = "color-mix(in srgb, var(--flare-color-on-surface) 12%, transparent)",
        FocusOutline = "none",
        FocusOutlineOffset = "0px",
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
        RangeLayer = "color-mix(in srgb, var(--flare-color-primary) 14%, transparent)",
        RowEditingPct = "6%",
        LoadingVeilPct = "55%",
        LoadingDim = "0.6",
    };

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
        BtnDisabledOpacity = "var(--flare-state-disabled-opacity)",
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
        Indent = "var(--flare-spacing-12)",
        ToggleSize = "1.5rem",
        IconSize = "1.25rem",
        SelectedBg = "color-mix(in srgb, var(--flare-color-primary) 16%, transparent)",
        SelectedColor = "var(--flare-color-primary)",
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
        Border = "none",
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

    private static readonly FileUploadTokens FileUpload = new()
    {
        BorderWidth = "2px",
        HoverBg = "color-mix(in srgb, var(--flare-color-primary) 5%, var(--flare-color-surface))",
        DraggingBg = "color-mix(in srgb, var(--flare-color-primary) 10%, var(--flare-color-surface))",
        DraggingRingWidth = "2px",
        IconSize = "2.5rem",
        ZoneMinHeight = "10rem",
        ZoneRadius = "var(--flare-shape-large)",
        FileIconSize = "1.25rem",
        RowGap = "var(--flare-spacing-2)",
        RowRadius = "var(--flare-shape-small)",
        RowActiveBg = "color-mix(in srgb, var(--flare-color-primary) 8%, transparent)",
        RowSuccessBg = "color-mix(in srgb, var(--flare-color-success) 8%, transparent)",
        RowErrorBg = "color-mix(in srgb, var(--flare-color-error) 8%, transparent)",
        RowErrorColor = "var(--flare-color-error)",
    };

    private static readonly FormTokens Form = new()
    {
        HorizontalColumns = "auto 1fr",
    };

    private static readonly LayoutTokens Layout = new()
    {
        AppBarHeight = "64px",
        AppBarHeightDense = "48px",
        AppBarBg = "var(--flare-color-surface)",
        ContentPadding = "0px",
        ContentPaddingMobile = "0px",
        DrawerRailWidth = "3.5rem",
        DrawerWidth = "15.5rem",
        AppBarBorder = "none",
        DrawerBorder = "none",
        AppBarShadow = "none",
    };

    private static readonly LinkTokens Link = new()
    {
        FocusRingWidth = "2px",
        HoverOpacity = "0.8",
        DisabledOpacity = "var(--flare-state-disabled-opacity)",
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
        PanelMinWidth = "22.5rem",
        PanelRadius = "var(--flare-shape-large)",
        HeaderHeight = "4rem",
        NavIconSize = "1.125rem",
        WeekdayHeight = "2.75rem",
        WeekdayFontSize = "var(--flare-typescale-body-large-size)",
        DaySize = "3rem",
        DayLayerSize = "2.5rem",
        DayFontSize = "var(--flare-typescale-body-large-size)",
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
    };

    private static readonly StripeTokens Stripe = new()
    {
        Background = "color-mix(in srgb, var(--flare-color-on-surface) 4%, transparent)",
    };

    private static readonly TouchTokens Touch = new()
    {
        TargetMin = "48px",
    };

    private static readonly ListTokens List = new()
    {
        Bg = "var(--flare-color-surface)",
        Radius = "var(--flare-shape-medium)",
        Divider = "1px solid var(--flare-color-outline-variant)",
        ItemHeight = "3.5rem",
        ItemHeightTwoLine = "4.5rem",
        ItemHeightDense = "3rem",
        ItemHeightTwoLineDense = "3.5rem",
        ItemPaddingBlock = "var(--flare-spacing-6)",
        ItemPaddingBlockDense = "var(--flare-spacing-3)",
        ItemPaddingInline = "var(--flare-spacing-8)",
        ItemGap = "var(--flare-spacing-6)",
        ItemContentGap = "var(--flare-spacing-1)",
        ItemRadius = "var(--flare-shape-small)",
        ItemLabelFont = "var(--flare-typescale-title-small-font)",
        ItemLabelSize = "var(--flare-typescale-title-small-size)",
        ItemColor = "var(--flare-color-on-surface)",
        ItemTrailingColor = "var(--flare-color-on-surface-variant2)",
        ItemSelectedBg = "var(--flare-color-primary-container)",
        ItemSelectedColor = "var(--flare-color-primary)",
        ItemDisabledOpacity = "var(--flare-state-disabled-opacity)",
        ItemIconSize = "1.5rem",
    };

    private static readonly AccordionTokens Accordion = new()
    {
        Border = "1px solid var(--flare-color-outline-variant)",
        Radius = "var(--flare-shape-medium)",
        PanelDivider = "1px solid var(--flare-color-outline-variant)",
        HeaderBg = "var(--flare-color-surface)",
        HeaderColor = "var(--flare-color-on-surface)",
        HeaderPaddingBlock = "var(--flare-spacing-8)",
        HeaderPaddingInline = "var(--flare-spacing-12)",
        HeaderGap = "var(--flare-spacing-4)",
        HeaderLabelFont = "var(--flare-typescale-title-small-font)",
        HeaderLabelSize = "var(--flare-typescale-title-small-size)",
        HeaderLabelWeight = "var(--flare-typescale-title-small-weight)",
        HeaderDisabledOpacity = "var(--flare-state-disabled-opacity)",
        IconSize = "1.25rem",
        BodyPaddingBlock = "var(--flare-spacing-8)",
        BodyPaddingInline = "var(--flare-spacing-12)",
        BodyColor = "var(--flare-color-on-surface-variant)",
    };

    private static readonly CollapseTokens Collapse = new()
    {
        HeaderBg = "transparent",
        HeaderColor = "var(--flare-color-on-surface)",
        HeaderRadius = "var(--flare-shape-small)",
        HeaderPaddingBlock = "var(--flare-spacing-6)",
        HeaderPaddingInline = "var(--flare-spacing-8)",
        HeaderGap = "var(--flare-spacing-4)",
        HeaderLabelFont = "var(--flare-typescale-title-small-font)",
        HeaderLabelSize = "var(--flare-typescale-title-small-size)",
        HeaderLabelWeight = "var(--flare-typescale-title-small-weight)",
        HeaderDisabledOpacity = "var(--flare-state-disabled-opacity)",
        IconColor = "var(--flare-color-on-surface-variant)",
    };

    private static readonly SplitterTokens Splitter = new()
    {
        GutterSize = "0.5rem",
        GripThickness = "2px",
        GripLength = "1.75rem",
        Color = "var(--flare-color-outline-variant)",
        HoverColor = "color-mix(in srgb, var(--flare-color-primary) 24%, transparent)",
        IconSize = "1.125rem",
        IconColor = "var(--flare-color-on-surface-variant)",
    };

    private static readonly TimePickerTokens TimePicker = new()
    {
        ColumnsSepSize = "1.5rem",
        DialCenterSize = "0.5rem",
        DialHandleSize = "3rem",
        DialSize = "16rem",
        DialTrackWidth = "0.125rem",
        PeriodHeight = "5rem",
        PeriodWidth = "3.25rem",
        TimeFieldHeight = "5rem",
        TimeFieldWidth = "6rem",
        DisplaySize = "3.5625rem",
        HeadlineTracking = "0.05em",
        PanelRadius = "var(--flare-shape-extra-large)",
        TimeSepSize = "3.5625rem",
    };

    // ----- Композиция (DesignTokens) -------------------------------------------------------------------

    public static readonly DesignTokens Design = new()
    {
        FocusRing = "2px solid var(--flare-color-primary)",
        Typography = Typography,
        Shape = Shape,
        Elevation = Elevation,
        Motion = Motion,
        State = State,
        Spacing = Spacing,
        Border = Border,
        Scrollbar = Scrollbar,
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
        Drag = Drag,
        Drawer = Drawer,
        Snackbar = Snackbar,
        Tooltip = Tooltip,
        Overlay = Overlay,
        Popover = Popover,
        DataGrid = DataGrid,
        Chart = Chart,
        Gauge = Gauge,
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
        FileUpload = FileUpload,
        Form = Form,
        Layout = Layout,
        Link = Link,
        Otp = Otp,
        Picker = Picker,
        Scrim = Scrim,
        ScrollTop = ScrollTop,
        Skeleton = Skeleton,
        Icon = Icon,
        Table = Table,
        TimePicker = TimePicker,
        Stripe = Stripe,
        Touch = Touch,
        List = List,
        Accordion = Accordion,
        Collapse = Collapse,
        Splitter = Splitter,

        // Кастомные переменные для музыкального плеера и акцентных градиентов
        Extended = new Dictionary<string, string>
        {
            // Глубокий стоп градиента — совпадает с primary-container #a078ff / inverse-primary #6d3bd7
            ["--deka-accent-strong"] = "oklch(0.50 0.220 293)",        // #6d3bd7

            // Матовое обсидиановое стекло — соответствует bg-surface-container-lowest/95 из макета
            ["--deka-glass"] = "oklch(0.16 0.008 286 / 0.95)", // #0e0e13 @ 95%

            // Чуть более прозрачное — для хедера (bg-surface-container-lowest/80)
            ["--deka-glass-header"] = "oklch(0.16 0.008 286 / 0.80)",

            // Тот же акцент, что и --flare-color-primary (лаванда #d0bcff)
            ["--deka-gradient-accent"] = "linear-gradient(150deg, var(--flare-color-primary), var(--deka-accent-strong))",
        },
    };

    private static TypeStyle T(string size, string weight, string lineHeight, string spacing, string family = JakartaStack) => new()
    {
        FontFamily = family,
        FontWeight = weight,
        FontSize = size,
        LineHeight = lineHeight,
        LetterSpacing = spacing,
    };
}
