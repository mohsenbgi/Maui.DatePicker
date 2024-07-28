using Maui.DatePicker;
using Maui.DatePicker.Animations;
using Maui.DatePicker.Constants;
using Maui.DatePicker.EventArgs;
using Maui.DatePicker.Extensions;
using Maui.DatePicker.Helpers;
using Maui.DatePicker.Interfaces;
using System.Globalization;

namespace Maui.DatePicker;

public partial class DatePicker : StackLayout
{
    #region Fields

    private bool _isRenderingMonthes;
    private Task _renderingMonthesTask;
    private double _arrangedWidth;
    private readonly PanGestureRecognizer _panGestureRecognizer;
    private readonly Dictionary<int, IMonthView> _monthViews;
    private bool _isHorizontalPan;
    private bool _isVerticalPan;
    private double _appliedTotalXDiff;
    private readonly IMonthView _todayMonthView;
    private MonthView _prevMonth;
    private MonthView _nextMonth;
    private MonthView _activeMonth;
    private MonthView _leftMonth => Culture.Current.TextInfo.IsRightToLeft ? _nextMonth : _prevMonth;
    private MonthView _rightMonth => Culture.Current.TextInfo.IsRightToLeft ? _prevMonth : _nextMonth;

    #endregion

    #region Properties

    public IMonthView ActiveMonth
    {
        get => (IMonthView)GetValue(ActiveMonthProperty);
        set => SetValue(ActiveMonthProperty, value);
    }

    public double Threshold
    {
        get => (double)GetValue(ThresholdProperty);
        set => SetValue(ThresholdProperty, value);
    }

    #endregion

    #region Bindable Properties

