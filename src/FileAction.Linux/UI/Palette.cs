using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Platform;

namespace FileAction.Linux.UI;

/// <summary>The same palette as the Windows app, for light and dark desktops.</summary>
public static class Palette
{
    public static bool IsDark { get; private set; } = true;

    /// <summary>Forces dark (true) or light (false); used by the self-test.</summary>
    public static bool? Override { get; set; }

    public static void Refresh()
    {
        var variant = Application.Current?.PlatformSettings?.GetColorValues().ThemeVariant;
        IsDark = Override ?? variant != PlatformThemeVariant.Light;
    }

    private static IBrush Solid(byte a, byte r, byte g, byte b) => new ImmutableSolidColorBrush(Color.FromArgb(a, r, g, b));

    private static IBrush Glass(byte a, byte r, byte g, byte b)
    {
        var color = Color.FromArgb(a, r, g, b);
        var highlight = Color.FromArgb(
            a,
            (byte)(r + (255 - r) * 0x24 / 255),
            (byte)(g + (255 - g) * 0x24 / 255),
            (byte)(b + (255 - b) * 0x24 / 255));
        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops = { new GradientStop(highlight, 0), new GradientStop(color, 1) },
        }.ToImmutable();
    }

    public static readonly Color AccentColor = Color.FromRgb(0xFF, 0x8A, 0x1F);
    public static readonly Color AccentColor2 = Color.FromRgb(0xFF, 0x4F, 0x6A);

    public static IBrush AccentGradient { get; } = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
        GradientStops = { new GradientStop(AccentColor, 0), new GradientStop(AccentColor2, 1) },
    }.ToImmutable();

    public static IBrush Accent { get; } = new ImmutableSolidColorBrush(AccentColor);
    public static IBrush TextOnAccent { get; } = Brushes.White;
    public static IBrush Error { get; } = Solid(0xFF, 0xF0, 0x5A, 0x5A);
    public static IBrush Success { get; } = Solid(0xFF, 0x3C, 0xC3, 0x6E);

    public static IBrush Backdrop => IsDark ? Glass(0xD6, 0x21, 0x2B, 0x3B) : Glass(0xD8, 0xF0, 0xF7, 0xFD);
    public static IBrush BackdropEdge => IsDark ? Solid(0x48, 0xFF, 0xFF, 0xFF) : Solid(0x38, 0xFF, 0xFF, 0xFF);
    public static IBrush Slice => IsDark ? Glass(0xD9, 0x2A, 0x34, 0x45) : Glass(0xC9, 0xFF, 0xFF, 0xFF);
    public static IBrush SliceEdge => Solid(0x32, 0xFF, 0xFF, 0xFF);
    public static IBrush Hub => IsDark ? Glass(0xE2, 0x1D, 0x26, 0x36) : Glass(0xE2, 0xFF, 0xFF, 0xFF);
    public static IBrush Panel => IsDark ? Glass(0xC8, 0x24, 0x2E, 0x40) : Glass(0xBB, 0xF9, 0xFC, 0xFF);
    public static IBrush PanelBorder => Solid(0x45, 0xFF, 0xFF, 0xFF);
    public static IBrush Text => IsDark ? Solid(0xFF, 0xF5, 0xF5, 0xF7) : Solid(0xFF, 0x1B, 0x1B, 0x22);
    public static IBrush Muted => IsDark ? Solid(0xFF, 0x9C, 0x9C, 0xA8) : Solid(0xFF, 0x6E, 0x6E, 0x7A);
    public static IBrush Track => IsDark ? Solid(0x30, 0xFF, 0xFF, 0xFF) : Solid(0x1E, 0, 0, 0);
    public static IBrush Page => IsDark ? Glass(0xFF, 0x10, 0x17, 0x22) : Glass(0xFF, 0xE7, 0xF0, 0xF8);

    public static DropShadowEffect Shadow(double blur = 24) =>
        new() { BlurRadius = blur, OffsetX = 0, OffsetY = 3, Color = Colors.Black, Opacity = IsDark ? 0.55 : 0.22 };

    public static DropShadowEffect Glow() =>
        new() { BlurRadius = 26, OffsetX = 0, OffsetY = 0, Color = AccentColor, Opacity = IsDark ? 0.75 : 0.5 };
}
