using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BrickLink.Client.Serialization.Converters;

/// <summary>
/// Custom JSON converter for DateTimeOffset values that handles BrickLink API date formats.
/// Supports both ISO 8601 format and Unix timestamp conversion.
/// </summary>
public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    private static readonly string[] SupportedFormats =
    {
        "yyyy-MM-ddTHH:mm:ss.fffZ",          // ISO 8601 with milliseconds
        "yyyy-MM-ddTHH:mm:ssZ",              // ISO 8601 without milliseconds
        "yyyy-MM-dd HH:mm:ss",               // BrickLink legacy format
        "yyyy-MM-ddTHH:mm:ss.fff",           // ISO 8601 without timezone
        "yyyy-MM-ddTHH:mm:ss"                // ISO 8601 simple format
    };

    /// <summary>
    /// Reads and converts JSON to a DateTimeOffset value.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert to.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The converted DateTimeOffset value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value cannot be converted to DateTimeOffset.</exception>
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Cannot convert null value to DateTimeOffset.");
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrEmpty(stringValue))
            {
                throw new JsonException("Cannot convert empty string to DateTimeOffset.");
            }

            // Try parsing with supported formats
            foreach (var format in SupportedFormats)
            {
                if (DateTimeOffset.TryParseExact(stringValue, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result))
                {
                    return result.ToUniversalTime();
                }
            }

            // Try standard parsing as fallback
            if (DateTimeOffset.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var fallbackResult))
            {
                return fallbackResult.ToUniversalTime();
            }

            throw new JsonException($"Unable to convert '{stringValue}' to DateTimeOffset. Supported formats: {string.Join(", ", SupportedFormats)}");
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            // Handle Unix timestamp (seconds since epoch)
            if (reader.TryGetInt64(out var unixTimestamp))
            {
                try
                {
                    return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    throw new JsonException($"Unix timestamp {unixTimestamp} is out of valid range.", ex);
                }
            }
        }

        throw new JsonException($"Unexpected token type {reader.TokenType} when parsing DateTimeOffset.");
    }

    /// <summary>
    /// Writes a DateTimeOffset value to JSON.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The DateTimeOffset value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        // Always write in ISO 8601 format with UTC timezone
        var formattedValue = value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
        writer.WriteStringValue(formattedValue);
    }
}