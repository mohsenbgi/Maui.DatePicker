namespace Maui.DatePicker.Interfaces
{
    public interface IMonthView : IReplaceableView<IMonthView>
    {
        DateTime SelectedDate { get; set; }

        double Threshold { get; set; }

        int ViewId { get; set; }

        IReadOnlyList<IWeekView> Weeks { get; }

        IReadOnlyList<IDayView> Days { get; }
    }
}
