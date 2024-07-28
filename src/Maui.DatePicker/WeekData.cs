using Maui.DatePicker.Interfaces;
using Maui.DatePicker.Constants;

namespace Maui.DatePicker
{
    public class WeekData : IWeekView
    {
        private Lazy<IDayView[]> _days;

        public WeekData()
        {
            _days = new Lazy<IDayView[]>(() => (new IDayView[Constants.DatePicker.NumberOfDaysInWeek]));
        }

        public bool IsSelected { get; set; }

        public IReadOnlyList<IDayView> Days => _days.Value;

        public void AddDay(int dayOfWeek, IDayView day)
        {
            _days.Value[dayOfWeek] = day;
        }

        public void Replace(IWeekView newView)
        {
            for (int i = 0; i < Constants.DatePicker.NumberOfDaysInWeek; i++)
            {
                _days.Value[i].Replace(newView.Days[i]);
            }
            IsSelected = newView.IsSelected;
        }
    }

}
