using Maui.DatePicker.Interfaces;
using Maui.DatePicker.Constants;

namespace Maui.DatePicker.Calendar;

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
       = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(DayView), false,
           propertyChanged: (bindable, oldValue, newValue) => ((DayView)bindable).OnIsSelectedChanged((bool)oldValue, (bool)newValue));

    public static BindableProperty IsTodayProperty
        = BindableProperty.Create(nameof(IsToday), typeof(bool), typeof(DayView), false);

    public static BindableProperty IsDisabledProperty
        = BindableProperty.Create(nameof(IsDisable), typeof(bool), typeof(DayView), false);

    public static BindableProperty DateTimeProperty
        = BindableProperty.Create(nameof(DateTime), typeof(DateTime), typeof(DayView), default);

    #endregion

    #region Events

    public event EventHandler<TappedEventArgs> Tapped;
    public event EventHandler IsSelectedChanged;

    #endregion

    #region Ctor

    public DayView()
    {
        var tabGestureRecognizer = new TapGestureRecognizer();
        tabGestureRecognizer.Tapped += OnTapped;
        GestureRecognizers.Add(tabGestureRecognizer);

        var pointer = new PointerGestureRecognizer();
        pointer.PointerEntered += PointerEntered;
        pointer.PointerExited += PointerExited;
        GestureRecognizers.Add(pointer);

        Behaviors.Add(new DayViewIsSelectedBehavior());

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

    static void PointerEntered(object? sender, PointerEventArgs eventArgs)
    {
        var element = (DayView)sender;

        if (element.IsSelected || element.IsDisable) return;

        if (element.BackgroundColor == null || element.BackgroundColor == Colors.Transparent) element.BackgroundColor = Colors.White;

        element.BackgroundColor = new Color(element.BackgroundColor.Red - .08f, element.BackgroundColor.Green - .08f, element.BackgroundColor.Blue - .08f);
    }

    static void PointerExited(object? sender, PointerEventArgs eventArgs)
    {
        var element = (DayView)sender;

        if (element.IsSelected || element.IsDisable) return;

        element.BackgroundColor = new Color(element.BackgroundColor.Red + .08f, element.BackgroundColor.Green + .08f, element.BackgroundColor.Blue + .08f);
    }

    protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
    {
        IsSelectedChanged?.Invoke(this, System.EventArgs.Empty);
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

public class DayViewIsSelectedBehavior : Behavior<DayView>
{
    protected override void OnAttachedTo(DayView bindable)
    {
        bindable.IsSelectedChanged += IsSelectedChanged;
        base.OnAttachedTo(bindable);
    }

    protected override void OnDetachingFrom(DayView bindable)
    {
        bindable.IsSelectedChanged -= IsSelectedChanged;
        base.OnDetachingFrom(bindable);
    }

    void IsSelectedChanged(object? sender, System.EventArgs eventArgs)
    {
        var view = (DayView)sender;
        if (view.IsSelected)
        {
            view.BackgroundColor = Color.FromArgb("C8C8C8");
        }
        else
        {
            view.BackgroundColor = Colors.White;
        }
    }
}