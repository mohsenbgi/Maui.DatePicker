namespace Maui.DatePicker.EventArgs
{
    public class DateSelectedEventArgs : System.EventArgs
    {
        public DateTime Date { get; private set; }

        public DateSelectedEventArgs(DateTime date)
        {
            Date = date;
        }
    }
}
