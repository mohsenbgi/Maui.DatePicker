namespace Maui.DatePicker.Animations
{
    public static class BoundsAnimation
    {
        #region BoundsHeightTo

        public static Task<bool> BoundsHeightTo(this VisualElement self, double toBoundsHeight, uint length = 250, Easing? easing = null)
        {
            easing = easing ?? Easing.Linear;

            var currentLayoutBounds = AbsoluteLayout.GetLayoutBounds(self);
            var taskCompletionSource = new TaskCompletionSource<bool>();

            self.Animate("BoundsHeightTo",
                        (v) => AbsoluteLayout.SetLayoutBounds(self, new Rect(currentLayoutBounds.X, currentLayoutBounds.Y, currentLayoutBounds.Width, v)),
                        currentLayoutBounds.Height,
                        toBoundsHeight,
                        16,
                        length,
                        easing,
                        (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }

        public static void CancelBoundsHeightToAnimation(this VisualElement self)
        {
            self.AbortAnimation("BoundsHeightTo");
        }

        #endregion

        #region BoundsXTo

        public static Task<bool> BoundsXTo(this VisualElement self, double toBoundsX, uint length = 250, Easing? easing = null)
        {
            easing = easing ?? Easing.Linear;

            var currentLayoutBounds = AbsoluteLayout.GetLayoutBounds(self);
            var taskCompletionSource = new TaskCompletionSource<bool>();

            self.Animate("BoundsXTo",
                        (v) =>
                        {
                            AbsoluteLayout.SetLayoutBounds(self, new Rect(v, currentLayoutBounds.Y, currentLayoutBounds.Width, currentLayoutBounds.Height));
                        },
                        currentLayoutBounds.X,
                        toBoundsX,
                        16,
                        length,
                        easing,
                        (v, c) =>
                        {
                            taskCompletionSource.SetResult(c);
                        });

            return taskCompletionSource.Task;
        }

        public static void CancelBoundsXToAnimation(this VisualElement self)
        {
            self.AbortAnimation("BoundsXTo");
        }

        #endregion
    }


}
