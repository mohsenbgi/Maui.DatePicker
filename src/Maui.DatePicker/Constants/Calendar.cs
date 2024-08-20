namespace Maui.DatePicker.Constants
{
    public static class Calendar
    {
        public const int NumberOfWeeksInMonth = 6;
        public const int NumberOfDaysInWeek = 7;
        public const int NumberOfDaysInMonth = NumberOfWeeksInMonth * NumberOfDaysInWeek;
        public const int GeneratedInVisibleMonthsCount = 2;
        public const int AnimationsLength = 200;
        public static Color CalendarGenericLightBackgroundColor => Color.FromArgb("#FFFFFF");
        public static Color CalendarGenericDarkBackgroundColor => Color.FromArgb("#1C1C1C");
    }
}
