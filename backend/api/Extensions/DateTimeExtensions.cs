namespace FakeRestuarantAPI.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Converts DateTime to Unix timestamp
    /// </summary>
    public static long ToUnixTimestamp(this DateTime dateTime)
    {
        return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
    }

    /// <summary>
    /// Converts Unix timestamp to DateTime
    /// </summary>
    public static DateTime FromUnixTimestamp(this long unixTimestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
    }

    /// <summary>
    /// Gets the start of the day (00:00:00)
    /// </summary>
    public static DateTime StartOfDay(this DateTime dateTime)
    {
        return dateTime.Date;
    }

    /// <summary>
    /// Gets the end of the day (23:59:59.999)
    /// </summary>
    public static DateTime EndOfDay(this DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddMilliseconds(-1);
    }

    /// <summary>
    /// Checks if date is today
    /// </summary>
    public static bool IsToday(this DateTime dateTime)
    {
        return dateTime.Date == DateTime.Today;
    }

    /// <summary>
    /// Checks if date is in the past
    /// </summary>
    public static bool IsInPast(this DateTime dateTime)
    {
        return dateTime < DateTime.Now;
    }

    /// <summary>
    /// Checks if date is in the future
    /// </summary>
    public static bool IsInFuture(this DateTime dateTime)
    {
        return dateTime > DateTime.Now;
    }

    /// <summary>
    /// Gets a human-readable time ago string
    /// </summary>
    public static string TimeAgo(this DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;

        if (timeSpan.TotalDays > 365)
            return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) == 1 ? "" : "s")} ago";
        
        if (timeSpan.TotalDays > 30)
            return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) == 1 ? "" : "s")} ago";
        
        if (timeSpan.TotalDays > 1)
            return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays == 1 ? "" : "s")} ago";
        
        if (timeSpan.TotalHours > 1)
            return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours == 1 ? "" : "s")} ago";
        
        if (timeSpan.TotalMinutes > 1)
            return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes == 1 ? "" : "s")} ago";
        
        return "Just now";
    }
}