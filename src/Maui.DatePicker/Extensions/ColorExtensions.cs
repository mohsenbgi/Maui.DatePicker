namespace Maui.DatePicker.Extensions
{
    public static class ColorExtensions
    {
        const float LighterFactor = 1.5f;
        const float DarkerFactor = 0.95f;

        public static Color Lighter(this Color color)
        {
            return new Color(
                color.Red * LighterFactor,
                color.Green * LighterFactor,
                color.Blue * LighterFactor,
                color.Alpha);
        }

        public static Color Darker(this Color color)
        {
            return new Color(
                color.Red * DarkerFactor,
                color.Green * DarkerFactor,
                color.Blue * DarkerFactor,
                color.Alpha);
        }

        public static Color ContrastColor(this Color color)
        {
            // Calculate the perceptive luminance (aka luma) - human eye favors green color 
            double luma = ((0.299 * color.Red) + (0.587 * color.Green) + (0.114 * color.Blue)) / 255;

            // Return black for bright colors, white for dark colors
            return luma > 0.5 ? Colors.Black : Colors.White;
        }
    }
}
