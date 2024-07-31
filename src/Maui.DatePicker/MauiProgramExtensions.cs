using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.DatePicker
{
    public static class MauiProgramExtensions
    {
        public static MauiAppBuilder UseMauiDatePicker(this MauiAppBuilder builder)
        {
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddEmbeddedResourceFont(AssemblyReference.Assembly, "MaterialIconsOutlined-Regular.otf", "MaterialIconsOutlined");
            });

            return builder;
        }
    }
}
