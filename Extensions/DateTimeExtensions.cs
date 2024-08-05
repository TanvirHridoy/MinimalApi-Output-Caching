namespace MinimalApi.Extensions;

public static class DateTimeExtensions
{
    public static DateTime? ConvertToTimeZone(this DateTime? utcDateTime, string timeZoneId)
    {
        if (utcDateTime.HasValue)
        {
            if (utcDateTime?.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("The DateTime must be in UTC.", nameof(utcDateTime));
            }

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.Value, timeZone);
        }
        return utcDateTime;
    }
}
