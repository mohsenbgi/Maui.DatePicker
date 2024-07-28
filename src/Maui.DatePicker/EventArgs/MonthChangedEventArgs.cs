using Maui.DatePicker.Interfaces;
using Maui.DatePicker.Common;

namespace Maui.DatePicker.EventArgs
{
    public class MonthChangedEventArgs : ChangedEventArgs<IMonthView>
    {
        public MonthChangedEventArgs(IMonthView? oldValue, IMonthView? newValue) : base(oldValue, newValue)
        {

        }
    }
}
