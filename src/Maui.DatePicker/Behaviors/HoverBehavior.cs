using Maui.DatePicker.Animations;
using Maui.DatePicker.Extensions;
using Maui.DatePicker.Helpers;
using Microsoft.Maui.Graphics.Platform;
using System.ComponentModel;

namespace Maui.DatePicker.Behaviors
{
    public class HoverBehavior : Behavior<VisualElement>
    {
        Color _originBackgroundColor;
        Color _hoveredBackgroundColor;
        PointerGestureRecognizer _pointerGestureRecognizer;
        TapGestureRecognizer _tapGestureRecognizer;
        bool _bgChanging;
        bool _tapping;
        bool _isHovered;
        VisualElement _element;


        public HoverBehavior()
        {
            _pointerGestureRecognizer = new PointerGestureRecognizer();
            _pointerGestureRecognizer.PointerEntered += OnPointerEntered;
            _pointerGestureRecognizer.PointerExited += OnPointerExited;

            _tapGestureRecognizer = new TapGestureRecognizer();
            _tapGestureRecognizer.Tapped += OnTapped;
        }

        protected override void OnAttachedTo(VisualElement bindable)
        {
            _element = bindable;
            if (bindable is IGestureRecognizers gesture)
            {
                gesture.GestureRecognizers.Add(_pointerGestureRecognizer);
                gesture.GestureRecognizers.Add(_tapGestureRecognizer);
            }
            _originBackgroundColor = bindable.BackgroundColor;
            bindable.PropertyChanged += OnPropertyChanged;
            bindable.Unloaded += (s, e) => OnPointerExited(bindable, new PointerEventArgs());

            if(Application.Current != null)
                Application.Current.RequestedThemeChanged += OnThemeChanged;

            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            if (Application.Current != null)
                Application.Current.RequestedThemeChanged -= OnThemeChanged;

            if (bindable is IGestureRecognizers gestures)
            {
                gestures.GestureRecognizers.Remove(_pointerGestureRecognizer);
                gestures.GestureRecognizers.Remove(_tapGestureRecognizer);
            }

            _element = null;
            base.OnDetachingFrom(bindable);
        }

        void OnThemeChanged(object? sender, System.EventArgs eventArgs)
        {
            _element.BackgroundColor = Application.Current?.RequestedTheme == AppTheme.Light
                    ? Constants.Calendar.CalendarGenericLightBackgroundColor
                    : Constants.Calendar.CalendarGenericDarkBackgroundColor;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!_bgChanging && e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                _originBackgroundColor = ((VisualElement)sender).BackgroundColor;
            }
        }

        async void OnTapped(object? sender, TappedEventArgs eventArgs)
        {
            var element = (VisualElement)sender;

            var bgColor = Application.Current?.RequestedTheme == AppTheme.Light
                    ? Constants.Calendar.CalendarGenericLightBackgroundColor
                    : Constants.Calendar.CalendarGenericDarkBackgroundColor;

            if (element.BackgroundColor == null || element.BackgroundColor == Colors.Transparent) element.BackgroundColor = bgColor;

            lock (sender)
            {
                if (_tapping)
                {
                    element.CancelColorToAnimation();
                }
            }

            _tapping = true;
            _bgChanging = true;

            var lastBgColor = _isHovered ? _hoveredBackgroundColor : _originBackgroundColor;

            var toColor = Application.Current?.RequestedTheme == AppTheme.Light ? lastBgColor.Darker() : lastBgColor.Lighter();

            var cancelled = await element.ColorTo(lastBgColor,
                                               toColor,
                                               v => element.BackgroundColor = v,
                                               100);
            if (!cancelled)
            {
                await element.ColorTo(element.BackgroundColor,
                                      lastBgColor,
                                      v => element.BackgroundColor = v,
                                      100);
            }

            _tapping = false;
            _bgChanging = false;
        }

        void OnPointerEntered(object? sender, PointerEventArgs eventArgs)
        {
            var element = (VisualElement)sender;

            var bgColor = Application.Current?.RequestedTheme == AppTheme.Light
                    ? Constants.Calendar.CalendarGenericLightBackgroundColor
                    : Constants.Calendar.CalendarGenericDarkBackgroundColor;

            if (element.BackgroundColor == null || element.BackgroundColor == Colors.Transparent) element.BackgroundColor = bgColor;

            _bgChanging = true;
            _isHovered = true;

            var toColor = Application.Current?.RequestedTheme == AppTheme.Light ? element.BackgroundColor.Darker() : element.BackgroundColor.Lighter();

            element.BackgroundColor = toColor;

            _hoveredBackgroundColor = element.BackgroundColor;
            _bgChanging = false;
        }

        void OnPointerExited(object? sender, PointerEventArgs eventArgs)
        {
            var element = (VisualElement)sender;

            _bgChanging = true;

            element.CancelColorToAnimation();
            element.BackgroundColor = _originBackgroundColor;

            _isHovered = false;
            _bgChanging = false;
        }
    }
}
