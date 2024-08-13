using Maui.DatePicker.Constants;
using Maui.DatePicker.Enums;
using Maui.DatePicker.EventArgs;
using Maui.DatePicker.Extensions;
using Microsoft.Maui.Controls.Shapes;
using System;

namespace Maui.DatePicker.Dialogs;

public partial class DatePickerDialog : ContentView
{
    public static BindableProperty LanguageProperty =
        BindableProperty.Create(nameof(Language), typeof(Language), typeof(DatePickerDialog), null,
            propertyChanged: (bindable, oldValue, newValue) => ((DatePickerDialog)bindable).OnLanguageChanged((Language)oldValue, (Language)newValue));

    public Language Language
    {
        get => (Language)GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    SelectYearDialog _yearsView;
    SelectMonthDialog _monthsView;
    Calendar.Calendar _calendar;
    public DatePickerDialog()
    {
        InitializeComponent();

        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => popup.Open();
        DatePicker.Focused += (s, e) => popup.Open();
        DatePicker.Clicked += (s, e) => popup.Open();
    }

    public void OnLanguageChanged(Language oldValue, Language newValue)
    {
        Config.Language = newValue;
        var flowDirection = newValue.GetDirection();

        _calendar = new Calendar.Calendar();
        _calendar.ActiveMonthChanged += MonthChanged;
        popup.Content = _calendar;

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
        await popup.NavigateBack();
    }

    public async void OnMonthSelected(object? sender, int selectedMonth)
    {
        var currentDate = _calendar.ActiveMonth.SelectedDate;
        var cal = Config.Language.GetCalendar();
        var expectedDate = new DateTime(int.Parse(currentYear.Text), selectedMonth, cal.GetDayOfMonth(currentDate), cal);
        _calendar.GoToDate(expectedDate);
        currentMonth.IsVisible = true;
        navBox.IsVisible = true;
        await popup.NavigateBack();
    }

    public void ChangeDialog(object? sender, TappedEventArgs eventArgs)
    {
        if (popup.CurrentContent == _yearsView) return;


        if(popup.CurrentContent == _monthsView)
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
        if (popup.CurrentContent == _monthsView) return;

        currentMonth.IsVisible = false;
        navBox.IsVisible = false;
        await popup.NavigateTo(_monthsView);
    }

    public async void SelectYear(object? sender, TappedEventArgs eventArgs)
    {
        if (popup.CurrentContent == _yearsView) return;

        await popup.NavigateTo(_yearsView);
    }
}