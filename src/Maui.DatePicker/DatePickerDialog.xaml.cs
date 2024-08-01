namespace Maui.DatePicker;

public partial class DatePickerDialog : ContentView
{
	public DatePickerDialog()
	{
		InitializeComponent();

        rightMonth.Clicked += (s, e) => scheduler.GoToRightMonth();
        leftMonth.Clicked += (s, e) => scheduler.GoToLeftMonth();

        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => popup.Open();
        DatePicker.Focused += (s, e) => popup.Open();

        scheduler.ActiveMonthChanged += (s, e) =>
        {
            currentMonth.Text = e.NewValue.SelectedDate.ToString("MMM");
            currentYear.Text = e.NewValue.SelectedDate.ToString("yyyy");
        };
    }
}