    public static readonly BindableProperty ActiveMonthProperty = BindableProperty.Create(
        nameof(ActiveMonth),
        typeof(IMonthView),
        typeof(DatePicker),
        null,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is DatePicker schedulerView)
            {
                schedulerView.OnActiveMonthChanged((IMonthView?)oldValue, (IMonthView?)newValue);
            }
        }
    );

    public static readonly BindableProperty ThresholdProperty =
            BindableProperty.Create(nameof(Threshold), typeof(double), typeof(MonthView), 50d);

    #endregion

    #region Events

    public event EventHandler<MonthChangedEventArgs> ActiveMonthChanged;

    #endregion

    #region Ctor

    public DatePicker()
    {
        InitializeComponent();

        if (Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
        {
            Application.Current.Resources["GlobalFlowDirection"] = FlowDirection.RightToLeft;
        }

        foreach (var item in Culture.Current.GetShortestDayNamesStartedFromFirstDayOfWeek(Culture.Current.DateTimeFormat.FirstDayOfWeek))
        {
            weekDays.Add(new Label()
            {
                Text = item,
                Style = (Style)this.FindResource("WeekDay")
            });
        }

        _monthViews = new Dictionary<int, IMonthView>();

        _panGestureRecognizer = new PanGestureRecognizer();
        _panGestureRecognizer.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(_panGestureRecognizer);

        _todayMonthView = MonthFactory.CreateMonthData(0, DateTime.Now.Date);

        _activeMonth = MonthFactory.CreateMonthView(0, DateTime.Now.Date);
        _monthViews[_activeMonth.ViewId] = MonthFactory.CreateMonthData(_activeMonth.ViewId, _activeMonth.SelectedDate);

        _prevMonth = MonthFactory.CreateMonthView(-1, DateTime.Now.Date.GetPrevMonthFirstDay());
        _monthViews[_prevMonth.ViewId] = MonthFactory.CreateMonthData(_prevMonth.ViewId, _prevMonth.SelectedDate);

        _nextMonth = MonthFactory.CreateMonthView(1, DateTime.Now.Date.GetNextMonthFirstDay());
        _monthViews[_nextMonth.ViewId] = MonthFactory.CreateMonthData(_nextMonth.ViewId, _nextMonth.SelectedDate);
    }

    #endregion

    #region Methods

    public IMonthView? GetMonthView(int viewId)
    {
        _monthViews.TryGetValue(viewId, out IMonthView? monthView);
        return monthView;
    }

    private void AddMonthView(MonthView monthView)
    {
        monthContainerBox.Add(monthView);

        //monthView.DayTapped += OnDayTapped;
        monthView.GestureRecognizers.Add(_panGestureRecognizer);
        foreach (var week in monthView.Weeks)
        {
            week.GestureRecognizers.Add(_panGestureRecognizer);
        }

        foreach (var day in monthView.Days)
        {
            day.GestureRecognizers.Add(_panGestureRecognizer);
        }
    }

    public bool ExistsMonthView(IMonthView monthView)
    {
        return _monthViews.ContainsKey(monthView.ViewId);
    }

    public async Task GoToNextMonth(DateTime? selctedDate = null)
    {
        if (Culture.Current.TextInfo.IsRightToLeft)
        {
            await Task.WhenAll(
                    SwipRight(_activeMonth),
                    SwipRight(_nextMonth));
        }
        else
        {
            await Task.WhenAll(
                    SwipLeft(_activeMonth),
                    SwipLeft(_nextMonth));
        }

        ActiveMonth = _nextMonth;
        if (selctedDate is not null) _activeMonth.SelectedDate = selctedDate.Value;
    }

    public async Task GoToPrevMonth(DateTime? selctedDate = null)
    {
        if (Culture.Current.TextInfo.IsRightToLeft)
        {
            await Task.WhenAll(
                    SwipLeft(_activeMonth),
                    SwipLeft(_prevMonth));
        }
        else
        {
            await Task.WhenAll(
                    SwipRight(_activeMonth),
                    SwipRight(_prevMonth));
        }

        ActiveMonth = _prevMonth;
        if (selctedDate is not null) ActiveMonth.SelectedDate = selctedDate.Value;
    }

    public void GoToday()
    {
        ActiveMonth = _todayMonthView;
    }

    protected virtual void OnPanRunning(object? sender, PanUpdatedEventArgs eventArgs)
    {
        if (_isHorizontalPan)
        {
            double totalX = 0;

            if (sender is MonthView || sender is WeekView || sender is DayView)
                totalX = eventArgs.TotalX + _appliedTotalXDiff;
            else totalX = eventArgs.TotalX;

            var toApplyXDiff = totalX - _appliedTotalXDiff;

            ChangeX(_activeMonth, toApplyXDiff);

            if (_appliedTotalXDiff > 0) ChangeX(_leftMonth, toApplyXDiff);
            else ChangeX(_rightMonth, toApplyXDiff);

            _appliedTotalXDiff += toApplyXDiff;
        }
    }

    protected virtual async Task OnPanCompleted(object? sender, PanUpdatedEventArgs eventArgs)
    {
        if (_isHorizontalPan)
        {
            var tasks = new List<Task>();

            if (_appliedTotalXDiff > 0) tasks.Add(ContinueOrPreventXChanging(_leftMonth));
            else tasks.Add(ContinueOrPreventXChanging(_rightMonth));

            tasks.Add(ContinueOrPreventXChanging(_activeMonth));

            await Task.WhenAll(tasks);

            if (AbsoluteLayout.GetLayoutBounds(_leftMonth).X == 0) ActiveMonth = _leftMonth;
            else if (AbsoluteLayout.GetLayoutBounds(_rightMonth).X == 0) ActiveMonth = _rightMonth;

            _appliedTotalXDiff = 0;
        }
    }

    public virtual async void OnPanUpdated(object? sender, PanUpdatedEventArgs eventArgs)
    {
        switch (eventArgs.StatusType)
        {
            case GestureStatus.Running:
                if (!_isHorizontalPan && eventArgs.IsVertical())
                {
                    _isVerticalPan = true;
                }
                else if (!_isVerticalPan && eventArgs.IsHorizontal())
                {
                    _isHorizontalPan = true;
                }
                OnPanRunning(sender, eventArgs);
                break;

            case GestureStatus.Completed:
                await OnPanCompleted(sender, eventArgs);
                _isVerticalPan = false;
                _isHorizontalPan = false;
                break;
        }
    }

    private void ChangeX(MonthView? monthView, double toApplyXDiff)
    {
        if (monthView is null) return;

        var lastBounds = AbsoluteLayout.GetLayoutBounds(monthView);
        var toApplyX = lastBounds.X + toApplyXDiff;
        var maximumX = _arrangedWidth;
        var minimumX = -maximumX;

        toApplyX = Math.Clamp(toApplyX, minimumX, maximumX);

        AbsoluteLayout.SetLayoutBounds(monthView, new Rect(toApplyX, lastBounds.Y, lastBounds.Width, lastBounds.Height));
    }

    public async Task SwipRight(MonthView monthView)
    {
        if (ActiveMonth == monthView)
        {

            await monthView.BoundsXTo(_arrangedWidth, Maui.DatePicker.Constants.DatePicker.AnimationsLength);
        }
        else
        {
            await monthView.BoundsXTo(0, Maui.DatePicker.Constants.DatePicker.AnimationsLength);
        }
    }

    public async Task SwipLeft(MonthView monthView)
    {
        if (ActiveMonth == monthView)
        {
            await monthView.BoundsXTo(-_arrangedWidth, Maui.DatePicker.Constants.DatePicker.AnimationsLength);
        }
        else
        {
            await monthView.BoundsXTo(0, Maui.DatePicker.Constants.DatePicker.AnimationsLength);
        }
    }

    private async Task PreventSwipping(MonthView monthView)
    {
        if (ActiveMonth == monthView)
        {
            await monthView.BoundsXTo(0, Maui.DatePicker.Constants.DatePicker.AnimationsLength);
        }
        else
        {
            if (AbsoluteLayout.GetLayoutBounds(monthView).X < 0)
            {
                await monthView.BoundsXTo(-_arrangedWidth, Maui.DatePicker.Constants.DatePicker.AnimationsLength);
            }
            else
            {
                await monthView.BoundsXTo(_arrangedWidth, Maui.DatePicker.Constants.DatePicker.AnimationsLength);
            }
        }

    }

    private async Task ContinueOrPreventXChanging(MonthView? monthView)
    {
        if (monthView is null) return;

        if (Math.Abs(_appliedTotalXDiff) > Threshold)
        {
            if (_appliedTotalXDiff > 0)
            {
                await SwipRight(monthView);
            }
            else
            {
                await SwipLeft(monthView);
            }
        }
        else
        {
            await PreventSwipping(monthView);
        }
    }

    private void RenderMontheViews()
    {
        if (!monthContainerBox.Children.Any())
        {
            foreach (var monthView in new[] { _activeMonth, _prevMonth, _nextMonth })
            {
                AddMonthView(monthView);
            }
            ActiveMonth = _activeMonth;
        }
    }

    protected override Size ArrangeOverride(Rect bounds)
    {
        var arrangedSize = base.ArrangeOverride(bounds);
        _arrangedWidth = arrangedSize.Width;
        RenderMontheViews();
        return arrangedSize;
    }

    private Task RenderPrevAndNextMonthes()
    {
        if (_isRenderingMonthes) return _renderingMonthesTask;

        _isRenderingMonthes = true;

        _renderingMonthesTask = Task.Run(render).ContinueWith((task) => { _isRenderingMonthes = false; });
        return _renderingMonthesTask;

        void render()
        {
            var currentMonth = ActiveMonth;

            for (int i = 0; i < Maui.DatePicker.Constants.DatePicker.GeneratedInVisibleMonthesCount; i++)
            {
                var nextMonthInList = GetMonthView(currentMonth.ViewId + 1);
                if (nextMonthInList is not null)
                {
                    currentMonth = nextMonthInList;
                    continue;
                }

                var nextMonth = MonthFactory.CreateMonthData(currentMonth.ViewId + 1, currentMonth.SelectedDate.GetNextMonthFirstDay());

                _monthViews[nextMonth.ViewId] = nextMonth;

                currentMonth = nextMonth;
            }

            currentMonth = ActiveMonth;

            for (int i = 0; i < Maui.DatePicker.Constants.DatePicker.GeneratedInVisibleMonthesCount; i++)
            {
                var prevMonthInList = GetMonthView(currentMonth.ViewId - 1);
                if (prevMonthInList is not null)
                {
                    currentMonth = prevMonthInList;
                    continue;
                }

                var prevMonth = MonthFactory.CreateMonthData(currentMonth.ViewId - 1, currentMonth.SelectedDate.GetPrevMonthFirstDay());

                _monthViews[prevMonth.ViewId] = prevMonth;

                currentMonth = prevMonth;
            }
        }
    }

    protected virtual async void OnActiveMonthChanged(IMonthView? deactivatedMonth, IMonthView? activatedMonth)
    {
        if (deactivatedMonth is MonthView deactivatedMonthView)
        {
            var deactivatedMonthData = GetMonthView(deactivatedMonthView.ViewId);
            if (deactivatedMonthData is not null)
            {
                deactivatedMonthData.Replace(deactivatedMonthView);
            }

            if (deactivatedMonthView.ViewId < activatedMonth?.ViewId)
            {
                _nextMonth = _prevMonth;
                _prevMonth = deactivatedMonthView;
            }
            else
            {
                _prevMonth = _nextMonth;
                _nextMonth = deactivatedMonthView;
            }
        }

        if (activatedMonth is not null)
        {
            if (activatedMonth is MonthView activatedMonthView)
            {
                var prevBounds = AbsoluteLayout.GetLayoutBounds(activatedMonthView);
                AbsoluteLayout.SetLayoutBounds(activatedMonthView, new Rect(0, prevBounds.Y, prevBounds.Width, prevBounds.Height));

                _activeMonth = activatedMonthView;
            }
            else
            {
                var prevBounds = AbsoluteLayout.GetLayoutBounds(_activeMonth);
                AbsoluteLayout.SetLayoutBounds(_activeMonth, new Rect(0, prevBounds.Y, prevBounds.Width, prevBounds.Height));
                _activeMonth.Replace(activatedMonth);

            }

            await ReplaceMonthViewByNewViewId(_prevMonth, activatedMonth.ViewId - 1);
            await ReplaceMonthViewByNewViewId(_nextMonth, activatedMonth.ViewId + 1);

            if (!ExistsMonthView(activatedMonth))
            {
                _monthViews[activatedMonth.ViewId] = activatedMonth;
            }

            RenderPrevAndNextMonthes();
        }

        ActiveMonthChanged?.Invoke(this, new MonthChangedEventArgs(deactivatedMonth, activatedMonth));
    }

    private async ValueTask ReplaceMonthViewByNewViewId(MonthView monthView, int viewId)
    {
        var prevBounds = AbsoluteLayout.GetLayoutBounds(monthView);
        var toApplyX = 0d;

        if (Culture.Current.TextInfo.IsRightToLeft)
        {
            toApplyX = _activeMonth.ViewId < viewId ? -_arrangedWidth : _arrangedWidth;
        }
        else
        {
            toApplyX = _activeMonth.ViewId > viewId ? -_arrangedWidth : _arrangedWidth;
        }

        AbsoluteLayout.SetLayoutBounds(monthView, new Rect(toApplyX, prevBounds.Y, prevBounds.Width, prevBounds.Height));

        if (monthView.ViewId == viewId) return;

        var monthData = GetMonthView(viewId);
        if (monthData is null)
        {
            await RenderPrevAndNextMonthes();
            monthData = GetMonthView(viewId);
        }

        monthView.Replace(monthData);
    }

    #endregion
}