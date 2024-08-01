using Maui.DatePicker.Interfaces;

namespace Maui.DatePicker.Scheduler
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
