using System.Globalization;

namespace Maui.DatePicker.Extensions
{
    public static class CultureInfoExtensions
    {
        public static string[] GetAbbreviatedDayNamesStartedFromFirstDayOfWeek(this CultureInfo culture, DayOfWeek firstDayOfWeek)
        {
            var result = new string[7];
            var dayNames = culture.DateTimeFormat.AbbreviatedDayNames;
            var firstDayOfWeekIndex = (int)firstDayOfWeek;
            for (int i = 0; i < 7; i++)
            {
                result[i] = dayNames[(firstDayOfWeekIndex + i) % 7];
            }

            return result;
        }

        public static string[] GetShortestDayNamesStartedFromFirstDayOfWeek(this CultureInfo culture, DayOfWeek firstDayOfWeek)
        {
            var result = new string[7];
            var dayNames = culture.DateTimeFormat.ShortestDayNames;
            var firstDayOfWeekIndex = (int)firstDayOfWeek;
            for (int i = 0; i < 7; i++)
            {
                result[i] = dayNames[(firstDayOfWeekIndex + i) % 7];
            }

            return result;
        }

        public static string[] GetDayNamesStartedFromFirstDayOfWeek(this CultureInfo culture, DayOfWeek firstDayOfWeek)
        {
            var result = new string[7];
            var dayNames = culture.DateTimeFormat.DayNames;
            var firstDayOfWeekIndex = (int)firstDayOfWeek;
            for (int i = 0; i < 7; i++)
            {
                result[i] = dayNames[(firstDayOfWeekIndex + i) % 7];
            }

            return result;
        }
    }
}
