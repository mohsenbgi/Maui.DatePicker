using Maui.DatePicker.Constants;
using System.Globalization;

namespace Maui.DatePicker.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetFirstDateOfWeek(this DateTime date)
        {
            var difference = (7 + (date.DayOfWeek - Culture.Current.DateTimeFormat.FirstDayOfWeek)) % 7;
            return date.AddDays(-1 * difference).Date;
        }

        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Now.Date;
        }

        public static DateTime GetPrevMonthFirstDay(this DateTime date)
        {
            int currentYear = Culture.Current.Calendar.GetYear(date);
            int currentMonth = Culture.Current.Calendar.GetMonth(date);

            int nextMonth = currentMonth - 1;
            if (nextMonth < 1)
            {
                nextMonth = 12;
                currentYear--;
            }

            return Culture.Current.Calendar.ToDateTime(currentYear, nextMonth, 1, 0, 0, 0, 0);
        }

        public static DateTime GetNextMonthFirstDay(this DateTime date)
        {
            int currentYear = Culture.Current.Calendar.GetYear(date);
            int currentMonth = Culture.Current.Calendar.GetMonth(date);

            int nextMonth = currentMonth + 1;
            if (nextMonth > 12)
            {
                nextMonth = 1;
                currentYear++;
            }

            return Culture.Current.Calendar.ToDateTime(currentYear, nextMonth, 1, 0, 0, 0, 0);
        }
    }
}
