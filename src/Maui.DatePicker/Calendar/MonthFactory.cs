﻿using Maui.DatePicker.Constants;
using Maui.DatePicker.Enums;
using Maui.DatePicker.Extensions;
using Maui.DatePicker.Interfaces;
using System.Globalization;

namespace Maui.DatePicker.Calendar
{
    public static class MonthFactory
    {
        private static (WeekT[], DayT[]) CreateWeeksAndDays<WeekT, DayT, DayTitleT>(DateTime selectedDate)
            where WeekT : IWeekView, new()
            where DayT : IDayView, new()
            where DayTitleT : IDayTitleView, new()
        {
            DateTime realFirstDateOfMonth = selectedDate.AddDays(-Config.Language.GetCalendar().GetDayOfMonth(selectedDate) + 1);

            DateTime firstDateOfMonth = realFirstDateOfMonth.GetFirstDateOfWeek();

            WeekT[] weeks = new WeekT[Constants.Calendar.NumberOfWeeksInMonth];
            DayT[] days = new DayT[Constants.Calendar.NumberOfDaysInMonth];

            int dayIndex = 0;
            for (int weekIndex = 0; weekIndex < Constants.Calendar.NumberOfWeeksInMonth; weekIndex++)
            {
                var currentWeek = new WeekT();
                weeks[weekIndex] = currentWeek;

                for (int dayOfWeekIndex = 0; dayOfWeekIndex < Constants.Calendar.NumberOfDaysInWeek; dayOfWeekIndex++)
                {
                    var currentDateOfMonth = firstDateOfMonth.AddDays(dayIndex);

                    var toAddDay = new DayT
                    {
                        DateTime = currentDateOfMonth,
                        IsToday = currentDateOfMonth.IsToday(),
                        IsSelected = currentDateOfMonth.Date == selectedDate.Date,
                        IsDisable = Config.Language.GetCalendar().GetMonth(currentDateOfMonth) != Config.Language.GetCalendar().GetMonth(selectedDate)
                    };
                    toAddDay.Title.Text = Config.Language.GetCalendar().GetDayOfMonth(currentDateOfMonth).ToString();

                    currentWeek.AddDay(dayOfWeekIndex, toAddDay);
                    days[dayIndex] = toAddDay;

                    if (toAddDay.IsSelected)
                    {
                        currentWeek.IsSelected = true;
                    }

                    dayIndex++;
                }
            }

            return (weeks, days);
        }

        public static MonthData CreateMonthData(int viewId, DateTime selectedDate)
        {
            var month = new MonthData
            {
                ViewId = viewId,
                SelectedDate = selectedDate,
            };

            var (weeks, days) = CreateWeeksAndDays<WeekData, DayData, DayTitleData>(selectedDate);

            month.Weeks = weeks;
            month.Days = days;

            return month;
        }

        public static MonthView CreateMonthView(int viewId, DateTime selectedDate)
        {
            return new MonthView(viewId, selectedDate);
        }

        public static (WeekView[] weeks, DayView[] days) CreateWeekAndDays(DateTime selectedDate)
        {
            return CreateWeeksAndDays<WeekView, DayView, DayTitleView>(selectedDate);
        }
    }
}
