using Maui.DatePicker.Enums;
using System.Globalization;

namespace Maui.DatePicker.Extensions;

public static class LanguageExtensions
{
    public static System.Globalization.Calendar GetCalendar(this Language language)
    {
        return language.GetCulture().Calendar;
    }

    public static bool IsRightToLeft(this Language language)
    {
        return language.GetDirection() == FlowDirection.RightToLeft;
    }

    public static bool IsLeftToRight(this Language language)
    {
        return language.GetDirection() == FlowDirection.LeftToRight;
    }

    public static CultureInfo GetCulture(this Language language)
    {
        return language switch
        {
            Language.Persian => new PersianCulture(),
            Language.English => new CultureInfo("en-US"),
            _ => new CultureInfo("en-US")
        };
    }

    public static FlowDirection GetDirection(this Language language)
    {
        return language switch
        {
            Language.Persian => FlowDirection.RightToLeft,
            Language.English => FlowDirection.LeftToRight,
            _ => FlowDirection.LeftToRight
        };
    }
}
