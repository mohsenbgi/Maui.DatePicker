namespace Maui.DatePicker.Interfaces
{
    public interface IWeekView : IReplaceableView<IWeekView>
    {
        public bool IsSelected { get; set; }

        public IReadOnlyList<IDayView> Days { get; }

        void AddDay(int dayOfWeek, IDayView day);
    }
}
