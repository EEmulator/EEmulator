using System;

namespace EEmulatorV3.helpers
{
    public static class DateTimeExtensions
    {
        internal static DateTime FromUnixTime(this long input) => new DateTime(1970, 1, 1).AddMilliseconds((long)input);
        internal static long ToUnixTime(this DateTime input) => (long)((input - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
    }
}
