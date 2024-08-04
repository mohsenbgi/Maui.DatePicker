namespace Maui.DatePicker.Dialogs;

public partial class SelectYearDialog : CollectionView
{
    public List<string> YearsDataItems
    {
        get => _yearsDataItems;
        set
        {
            _yearsDataItems = value;
            OnPropertyChanged();
        }
    }

    public EventHandler<int> YearSelected;

    List<string> _yearsDataItems;
    public SelectYearDialog()
    {
        InitializeComponent();

        YearsDataItems = new List<string>(200);
        int startYear = Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateTime.Now.AddYears(-100));
        for (int year = startYear; year < startYear + 200; year++)
        {
            YearsDataItems.Add(year.ToString());
        }

        #if WINDOWS
                SizeChanged += (s, e) => ScrollTo(25, position: ScrollToPosition.Center, animate: false);
        #else
                SizeChanged += (s, e) => ScrollTo(Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateTime.Now).ToString(), null, ScrollToPosition.Center, false);
        #endif

    }

    public void OnYearTapped(object sender, TappedEventArgs eventArgs)
    {
        var selectedYear = int.Parse(((Label)((Border)sender).Content).Text);
        YearSelected?.Invoke(sender, selectedYear);
    }

    
}