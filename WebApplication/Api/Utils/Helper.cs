using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Api.Utils
{
    public static class Helper
    {
        public static DateTimeOffset? FromUnixTime(long? unixTime)
        {
            if (unixTime.HasValue)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return new DateTimeOffset(epoch.AddMilliseconds(unixTime.Value));
            }
            else { return null; }
        }

        public static long ToUnixTime(DateTimeOffset dateTimeOffset)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((dateTimeOffset.ToUniversalTime() - epoch).TotalMilliseconds);
        }
    }
}