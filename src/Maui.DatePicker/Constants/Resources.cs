using Maui.DatePicker.Enums;

namespace Maui.DatePicker.Constants
{
    public static class Resources
    {
        public static string Ok => "Ok";
        public static string Cancel => "Cancel";
        public static string Today => "Today";

        public static Dictionary<string, string> EnglishResources = new Dictionary<string, string>
        {
            { Ok, "Ok" },
            { Cancel, "Cancel" },
            { Today, "Today" },
        };

        public static Dictionary<string, string> PersianResources = new Dictionary<string, string>
        {
            { Ok, "تایید" },
            { Cancel, "انصراف" },
            { Today, "امروز" },
        };

        public static string GetStringValue(string key)
        {
            return Config.Language switch
            {
                Language.English => EnglishResources[key],
                Language.Persian => PersianResources[key],
            };
        }
    }
}
