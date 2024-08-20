using Maui.DatePicker.Constants;
using Maui.DatePicker.Enums;
using Maui.DatePicker.EventArgs;
using Maui.DatePicker.Extensions;
using Microsoft.Maui.Controls.Shapes;
using System;

namespace Maui.DatePicker.Dialogs;

public partial class DatePickerDialog : Popup
{
    public static BindableProperty LanguageProperty =
        BindableProperty.Create(nameof(Language), typeof(Language), typeof(DatePickerDialog), null,
            propertyChanged: (bindable, oldValue, newValue) => ((DatePickerDialog)bindable).OnLanguageChanged((Language)oldValue, (Language)newValue));

    public Language Language
    {
        get => (Language)GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public EventHandler<DateSelectedEventArgs> DateSelected;

    SelectYearDialog _yearsView;
    SelectMonthDialog _monthsView;
    Calendar.Calendar _calendar;
    public DatePickerDialog()
    {
        InitializeComponent();
    }

    public void OnLanguageChanged(Language oldValue, Language newValue)
    {
        Config.Language = newValue;
        var flowDirection = newValue.GetDirection();

        okButton.Text = Constants.Resources.GetStringValue(Constants.Resources.Ok);
        cancelButton.Text = Constants.Resources.GetStringValue(Constants.Resources.Cancel);
        todayButton.Text = Constants.Resources.GetStringValue(Constants.Resources.Today);

        _calendar = new Calendar.Calendar();
        _calendar.ActiveMonthChanged += MonthChanged;
        Content = _calendar;

        _yearsView = new SelectYearDialog();
        _yearsView.YearSelected += OnYearSelected;
        _yearsView.FlowDirection = flowDirection;

        _monthsView = new SelectMonthDialog(new List<string>(Config.Language.GetCulture().DateTimeFormat.AbbreviatedMonthNames).Take(12).ToList());
        _monthsView.MonthSelected += OnMonthSelected;
        _monthsView.FlowDirection= flowDirection;
    }

    public void MonthChanged(object sender, MonthChangedEventArgs eventArgs)
    {
        currentMonth.Text = eventArgs.NewValue.SelectedDate.ToString("MMM", Config.Language.GetCulture());
        currentYear.Text = eventArgs.NewValue.SelectedDate.ToString("yyyy", Config.Language.GetCulture());
    }

    public void GoToNextMonth(object sender, System.EventArgs eventArgs)
    {
        _calendar.GoToRightMonth();
    }

    public void GoToPrevMonth(object sender, System.EventArgs eventArgs)
    {
        _calendar.GoToLeftMonth();
    }

    public async void OnYearSelected(object? sender, int selectedYear)
    {
        currentYear.Text = selectedYear.ToString();
        await NavigateBack();
    }

    public async void OnMonthSelected(object? sender, int selectedMonth)
    {
        var currentDate = _calendar.ActiveMonth.SelectedDate;
        var cal = Config.Language.GetCalendar();
        var expectedDate = new DateTime(int.Parse(currentYear.Text), selectedMonth, cal.GetDayOfMonth(currentDate), cal);
        _calendar.GoToDate(expectedDate);
        await NavigateBack();
        currentMonth.IsVisible = true;
        todayButton.IsVisible = true;
        navBox.IsVisible = true;
    }

    public void ChangeDialog(object? sender, TappedEventArgs eventArgs)
    {
        if (CurrentContent == _yearsView) return;


        if(CurrentContent == _monthsView)
        {
            SelectYear(sender, eventArgs);
        }
        else
        {
            SelectMonth(sender, eventArgs);
        }
    }

    public async void SelectMonth(object? sender, TappedEventArgs eventArgs)
    {
        if (CurrentContent == _monthsView) return;

        currentMonth.IsVisible = false;
        todayButton.IsVisible = false;
        navBox.IsVisible = false;
        await NavigateTo(_monthsView);
    }

    public async void SelectYear(object? sender, TappedEventArgs eventArgs)
    {
        if (CurrentContent == _yearsView) return;

        await NavigateTo(_yearsView);
    }

    private void OkButtonClicked(object sender, System.EventArgs e)
    {
        DateSelected?.Invoke(this, new DateSelectedEventArgs(_calendar.ActiveMonth.SelectedDate));
        Close();
    }

    private void CancelButtonClicked(object sender, System.EventArgs e)
    {
        Close();
    }

    private void TodayButtonClicked(object sender, System.EventArgs e)
    {
        _calendar.GoToday();
    }
}