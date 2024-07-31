namespace Maui.DatePicker;

public partial class DatePickerDialog : ContentView
{
	public DatePickerDialog()
	{
		InitializeComponent();

        rightMonth.Clicked += (s, e) => scheduler.GoToRightMonth();
        leftMonth.Clicked += (s, e) => scheduler.GoToLeftMonth();

        DatePicker.Clicked += (s, e) =>
        {
            popup.Open();
        };

        scheduler.ActiveMonthChanged += (s, e) =>
        {
            currentMonth.Text = e.NewValue.SelectedDate.ToString("MMM yyyy");
        };
    }
}