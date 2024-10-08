﻿using Maui.DatePicker.Interfaces;
using Maui.DatePicker.Constants;
using Maui.DatePicker.Extensions;
using Maui.DatePicker.Helpers;
using Microsoft.Maui.Controls;

namespace Maui.DatePicker.Calendar;

public class DayView : Border, IDayView, IDisposable
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
        = BindableProperty.Create(nameof(IsToday), typeof(bool), typeof(DayView), false,
            propertyChanged: OnIsTodayChanged);

    public static BindableProperty IsDisabledProperty
        = BindableProperty.Create(nameof(IsDisable), typeof(bool), typeof(DayView), false,
            propertyChanged: OnIsDisabledChanged);

    public static BindableProperty DateTimeProperty
        = BindableProperty.Create(nameof(DateTime), typeof(DateTime), typeof(DayView), default);

    #endregion

    #region Events

    public event EventHandler<TappedEventArgs> Tapped;
    public event EventHandler IsSelectedChanged;

    #endregion

    #region Ctor

    Color _originBackgroundColor;
    public DayView()
    {
        var tabGestureRecognizer = new TapGestureRecognizer();
        tabGestureRecognizer.Tapped += OnTapped;
        GestureRecognizers.Add(tabGestureRecognizer);

        var pointer = new PointerGestureRecognizer();
        pointer.PointerEntered += PointerEntered;
        pointer.PointerExited += PointerExited;
        GestureRecognizers.Add(pointer);

        if(Application.Current != null)
            Application.Current.RequestedThemeChanged += OnThemeChanged;

        Behaviors.Add(new DayViewIsSelectedBehavior());

        _title = new DayTitleView();
        Content = _title;
    }

    #endregion

    #region Methods

    public void Dispose()
    {
        if (Application.Current != null)
            Application.Current.RequestedThemeChanged -= OnThemeChanged;
    }

    protected virtual void OnTapped(object? sender, TappedEventArgs e)
    {
        if (sender is DayView day)
        {
            Tapped?.Invoke(sender, e);
        }
    }

    void OnThemeChanged(object? sender, System.EventArgs eventArgs)
    {
        GenerateTextColor();
    }

    private void GenerateTextColor()
    {
        Color color;

        if (IsDisable) color = Colors.Transparent;
        else
        {
            if (IsToday)
            {
                color = Application.Current?.RequestedTheme == AppTheme.Light ? Color.FromArgb("#e67207") : Color.FromArgb("#faa95c");
            }
            else
            {
                color = Application.Current?.RequestedTheme == AppTheme.Light ? Colors.Black : Colors.White;
            }
        }

        Title.TextColor = color;
    }

    private static void OnIsTodayChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((DayView)bindable).GenerateTextColor();
    }

    private static void OnIsDisabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((DayView)bindable).GenerateTextColor();
    }

    void PointerEntered(object? sender, PointerEventArgs eventArgs)
    {
        var element = (DayView)sender;

        if (element.IsSelected || element.IsDisable) return;

        var defaultBackgroundColor = Application.Current?.RequestedTheme == AppTheme.Light
            ? Constants.Calendar.CalendarGenericLightBackgroundColor : Constants.Calendar.CalendarGenericDarkBackgroundColor;

        if (element.BackgroundColor == null || element.BackgroundColor == Colors.Transparent) element.BackgroundColor = defaultBackgroundColor;

        _originBackgroundColor = element.BackgroundColor;
        if (Application.Current?.RequestedTheme == AppTheme.Light)
        {
            element.BackgroundColor = element.BackgroundColor.Darker();
        }
        else
        {
            element.BackgroundColor = element.BackgroundColor.Lighter();
        }
    }

    void PointerExited(object? sender, PointerEventArgs eventArgs)
    {
        var element = (DayView)sender;

        if (element.IsSelected || element.IsDisable) return;
        element.BackgroundColor = _originBackgroundColor;
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
    DayView _dayView;
    protected override void OnAttachedTo(DayView bindable)
    {
        _dayView = bindable;
        bindable.IsSelectedChanged += IsSelectedChanged;
        
        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += OnThemeChanged;

        base.OnAttachedTo(bindable);
    }

    protected override void OnDetachingFrom(DayView bindable)
    {
        bindable.IsSelectedChanged -= IsSelectedChanged;

        if (Application.Current != null)
            Application.Current.RequestedThemeChanged -= OnThemeChanged;
        
        _dayView = null;

        base.OnDetachingFrom(bindable);
    }

    void OnThemeChanged(object? sender, System.EventArgs eventArgs)
    {
        IsSelectedChanged(_dayView, System.EventArgs.Empty);
    }

    void IsSelectedChanged(object? sender, System.EventArgs eventArgs)
    {
        var view = (DayView)sender;

        var bgColor = Application.Current?.RequestedTheme == AppTheme.Light 
            ? Constants.Calendar.CalendarGenericLightBackgroundColor 
            : Constants.Calendar.CalendarGenericDarkBackgroundColor;

        view.BackgroundColor = bgColor;

        if (view.IsSelected)
        {
            if (Application.Current?.RequestedTheme == AppTheme.Light)
            {
                view.BackgroundColor = view.BackgroundColor.Darker().Darker();
            }
            else
            {
                view.BackgroundColor = view.BackgroundColor.Lighter().Lighter();
            }
        }
        else
        {
            view.BackgroundColor = bgColor;
        }
    }
}