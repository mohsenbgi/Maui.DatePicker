using Bumptech.Glide.Load.Resource.Gif;
using Maui.DatePicker.Dialogs;
using Maui.DatePicker.Enums;
using System.Globalization;

namespace SampleApp
{
    public partial class CalendarPage : ContentPage
    {
        DatePickerDialog _dialog;
        public CalendarPage()
        {
            InitializeComponent();

            DarkBtn.Clicked += (s, e) => App.Current.UserAppTheme = AppTheme.Dark;
            LightBtn.Clicked += (s, e) => App.Current.UserAppTheme = AppTheme.Light;
            InitCalendarDialog();
        }

        public void InitCalendarDialog(Language language = Language.Persian)
        {
            _dialog = new DatePickerDialog() { Language = language };
            grid.Add(_dialog);

            popupOpener.Clicked += (s, e) => _dialog.Open();

            FaBtn.Clicked += (s, e) =>
            {
                grid.Remove(_dialog);
                InitCalendarDialog(Language.Persian);
            };
            EnBtn.Clicked += (s, e) =>
            {
                grid.Remove(_dialog);
                InitCalendarDialog(Language.English);
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => _dialog.Open();
            datePicker.GestureRecognizers.Add(tap);

            datePicker.Focused += (s, e) => _dialog.Open();
            _dialog.DateSelected += (s, e) =>
            {
                Calendar cal = _dialog.Language == Language.Persian ? new PersianCalendar() : new GregorianCalendar();
                datePicker.Text = $"{cal.GetYear(e.Date)}/{cal.GetMonth(e.Date)}/{cal.GetDayOfMonth(e.Date)}";
            };
        }
    }
}
