using Maui.DatePicker.Common;
using Maui.DatePicker.Interfaces;

namespace Maui.DatePicker.EventArgs
{
    public class SelectedDateChangedEventArgs : ChangedEventArgs<DateTime>
    {
        public SelectedDateChangedEventArgs(DateTime oldValue, DateTime newValue) : base(oldValue, newValue)
        {
        }
    }
}
