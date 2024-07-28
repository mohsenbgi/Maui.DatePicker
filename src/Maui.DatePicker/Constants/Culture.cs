using System.Globalization;

namespace Maui.DatePicker.Constants
{
    public static class Culture
    {
        public static CultureInfo[] SupportedCultures => CultureInfo.GetCultures(CultureTypes.AllCultures);

        public static CultureInfo Current => Thread.CurrentThread.CurrentCulture;
    }
}
