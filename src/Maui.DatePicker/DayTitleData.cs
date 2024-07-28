using Maui.DatePicker.Interfaces;

namespace Maui.DatePicker
{
    public class DayTitleData : IDayTitleView
    {
        public string Text { get; set; }

        public void Replace(IDayTitleView newView)
        {
            Text = newView.Text;
        }
    }

}
