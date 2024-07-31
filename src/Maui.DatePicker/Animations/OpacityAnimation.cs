namespace Maui.DatePicker.Animations
{
    public static class OpacityAnimation
    {
        public static Task<bool> OpacityTo(this VisualElement self, double fromOpacity, double toOpacity, uint length = 250, Easing? easing = null)
        {
            easing = easing ?? Easing.Linear;

            var currentLayoutBounds = AbsoluteLayout.GetLayoutBounds(self);
            var taskCompletionSource = new TaskCompletionSource<bool>();

            self.Animate("OpacityTo",
                        (v) => self.Opacity = v,
                        fromOpacity,
                        toOpacity,
                        16,
                        length,
                        easing,
                        (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }

        public static void CancelOpacityToAnimation(this VisualElement self)
        {
            self.AbortAnimation("OpacityTo");
        }

    }
}
