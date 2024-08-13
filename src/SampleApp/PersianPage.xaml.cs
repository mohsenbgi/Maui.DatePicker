using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace SampleApp
{
    public partial class PersianPage : ContentPage
    {
        public PersianPage()
        {
            InitializeComponent();

            DarkBtn.Clicked += (s, e) => App.Current.UserAppTheme = AppTheme.Dark;
            LightBtn.Clicked += (s, e) => App.Current.UserAppTheme = AppTheme.Light;
        }
    }
}
