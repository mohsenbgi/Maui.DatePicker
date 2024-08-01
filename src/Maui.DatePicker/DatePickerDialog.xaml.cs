using Microsoft.Maui.Controls.Shapes;

namespace Maui.DatePicker;

public partial class DatePickerDialog : ContentView
{
    public CollectionView _yearsView;
    
    public List<string> YearsDataItems 
    {
        get => _yearsDataItems;
        set
        {
            _yearsDataItems = value;
            OnPropertyChanged();
        }
    }

    List<string> _yearsDataItems;
    public DatePickerDialog()
	{
        _yearsView = new CollectionView();
        _yearsView.SelectionMode = SelectionMode.Single;
        _yearsView.BindingContext = this;
        _yearsView.SetBinding(ItemsView.ItemsSourceProperty, nameof(YearsDataItems));
        _yearsView.ItemTemplate = new DataTemplate(() =>
        {
            var label = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 17
            };
            label.SetBinding(Label.TextProperty, ".");

            return new Border 
            { 
                Content = label,
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = 50,
                },
                HeightRequest = 50
            };
        });
        _yearsView.SelectionChanged += (sender, args) =>
        {
            var selectedYear = args.CurrentSelection.FirstOrDefault().ToString();
            var currentDate = scheduler.ActiveMonth.SelectedDate;
            var expectedDate = new DateTime(int.Parse(selectedYear), currentDate.Month, currentDate.Day);
            scheduler.GoToDate(expectedDate);
            popup.NavigateBack();
        };
        _yearsView.ItemsLayout = new GridItemsLayout(ItemsLayoutOrientation.Vertical) 
        { 
            Span = 4,
            VerticalItemSpacing = 20,
            HorizontalItemSpacing = 5
        };


        InitializeComponent();


        rightMonth.Clicked += (s, e) => scheduler.GoToRightMonth();
        leftMonth.Clicked += (s, e) => scheduler.GoToLeftMonth();

        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => popup.Open();
        DatePicker.Focused += (s, e) => popup.Open();
        DatePicker.Clicked += (s, e) => popup.Open();

        scheduler.ActiveMonthChanged += (s, e) =>
        {
            currentMonth.Text = e.NewValue.SelectedDate.ToString("MMM");
            currentYear.Text = e.NewValue.SelectedDate.ToString("yyyy");
        };

        var selectYearTapGesture = new TapGestureRecognizer();
        selectYearTapGesture.Tapped += (s, e) => SelectYear();
        currentYear.GestureRecognizers.Add(selectYearTapGesture);
    }

    public void SelectYear()
    {
        if(YearsDataItems is null)
        {
            YearsDataItems = new List<string>(200);
            int startYear = Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateTime.Now.AddYears(-100));
            for (int year = startYear; year < startYear + 200; year++)
            {
                YearsDataItems.Add(year.ToString());
            }
        }

        popup.NavigateTo(_yearsView);
        _yearsView.SizeChanged += (s, e) =>
        {
            _yearsView.ScrollTo(96);
        };
    }
}