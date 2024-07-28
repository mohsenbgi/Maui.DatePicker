namespace Maui.DatePicker.Common
{
    public class ChangedEventArgs<T> : System.EventArgs
    {
        public T? OldValue { get; private set; }
        public T? NewValue { get; private set; }

        public ChangedEventArgs(T? oldValue, T? newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
