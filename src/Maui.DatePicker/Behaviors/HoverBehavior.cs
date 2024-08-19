using Maui.DatePicker.Animations;
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
            if (bindable is IGestureRecognizers gesture)
            {
                gesture.GestureRecognizers.Add(_pointerGestureRecognizer);
                gesture.GestureRecognizers.Add(_tapGestureRecognizer);
            }
            _originBackgroundColor = bindable.BackgroundColor;
            bindable.PropertyChanged += OnPropertyChanged;
            bindable.Unloaded += (s, e) => OnPointerExited(bindable, new PointerEventArgs());

            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            if (bindable is IGestureRecognizers gestures)
            {
                gestures.GestureRecognizers.Remove(_pointerGestureRecognizer);
                gestures.GestureRecognizers.Remove(_tapGestureRecognizer);
            }
            base.OnDetachingFrom(bindable);
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

            if (element.BackgroundColor == null || element.BackgroundColor == Colors.Transparent) element.BackgroundColor = Colors.White;

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

            var cancelled = await element.ColorTo(lastBgColor,
                                               new Color(lastBgColor.Red - .04f, lastBgColor.Green - .04f, lastBgColor.Blue - .04f),
                                               v => element.BackgroundColor = v,
                                               200);
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

            if (element.BackgroundColor == null || element.BackgroundColor == Colors.Transparent) element.BackgroundColor = Colors.White;

            _bgChanging = true;
            _isHovered = true;

            element.BackgroundColor = new Color(element.BackgroundColor.Red - .04f, element.BackgroundColor.Green - .04f, element.BackgroundColor.Blue - .04f);

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
