using Maui.DatePicker.Interfaces;

/* Unmerged change from project 'Maui.DatePicker (net8.0-android)'
Before:
using Maui.DatePicker.Constants;
After:
using Maui.DatePicker.Constants;
using Maui;
using Maui.DatePicker;
using Maui.DatePicker.Calendar;
*/
using Maui.DatePicker.Constants;

namespace Maui.DatePicker.Calendar;

public class WeekView : HorizontalStackLayout, IWeekView
{
    #region Fields

    private DayView[] _days;

    #endregion

    #region Properties

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public IReadOnlyList<DayView> Days => _days;

    IReadOnlyList<IDayView> IWeekView.Days => _days;

    #endregion

    #region Bindable Properties

    public static BindableProperty IsSelectedProperty
        = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(WeekView), false);

    #endregion

    #region Ctor

    public WeekView()
    {
        _days = new DayView[Constants.Calendar.NumberOfDaysInWeek];
    }

    #endregion

    #region Methods

    public void AddDay(int dayOfWeek, IDayView day)
    {
        if (day is DayView toAddDay)
        {
            _days[dayOfWeek] = toAddDay;
            Add(toAddDay);
        }
        else
        {
            toAddDay = _days[dayOfWeek] ?? new DayView();
            toAddDay.Replace(day);

            _days[dayOfWeek] = toAddDay;
            Add(toAddDay);
        }
    }

    public void Replace(IWeekView newView)
    {
        for (int i = 0; i < Constants.Calendar.NumberOfDaysInWeek; i++)
        {
            _days[i].Replace(newView.Days[i]);
        }
        IsSelected = newView.IsSelected;
    }

    #endregion

}
