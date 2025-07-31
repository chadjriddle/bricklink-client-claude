using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BrickLink.Client.Serialization.Converters;

/// <summary>
/// Custom JSON converter for decimal values that ensures precise handling of monetary amounts
/// and pricing information from the BrickLink API. Maintains precision and handles various
/// numeric formats that may be returned by the API.
/// </summary>
public class DecimalPrecisionConverter : JsonConverter<decimal>
{
    private const int DefaultPrecision = 4; // Support up to 4 decimal places for currency

    /// <summary>
    /// Reads and converts JSON to a decimal value with proper precision handling.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert to.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The converted decimal value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value cannot be converted to decimal.</exception>
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                if (reader.TryGetDecimal(out var decimalValue))
                {
                    // Round to maintain precision and avoid floating point artifacts
                    return Math.Round(decimalValue, DefaultPrecision, MidpointRounding.AwayFromZero);
                }

                // Throw an exception if decimal parsing fails
                throw new JsonException($"Unable to convert numeric value to decimal. Ensure the value is a valid decimal representation.");

            case JsonTokenType.String:
                var stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                {
                    throw new JsonException("Cannot convert null or empty string to decimal.");
                }

                // Handle string representations of numbers (some APIs may return prices as strings)
                if (decimal.TryParse(stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsedDecimal))
                {
                    return Math.Round(parsedDecimal, DefaultPrecision, MidpointRounding.AwayFromZero);
                }

                throw new JsonException($"Unable to convert string '{stringValue}' to decimal.");

            case JsonTokenType.Null:
                throw new JsonException("Cannot convert null value to decimal.");

            default:
                throw new JsonException($"Unexpected token type {reader.TokenType} when parsing decimal.");
        }
    }

    /// <summary>
    /// Writes a decimal value to JSON with appropriate precision.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The decimal value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        // Write the decimal value directly, letting System.Text.Json handle the formatting
        // This ensures maximum precision while maintaining JSON compatibility
        writer.WriteNumberValue(value);
    }
}

/// <summary>
/// Custom JSON converter for nullable decimal values that ensures precise handling.
/// </summary>
public class NullableDecimalPrecisionConverter : JsonConverter<decimal?>
{
    private readonly DecimalPrecisionConverter _baseConverter = new();

    /// <summary>
    /// Reads and converts JSON to a nullable decimal value.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert to.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The converted nullable decimal value.</returns>
    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return _baseConverter.Read(ref reader, typeof(decimal), options);
    }

    /// <summary>
    /// Writes a nullable decimal value to JSON.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The nullable decimal value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            _baseConverter.Write(writer, value.Value, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
