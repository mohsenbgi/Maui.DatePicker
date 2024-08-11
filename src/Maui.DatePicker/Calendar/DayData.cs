using Maui.DatePicker.Interfaces;

namespace Maui.DatePicker.Calendar
{
    public class DayData : IDayView
    {
        public DayData()
        {
            Title = new DayTitleData();
        }

        public DateTime DateTime { get; set; }

        public bool IsToday { get; set; }

        public bool IsSelected { get; set; }

        public bool IsDisable { get; set; }

        public IDayTitleView Title { get; private set; }

        public void Replace(IDayView newView)
        {
            DateTime = newView.DateTime;
            IsToday = newView.IsToday;
            IsSelected = newView.IsSelected;
            IsDisable = newView.IsDisable;
            Title.Replace(newView.Title);
        }
    }

}
