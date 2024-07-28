using Maui.DatePicker.EventArgs;
using Maui.DatePicker.Interfaces;

namespace Maui.DatePicker
{
    public class MonthView : VerticalStackLayout, IMonthView
    {
        #region Fields

        private WeekView[] _weeks;
        private DayView[] _days;

        #endregion

        #region Properties

        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public double Threshold
        {
            get => (double)GetValue(ThresholdProperty);
            set => SetValue(ThresholdProperty, value);
        }

        public int ViewId { get; set; }

        public IReadOnlyList<WeekView> Weeks => _weeks;

        public IReadOnlyList<DayView> Days => _days;

        IReadOnlyList<IWeekView> IMonthView.Weeks => _weeks;

        IReadOnlyList<IDayView> IMonthView.Days => _days;

        #endregion

        #region Events

        public event EventHandler<SelectedDateChangedEventArgs> SelectedDateChanged;

        public event EventHandler<TappedEventArgs> DayTapped;

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty SelectedDateProperty =
            BindableProperty.Create(nameof(SelectedDate), typeof(DateTime), typeof(MonthView), DateTime.Now.Date,
            propertyChanged: (bindable, oldValue, newValue) => ((MonthView)bindable).OnSelectedDateChanged((DateTime)oldValue, (DateTime)newValue));

        public static readonly BindableProperty ThresholdProperty =
            BindableProperty.Create(nameof(Threshold), typeof(double), typeof(MonthView), 50d);

        #endregion

        #region Ctor

        public MonthView() : this(0, DateTime.Now.Date)
        {

        }

        public MonthView(int viewId, DateTime selectedDate)
        {
            ViewId = viewId;
            SelectedDate = selectedDate;

            RenderView();
        }

        #endregion

        #region Methods

        protected virtual void RenderView()
        {
            (_weeks, _days) = MonthFactory.CreateWeekAndDays(SelectedDate);

            int selectedWeekIndex = 0;
            int weekIndex = 0;
            foreach (var currentWeek in _weeks)
            {
                if (currentWeek.IsSelected) selectedWeekIndex = weekIndex;
                Add(currentWeek);
                weekIndex++;
            }

            foreach (var currentDay in _days)
            {
                currentDay.Tapped += OnDayTapped;
            }
        }

        protected virtual void OnDayTapped(object? sender, TappedEventArgs eventArgs)
        {
            if (sender is DayView day)
            {
                if (!day.IsDisable) SelectedDate = day.DateTime;

                DayTapped?.Invoke(sender, eventArgs);
            }

        }

        protected virtual void OnSelectedDateChanged(DateTime oldValue, DateTime newValue)
        {
            var oldDay = _days?.FirstOrDefault(d => d.DateTime.Date == oldValue.Date);
            if (oldDay is not null)
            {
                oldDay.IsSelected = false;

                var oldWeek = _weeks.FirstOrDefault(w => w.Days.Contains(oldDay));
                if (oldWeek is not null) oldWeek.IsSelected = false;
            }

            var newDay = _days?.FirstOrDefault(d => d.DateTime.Date == newValue.Date);
            if (newDay is not null)
            {
                newDay.IsSelected = true;

                var newWeek = _weeks.FirstOrDefault(w => w.Days.Contains(newDay));
                if (newWeek is not null)
                {
                    newWeek.IsSelected = true;
                }
            }

            SelectedDateChanged?.Invoke(this, new SelectedDateChangedEventArgs(oldValue, newValue));
        }

        public void Replace(IMonthView newView)
        {
            for (int i = 0; i < Constants.DatePicker.NumberOfWeeksInMonth; i++)
            {
                _weeks[i].Replace(newView.Weeks[i]);
            }

            for (int i = 0; i < Constants.DatePicker.NumberOfDaysInMonth; i++)
            {
                _days[i].Replace(newView.Days[i]);
            }

            SelectedDate = newView.SelectedDate;
            Threshold = newView.Threshold;
            ViewId = newView.ViewId;
        }

        #endregion
    }
}
