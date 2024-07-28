namespace Maui.DatePicker.Helpers
{
    public static class ResourceHelper
    {
        public static object? FindResource(this VisualElement? o, string key)
        {
            while (o != null)
            {
                if (o.Resources.TryGetValue(key, out object r1)) return r1;
                if (o is Page) break;
                if (o is IElement e) o = e.Parent as VisualElement;
            }

            if (Application.Current?.Resources.TryGetValue(key, out object r2) ?? false) return r2;

            foreach (var resource in Application.Current?.Resources.MergedDictionaries ?? Array.Empty<ResourceDictionary>())
            {
                if (resource.TryGetValue(key, out object r3)) return r3;
            }

            return null;
        }

        public static Color GetColor(string key, Color? fallBack = default)
        {
            if (FindColor(key, out var color))
            {
                return (Color)color;
            }
            else
            {
                return fallBack ?? Colors.Transparent;
            }
        }

        public static Color GetColor(string lightKey, string darkKey, Color? fallBack = default)
        {
            if (Application.Current is null)
            {
                return fallBack ?? Colors.Transparent;
            }

            var key = Application.Current.RequestedTheme == AppTheme.Light ? lightKey : darkKey;

            if (FindColor(key, out var color))
            {
                return (Color)color;
            }
            else
            {
                return fallBack ?? Colors.Transparent;
            }
        }

        static bool FindColor(string key, out object value)
        {
            bool result = false;

            if (Application.Current?.Resources.TryGetValue(key, out value) ?? false) result = true;
            foreach (var resource in Application.Current?.Resources.MergedDictionaries ?? Array.Empty<ResourceDictionary>())
            {
                if (resource.TryGetValue(key, out value)) result = true;
            }

            value = Colors.Transparent;
            return result;
        }

    }
}
