using System;

namespace CFB.Common.Utilities
{
    public static class DateTimeHelper
    {
        public static string ToIsoFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
    }
}
