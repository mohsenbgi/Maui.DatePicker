namespace Maui.DatePicker.Behaviors
{
    public class HoverBehavior : Behavior<VisualElement>
    {
        Color _originBackgroundColor;
        PointerGestureRecognizer _pointerGestureRecognizer;


        public HoverBehavior()
        {
            _pointerGestureRecognizer = new PointerGestureRecognizer();
            _pointerGestureRecognizer.PointerEntered += PointerEntered;
            _pointerGestureRecognizer.PointerExited += PointerExited;
        }

        protected override void OnAttachedTo(VisualElement bindable)
        {
            if (bindable is IGestureRecognizers gesture)
            {
                gesture.GestureRecognizers.Add(_pointerGestureRecognizer);
            }
            _originBackgroundColor = bindable.BackgroundColor;
            bindable.Unloaded += (s, e) => PointerExited(bindable, new PointerEventArgs());

            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            if (bindable is IGestureRecognizers gestures)
            {
                gestures.GestureRecognizers.Remove(_pointerGestureRecognizer);
            }
            base.OnDetachingFrom(bindable);
        }


        void PointerEntered(object? sender, PointerEventArgs eventArgs)
        {
            var element = (VisualElement)sender;

            if (element.BackgroundColor == null || element.BackgroundColor == Colors.Transparent) element.BackgroundColor = Colors.White;

            element.BackgroundColor = new Color(element.BackgroundColor.Red - .04f, element.BackgroundColor.Green - .04f, element.BackgroundColor.Blue - .04f);
        }

        void PointerExited(object? sender, PointerEventArgs eventArgs)
        {
            var element = (VisualElement)sender;

            element.BackgroundColor = _originBackgroundColor;
        }
    }
}
