using Maui.DatePicker.EventArgs;
using Microsoft.Maui.Controls.Shapes;

namespace Maui.DatePicker.Dialogs;

public partial class DatePickerDialog : ContentView
{
    public SelectYearDialog _yearsView;

    public DatePickerDialog()
    {
        InitializeComponent();

        _yearsView = new SelectYearDialog();
        _yearsView.YearSelected += OnYearSelected;

        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => popup.Open();
        DatePicker.Focused += (s, e) => popup.Open();
        DatePicker.Clicked += (s, e) => popup.Open();
    }

    public void MonthChanged(object sender, MonthChangedEventArgs eventArgs)
    {
        currentMonth.Text = eventArgs.NewValue.SelectedDate.ToString("MMM");
        currentYear.Text = eventArgs.NewValue.SelectedDate.ToString("yyyy");
    }

    public void GoToNextMonth(object sender, System.EventArgs eventArgs)
    {
        scheduler.GoToRightMonth();
    }

    public void GoToPrevMonth(object sender, System.EventArgs eventArgs)
    {
        scheduler.GoToLeftMonth();
    }

    public async void OnYearSelected(object sender, int selectedYear)
    {
        var currentDate = scheduler.ActiveMonth.SelectedDate;
        var calendar = Thread.CurrentThread.CurrentUICulture.Calendar;
        var expectedDate = new DateTime(selectedYear, calendar.GetMonth(currentDate), calendar.GetDayOfMonth(currentDate), calendar);
        scheduler.GoToDate(expectedDate);
        await popup.NavigateBack();
    }

    public async void SelectYear(object sender, TappedEventArgs eventArgs)
    {
        if (popup.CurrentContent == _yearsView) return;

        await popup.NavigateTo(_yearsView);
    }
}