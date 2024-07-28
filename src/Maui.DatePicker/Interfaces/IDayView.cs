namespace Maui.DatePicker.Interfaces
{
    public interface IDayView : IReplaceableView<IDayView>
    {
        DateTime DateTime { get; set; }

        bool IsToday { get; set; }

        bool IsSelected { get; set; }

        IDayTitleView Title { get; }

        bool IsDisable { get; set; }
    }
}
