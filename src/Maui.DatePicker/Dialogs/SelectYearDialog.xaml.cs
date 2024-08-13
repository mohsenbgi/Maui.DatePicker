using Maui.DatePicker.Constants;
using Maui.DatePicker.Extensions;

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
    public string SelectedYear { get; set; }

    public EventHandler<int> YearSelected;

    List<string> _yearsDataItems;
    public SelectYearDialog()
    {
        InitializeComponent();

        YearsDataItems = new List<string>(200);
        int startYear = Config.Language.GetCalendar().GetYear(DateTime.Now.AddYears(-100));
        for (int year = startYear; year < startYear + 200; year++)
        {
            YearsDataItems.Add(year.ToString());
        }
        SelectedYear = Config.Language.GetCalendar().GetYear(DateTime.Now).ToString();

        #if WINDOWS
            Loaded += (s, e) => ScrollTo(YearsDataItems.IndexOf(SelectedYear), position: ScrollToPosition.Center, animate: false);
        #else
            Loaded += (s, e) => ScrollTo(SelectedYear, null, ScrollToPosition.Center, false);
        #endif

    }

    public void OnYearTapped(object sender, TappedEventArgs eventArgs)
    {
        SelectedYear = ((Label)((Border)sender).Content).Text;
        YearSelected?.Invoke(sender, int.Parse(SelectedYear));
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
    }
}