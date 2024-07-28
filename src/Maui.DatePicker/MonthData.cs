using Maui.DatePicker.Interfaces;

namespace Maui.DatePicker
{
    public record MonthData : IMonthView
    {
        public DateTime SelectedDate { get; set; }

        public double Threshold { get; set; }

        public int ViewId { get; set; }

        public IReadOnlyList<IWeekView> Weeks { get; set; }

        public IReadOnlyList<IDayView> Days { get; set; }

        public void Replace(IMonthView newView)
        {
            for (int i = 0; i < Constants.DatePicker.NumberOfWeeksInMonth; i++)
            {
                Weeks[i].Replace(newView.Weeks[i]);
            }
            for (int i = 0; i < Constants.DatePicker.NumberOfDaysInMonth; i++)
            {
                Days[i].Replace(newView.Days[i]);
            }
            SelectedDate = newView.SelectedDate;
            Threshold = newView.Threshold;
            ViewId = newView.ViewId;
        }
    }

}
