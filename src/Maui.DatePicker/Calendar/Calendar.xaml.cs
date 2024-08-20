using Maui.DatePicker.Animations;
using Maui.DatePicker.Constants;
using Maui.DatePicker.Enums;
using Maui.DatePicker.EventArgs;
using Maui.DatePicker.Extensions;
using Maui.DatePicker.Helpers;
using Maui.DatePicker.Interfaces;

namespace Maui.DatePicker.Calendar;

public partial class Calendar : Grid
{
    #region Fields

    bool _isRenderingMonths;
    Task _renderingMonthsTask;
    readonly PanGestureRecognizer _panGestureRecognizer;
    readonly Dictionary<int, IMonthView> _monthViews;
    bool _isHorizontalPan;
    bool _isVerticalPan;
    double _appliedTotalXDiff;
    readonly IMonthView _todayMonthView;
    MonthView _prevMonth;
    MonthView _nextMonth;
    MonthView _activeMonth;
    MonthView _leftMonth => Config.Language.IsRightToLeft() ? _nextMonth : _prevMonth;
    MonthView _rightMonth => Config.Language.IsRightToLeft() ? _prevMonth : _nextMonth;

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
        typeof(Calendar),
        null,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is Calendar calendarView)
            {
                calendarView.OnActiveMonthChanged((IMonthView?)oldValue, (IMonthView?)newValue);
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

    public Calendar()
    {
        InitializeComponent();

        Resources["GlobalFlowDirection"] = Config.Language.GetDirection();

        foreach (var item in Config.Language.GetCulture().GetAbbreviatedDayNamesStartedFromFirstDayOfWeek(Config.Language.GetCulture().DateTimeFormat.FirstDayOfWeek))
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

    public void OnLanguageChanged(Language oldValue, Language newValue)
    {
        var flowDirection = newValue.GetDirection();
        Resources["GlobalFlowDirection"] = flowDirection;
    }

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

    public async Task GoToRightMonth(DateTime? selectedDate = null)
    {
        await Task.WhenAll(
                SwipeLeft(_activeMonth),
                SwipeLeft(_rightMonth));
     
        ActiveMonth = _rightMonth;
        if (selectedDate is not null) _activeMonth.SelectedDate = selectedDate.Value;
    }

    public async Task GoToLeftMonth(DateTime? selectedDate = null)
    {    
        await Task.WhenAll(
                SwipeRight(_activeMonth),
                SwipeRight(_leftMonth));
     
        ActiveMonth = _leftMonth;
        if (selectedDate is not null) ActiveMonth.SelectedDate = selectedDate.Value;
    }

    public void GoToday()
    {
        ActiveMonth = _todayMonthView;
        ActiveMonth = _activeMonth;
    }

    public void GoToDate(DateTime date)
    {
        var differentTime = (date - ActiveMonth.SelectedDate);

        DateTime startDate = DateTime.MinValue;
        DateTime endDate = startDate.Add(differentTime.Duration());

        int totalMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;

        if (differentTime.Ticks < 0) totalMonths = -totalMonths;

        var expectedViewId = ActiveMonth.ViewId + totalMonths;
        var expectedMonthData = GetMonthView(expectedViewId);
        if(expectedMonthData is null)
        {
            expectedMonthData = MonthFactory.CreateMonthData(expectedViewId, date);
            _monthViews[expectedMonthData.ViewId] = expectedMonthData;
        }

        ActiveMonth = expectedMonthData;
        ActiveMonth = _activeMonth;
    }

    protected virtual void OnPanRunning(object? sender, PanUpdatedEventArgs eventArgs)
    {
        if (_isHorizontalPan)
        {
            double totalX = 0;

            if (sender is MonthView || sender is WeekView || sender is DayView)
                totalX = eventArgs.TotalX + _appliedTotalXDiff;
            else totalX = eventArgs.TotalX;

            var maximumX = Width;
            var minimumX = -maximumX;
            totalX = Math.Clamp(totalX, minimumX, maximumX);

            var toApplyXDiff = totalX - _appliedTotalXDiff;

            #if WINDOWS
            if (Config.Language.IsRightToLeft())
            {
                toApplyXDiff = -toApplyXDiff;
            }
            #endif

            _activeMonth.TranslationX = Math.Clamp(_activeMonth.TranslationX + toApplyXDiff, minimumX, maximumX);

            if (Config.Language.IsRightToLeft() && DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                _leftMonth.TranslationX = _activeMonth.TranslationX + Width;
                _rightMonth.TranslationX = _activeMonth.TranslationX - Width;
            }
            else
            {
                _leftMonth.TranslationX = _activeMonth.TranslationX - Width;
                _rightMonth.TranslationX = _activeMonth.TranslationX + Width;
            }

            #if WINDOWS
            if (Config.Language.IsRightToLeft())
            {
                toApplyXDiff = -toApplyXDiff;
            }
            #endif

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

            if (_leftMonth.TranslationX == 0) ActiveMonth = _leftMonth;
            else if (_rightMonth.TranslationX == 0) ActiveMonth = _rightMonth;

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

    public async Task SwipeRight(MonthView monthView)
    {
        if (ActiveMonth == monthView)
        {
            if (Config.Language.IsRightToLeft() && DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                await monthView.TranslateTo(-Width, 0, Maui.DatePicker.Constants.Calendar.AnimationsLength);
            }
            else
            {
                await monthView.TranslateTo(Width, 0, Maui.DatePicker.Constants.Calendar.AnimationsLength);
            }
        }
        else
        {
            await monthView.TranslateTo(0, 0, Maui.DatePicker.Constants.Calendar.AnimationsLength);
        }
    }

    public async Task SwipeLeft(MonthView monthView)
    {
        if (ActiveMonth == monthView)
        {
            if (Config.Language.IsRightToLeft() && DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                await monthView.TranslateTo(Width, 0, Maui.DatePicker.Constants.Calendar.AnimationsLength);
            }
            else
            {
                await monthView.TranslateTo(-Width, 0, Maui.DatePicker.Constants.Calendar.AnimationsLength);
            }
        }
        else
        {
            await monthView.TranslateTo(0, 0, Maui.DatePicker.Constants.Calendar.AnimationsLength);
        }
    }

    private async Task PreventSwiping(MonthView monthView)
    {
        if (ActiveMonth == monthView)
        {
            await monthView.TranslateTo(0, 0, Maui.DatePicker.Constants.Calendar.AnimationsLength);
        }
        else
        {
            if (monthView.TranslationX < 0)
            {
                await monthView.TranslateTo(-Width, 0, Maui.DatePicker.Constants.Calendar.AnimationsLength);
            }
            else
            {
                await monthView.TranslateTo(Width, 0, Maui.DatePicker.Constants.Calendar.AnimationsLength);
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
                await SwipeRight(monthView);
            }
            else
            {
                await SwipeLeft(monthView);
            }
        }
        else
        {
            await PreventSwiping(monthView);
        }
    }

    private void RenderMonthViews()
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

        RenderMonthViews();
        return arrangedSize;
    }

    private Task RenderPrevAndNextMonths()
    {
        if (_isRenderingMonths) return _renderingMonthsTask;

        _isRenderingMonths = true;

        _renderingMonthsTask = Task.Run(render).ContinueWith((task) => { _isRenderingMonths = false; });
        return _renderingMonthsTask;

        void render()
        {
            var currentMonth = ActiveMonth;

            for (int i = 0; i < Maui.DatePicker.Constants.Calendar.GeneratedInVisibleMonthsCount; i++)
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

            for (int i = 0; i < Maui.DatePicker.Constants.Calendar.GeneratedInVisibleMonthsCount; i++)
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
        if (deactivatedMonth is MonthView deactivatedMonthView && activatedMonth is not MonthData)
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
                activatedMonthView.TranslationX = 0;
                _activeMonth = activatedMonthView;
            }
            else
            {
                _activeMonth.TranslationX = 0;
                _activeMonth.Replace(activatedMonth);
                
            }

            await ReplaceMonthViewByNewViewId(_prevMonth, activatedMonth.ViewId - 1);
            await ReplaceMonthViewByNewViewId(_nextMonth, activatedMonth.ViewId + 1);

            if (!ExistsMonthView(activatedMonth))
            {
                _monthViews[activatedMonth.ViewId] = activatedMonth;
            }

            RenderPrevAndNextMonths();
        }

        ActiveMonthChanged?.Invoke(this, new MonthChangedEventArgs(deactivatedMonth, activatedMonth));
    }

    private async ValueTask ReplaceMonthViewByNewViewId(MonthView monthView, int viewId)
    {
        var toApplyX = 0d;

        if (Config.Language.IsRightToLeft() && DeviceInfo.Current.Platform != DevicePlatform.WinUI)
        {
            toApplyX = _activeMonth.ViewId < viewId ? -Width : Width;
        }
        else
        {
            toApplyX = _activeMonth.ViewId > viewId ? -Width : Width;
        }

        monthView.TranslationX = toApplyX;

        if (monthView.ViewId == viewId) return;

        var monthData = GetMonthView(viewId);
        if (monthData is null)
        {
            await RenderPrevAndNextMonths();
            monthData = GetMonthView(viewId);
        }

        monthView.Replace(monthData);
    }

#endregion
}