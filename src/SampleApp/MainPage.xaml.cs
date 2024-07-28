using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace SampleApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            App.Current.UserAppTheme = AppTheme.Light;
            InitializeComponent();
        }

    }

    public class PersianCulture : CultureInfo
    {

        private readonly Calendar cal;

        public PersianCulture()

            : this("fa-IR", true)
        {

        }
        public PersianCulture(string cultureName, bool useUserOverride)

            : base(cultureName, useUserOverride)
        {
            cal = new PersianCalendar();

            var fieldInfo = typeof(DateTimeFormatInfo).GetField("calendar", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null)
                fieldInfo.SetValue(DateTimeFormat, cal);

            var field = typeof(CultureInfo).GetField("calendar", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
                field.SetValue(DateTimeFormat, cal);

            DateTimeFormat.AbbreviatedDayNames = ["یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه"];
            DateTimeFormat.ShortestDayNames = ["ی", "د", "س", "چ", "پ", "ج", "ش"];
            DateTimeFormat.DayNames = ["یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه"];
            DateTimeFormat.AbbreviatedMonthNames = ["فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی",
                                      "بهمن", "اسفند", ""];
            DateTimeFormat.MonthNames = ["فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی",
                                      "بهمن", "اسفند", ""];
            DateTimeFormat.AMDesignator = "صبح";
            DateTimeFormat.PMDesignator = "عصر";
            DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            DateTimeFormat.FirstDayOfWeek = DayOfWeek.Saturday;
        }

        public override Calendar Calendar => cal;
    }

}
