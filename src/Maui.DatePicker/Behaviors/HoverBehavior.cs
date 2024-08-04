namespace Maui.DatePicker.Behaviors
{
    public class HoverBehavior : Behavior<VisualElement>
    {
        Lazy<PointerGestureRecognizer> PointerGesture = new Lazy<PointerGestureRecognizer>(() =>
        {
            var pointer = new PointerGestureRecognizer();
            pointer.PointerEntered += PointerEntered;
            pointer.PointerExited += PointerExited;

            return pointer;
        });

        protected override void OnAttachedTo(VisualElement bindable)
        {
            if (bindable is IGestureRecognizers gestures)
            {
                gestures.GestureRecognizers.Add(PointerGesture.Value);
            }
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            if (bindable is IGestureRecognizers gestures)
            {
                gestures.GestureRecognizers.Remove(PointerGesture.Value);
            }
            base.OnDetachingFrom(bindable);
        }


        static void PointerEntered(object? sender, PointerEventArgs eventArgs)
        {
            var element = (VisualElement)sender;

            if (element.BackgroundColor == null || element.BackgroundColor == Colors.Transparent) element.BackgroundColor = Colors.White;

            element.BackgroundColor = new Color(element.BackgroundColor.Red - .08f, element.BackgroundColor.Green - .08f, element.BackgroundColor.Blue - .08f);
        }

        static void PointerExited(object? sender, PointerEventArgs eventArgs)
        {
            var element = (VisualElement)sender;

            element.BackgroundColor = new Color(element.BackgroundColor.Red + .08f, element.BackgroundColor.Green + .08f, element.BackgroundColor.Blue + .08f);
        }
    }
}
