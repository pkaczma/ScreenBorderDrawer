namespace ScreenBorderDrawer.Internal;
internal static class ColorExtensions
{
    public static Color Invert(this Color color)
    {
        return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
    }    
}