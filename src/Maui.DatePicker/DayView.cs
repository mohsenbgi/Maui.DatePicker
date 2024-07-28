using Maui.DatePicker.Interfaces;
using Maui.DatePicker.Constants;

namespace Maui.DatePicker;

public class DayView : Border, IDayView
{
    #region Fields

    private readonly DayTitleView _title;

    #endregion

    #region Properties

    public DateTime DateTime
    {
        get => (DateTime)GetValue(DateTimeProperty);
        set => SetValue(DateTimeProperty, value);
    }

    public bool IsToday
    {
        get => (bool)GetValue(IsTodayProperty);
        set => SetValue(IsTodayProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public DayTitleView Title => _title;

    IDayTitleView IDayView.Title => _title;

    public bool IsDisable
    {
        get => (bool)GetValue(IsDisabledProperty);
        set => SetValue(IsDisabledProperty, value);
    }

    #endregion

    #region Bindable Properties

    public static BindableProperty IsSelectedProperty
       = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(DayView), false);

    public static BindableProperty IsTodayProperty
        = BindableProperty.Create(nameof(IsToday), typeof(bool), typeof(DayView), false);

    public static BindableProperty IsDisabledProperty
        = BindableProperty.Create(nameof(IsDisable), typeof(bool), typeof(DayView), false);

    public static BindableProperty DateTimeProperty
        = BindableProperty.Create(nameof(DateTime), typeof(DateTime), typeof(DayView), default);

    #endregion

    #region Events

    public event EventHandler<TappedEventArgs> Tapped;

    #endregion

    #region Ctor

    public DayView()
    {
        var tabGestureRecognizer = new TapGestureRecognizer();
        tabGestureRecognizer.Tapped += OnTapped;
        GestureRecognizers.Add(tabGestureRecognizer);

        _title = new DayTitleView();
        Content = _title;
    }

    #endregion

    #region Methods

    protected virtual void OnTapped(object? sender, TappedEventArgs e)
    {
        if (sender is DayView day)
        {
            Tapped?.Invoke(sender, e);
        }
    }

    public void Replace(IDayView newView)
    {
        DateTime = newView.DateTime;
        IsToday = newView.IsToday;
        IsSelected = newView.IsSelected;
        IsDisable = newView.IsDisable;
        Title.Replace(newView.Title);
    }

    #endregion
}