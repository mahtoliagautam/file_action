using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace FileAction.App.UI;

/// <summary>
/// Colours and fonts shared by the wheel, the job cards and the shelf. Follows the Windows
/// light/dark app setting; <see cref="Refresh"/> re-reads it before a window is built.
/// </summary>
internal static class Theme
{
    private static readonly Palette DarkPalette = new(
        Backdrop: Color.FromArgb(0xD6, 0x21, 0x2B, 0x3B),
        BackdropEdge: Color.FromArgb(0x48, 0xFF, 0xFF, 0xFF),
        Slice: Color.FromArgb(0xD9, 0x2A, 0x34, 0x45),
        SliceEdge: Color.FromArgb(0x32, 0xFF, 0xFF, 0xFF),
        Hub: Color.FromArgb(0xE2, 0x1D, 0x26, 0x36),
        Panel: Color.FromArgb(0xC8, 0x24, 0x2E, 0x40),
        PanelEdge: Color.FromArgb(0x45, 0xFF, 0xFF, 0xFF),
        Text: Color.FromRgb(0xF5, 0xF5, 0xF7),
        Muted: Color.FromRgb(0x9C, 0x9C, 0xA8),
        Track: Color.FromArgb(0x30, 0xFF, 0xFF, 0xFF),
        Page: Color.FromRgb(0x10, 0x17, 0x22),
        Card: Color.FromArgb(0xA8, 0x30, 0x3D, 0x52),
        Hover: Color.FromArgb(0x18, 0xFF, 0xFF, 0xFF),
        Shadow: 0.55);

    private static readonly Palette LightPalette = new(
        Backdrop: Color.FromArgb(0xD8, 0xF0, 0xF7, 0xFD),
        BackdropEdge: Color.FromArgb(0x38, 0xFF, 0xFF, 0xFF),
        Slice: Color.FromArgb(0xC9, 0xFF, 0xFF, 0xFF),
        SliceEdge: Color.FromArgb(0x32, 0xFF, 0xFF, 0xFF),
        Hub: Color.FromArgb(0xE2, 0xFF, 0xFF, 0xFF),
        Panel: Color.FromArgb(0xBB, 0xF9, 0xFC, 0xFF),
        PanelEdge: Color.FromArgb(0x42, 0xFF, 0xFF, 0xFF),
        Text: Color.FromRgb(0x1B, 0x1B, 0x22),
        Muted: Color.FromRgb(0x6E, 0x6E, 0x7A),
        Track: Color.FromArgb(0x1E, 0x00, 0x00, 0x00),
        Page: Color.FromRgb(0xE7, 0xF0, 0xF8),
        Card: Color.FromArgb(0xA8, 0xFF, 0xFF, 0xFF),
        Hover: Color.FromArgb(0x0E, 0x00, 0x00, 0x00),
        Shadow: 0.22);

    private sealed record Palette(
        Color Backdrop,
        Color BackdropEdge,
        Color Slice,
        Color SliceEdge,
        Color Hub,
        Color Panel,
        Color PanelEdge,
        Color Text,
        Color Muted,
        Color Track,
        Color Page,
        Color Card,
        Color Hover,
        double Shadow);

    private static Palette _palette = DarkPalette;

    public static readonly Color AccentColor = Color.FromRgb(0xFF, 0x8A, 0x1F);
    public static readonly Color AccentColor2 = Color.FromRgb(0xFF, 0x4F, 0x6A);

    /// <summary>Orange → coral, used for the selected slice, active tab and progress bars.</summary>
    public static readonly LinearGradientBrush AccentGradient = Freeze(new LinearGradientBrush(AccentColor, AccentColor2, new Point(0, 0), new Point(1, 1)));

    public static readonly SolidColorBrush Accent = Freeze(new SolidColorBrush(AccentColor));
    public static readonly SolidColorBrush TextOnAccent = Freeze(new SolidColorBrush(Colors.White));
    public static readonly SolidColorBrush Error = Freeze(new SolidColorBrush(Color.FromRgb(0xF0, 0x5A, 0x5A)));
    public static readonly SolidColorBrush Success = Freeze(new SolidColorBrush(Color.FromRgb(0x3C, 0xC3, 0x6E)));

    public static readonly FontFamily Icons = new("Segoe Fluent Icons, Segoe MDL2 Assets");
    public static readonly FontFamily UiFont = new("Segoe UI Variable Text, Segoe UI");
    public static readonly FontFamily DisplayFont = new("Segoe UI Variable Display, Segoe UI");

    public static bool IsDark { get; private set; } = true;

    public static Brush Backdrop => Glass(_palette.Backdrop);
    public static Brush BackdropEdge => Solid(_palette.BackdropEdge);
    public static Brush Slice => Glass(_palette.Slice);
    public static Brush SliceEdge => Solid(_palette.SliceEdge);
    public static Brush Hub => Glass(_palette.Hub);
    public static Brush Panel => Glass(_palette.Panel);
    public static Brush PanelBorder => Solid(_palette.PanelEdge);
    public static Brush Text => Solid(_palette.Text);
    public static Brush Muted => Solid(_palette.Muted);
    public static Brush Track => Solid(_palette.Track);
    public static Brush Page => Glass(_palette.Page);
    public static Brush Card => Glass(_palette.Card);
    public static Brush Hover => Solid(_palette.Hover);
    public static Brush AccentSoft => Solid(Color.FromArgb(IsDark ? (byte)0x38 : (byte)0x26, AccentColor.R, AccentColor.G, AccentColor.B));

    /// <summary>Soft drop shadow under floating surfaces.</summary>
    public static System.Windows.Media.Effects.DropShadowEffect Shadow(double blur = 24) =>
        new() { BlurRadius = blur, ShadowDepth = 3, Direction = 270, Opacity = _palette.Shadow, Color = Colors.Black };

    /// <summary>Coloured glow for the highlighted slice.</summary>
    public static System.Windows.Media.Effects.DropShadowEffect Glow() =>
        new() { BlurRadius = 26, ShadowDepth = 0, Opacity = IsDark ? 0.75 : 0.5, Color = AccentColor };

    /// <summary>Forces dark (true) or light (false) instead of following Windows; used by the self-test.</summary>
    public static bool? Override { get; set; }

    /// <summary>Re-reads the Windows "app mode" (light/dark).</summary>
    public static void Refresh() => Use(Override ?? ReadSystemDark());

    private static void Use(bool dark)
    {
        IsDark = dark;
        _palette = dark ? DarkPalette : LightPalette;
    }

    private static bool ReadSystemDark()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            return key?.GetValue("AppsUseLightTheme") is not int light || light == 0;
        }
        catch (System.Security.SecurityException)
        {
            return true;
        }
    }

    private static SolidColorBrush Solid(Color color) => Freeze(new SolidColorBrush(color));

    private static LinearGradientBrush Glass(Color color) =>
        Freeze(new LinearGradientBrush(Lift(color, 0x24), color, new Point(0, 0), new Point(0, 1)));

    private static Color Lift(Color color, byte amount) => Color.FromArgb(
        color.A,
        (byte)(color.R + (255 - color.R) * amount / 255),
        (byte)(color.G + (255 - color.G) * amount / 255),
        (byte)(color.B + (255 - color.B) * amount / 255));

    private static T Freeze<T>(T brush)
        where T : Freezable
    {
        brush.Freeze();
        return brush;
    }
}
