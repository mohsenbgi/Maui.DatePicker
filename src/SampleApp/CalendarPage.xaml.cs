using Maui.DatePicker.Enums;
using System.Globalization;

namespace SampleApp
{
    public partial class CalendarPage : ContentPage
    {
        public CalendarPage()
        {
            InitializeComponent();

            DarkBtn.Clicked += (s, e) => App.Current.UserAppTheme = AppTheme.Dark;
            LightBtn.Clicked += (s, e) => App.Current.UserAppTheme = AppTheme.Light;
            popupOpener.Clicked += (s, e) => dialog.Open();

            FaBtn.Clicked += (s, e) => dialog.Language = Language.Persian;
            EnBtn.Clicked += (s, e) => dialog.Language = Language.English;

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => dialog.Open();
            datePicker.GestureRecognizers.Add(tap);

            datePicker.Focused += (s, e) => dialog.Open();
            dialog.DateSelected += (s, e) =>
            {
                Calendar cal = dialog.Language == Language.Persian ? new PersianCalendar() : new GregorianCalendar();
                datePicker.Text = $"{cal.GetYear(e.Date)}/{cal.GetMonth(e.Date)}/{cal.GetDayOfMonth(e.Date)}";
            };
        }
    }
}
