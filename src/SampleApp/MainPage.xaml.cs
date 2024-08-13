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
}
