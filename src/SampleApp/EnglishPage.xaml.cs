using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace SampleApp
{
    public partial class EnglishPage : ContentPage
    {

        public EnglishPage()
        {
            InitializeComponent();

            DarkBtn.Clicked += (s, e) => App.Current.UserAppTheme = AppTheme.Dark;
            LightBtn.Clicked += (s, e) => App.Current.UserAppTheme = AppTheme.Light;
            popupOpener.Clicked += (s, e) => dialog.Open();
        }
    }
}
