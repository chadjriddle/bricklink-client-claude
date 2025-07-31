namespace BrickLink.Client.Auth;

/// <summary>
/// Provides Unix timestamp generation for OAuth 1.0a authentication.
/// The timestamp represents the number of seconds since January 1, 1970 00:00:00 UTC.
/// </summary>
public static class TimestampGenerator
{
    /// <summary>
    /// The Unix epoch start time (January 1, 1970 00:00:00 UTC).
    /// </summary>
    private static readonly DateTimeOffset UnixEpoch = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// Generates a Unix timestamp representing the current UTC time.
    /// </summary>
    /// <returns>
    /// The number of seconds since January 1, 1970 00:00:00 UTC as a string.
    /// </returns>
    /// <remarks>
    /// The timestamp is calculated using <see cref="DateTimeOffset.UtcNow"/> to ensure
    /// consistency across different time zones and system configurations.
    /// </remarks>
    public static string Generate()
    {
        return Generate(DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Generates a Unix timestamp for the specified date and time.
    /// </summary>
    /// <param name="dateTime">The date and time to convert to a Unix timestamp.</param>
    /// <returns>
    /// The number of seconds since January 1, 1970 00:00:00 UTC as a string.
    /// </returns>
    /// <remarks>
    /// The input <paramref name="dateTime"/> is used as-is since DateTimeOffset
    /// already handles UTC offset correctly.
    /// </remarks>
    public static string Generate(DateTimeOffset dateTime)
    {
        var totalSeconds = (long)(dateTime - UnixEpoch).TotalSeconds;
        return totalSeconds.ToString();
    }

    /// <summary>
    /// Converts a Unix timestamp string back to a <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="timestamp">The Unix timestamp as a string.</param>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing the timestamp in UTC.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="timestamp"/> is null.
    /// </exception>
    /// <exception cref="FormatException">
    /// Thrown when <paramref name="timestamp"/> is not a valid integer.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="timestamp"/> represents a date outside the valid range.
    /// </exception>
    public static DateTimeOffset FromTimestamp(string timestamp)
    {
        if (timestamp == null)
        {
            throw new ArgumentNullException(nameof(timestamp));
        }

        if (!long.TryParse(timestamp, out var seconds))
        {
            throw new FormatException($"Invalid timestamp format: {timestamp}");
        }

        try
        {
            return UnixEpoch.AddSeconds(seconds);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new ArgumentOutOfRangeException(nameof(timestamp),
                $"Timestamp {timestamp} is outside the valid date range.")
            { Source = ex.Source };
        }
    }
}
