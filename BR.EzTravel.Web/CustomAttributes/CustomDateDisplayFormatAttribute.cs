using System.ComponentModel.DataAnnotations;
using BR.EzTravel.Web.Properties;

namespace BR.EzTravel.Web.CustomAttributes
{
    public class CustomDateDisplayFormatAttribute : DisplayFormatAttribute
    {
        public CustomDateDisplayFormatAttribute()
        {
            DataFormatString = Settings.Default.DateFormat;
        }
    }
}
