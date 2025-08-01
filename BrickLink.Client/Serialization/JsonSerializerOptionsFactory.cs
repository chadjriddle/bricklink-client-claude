using System.Text.Json;
using System.Text.Json.Serialization;
using BrickLink.Client.Serialization.Converters;

namespace BrickLink.Client.Serialization;

/// <summary>
/// Factory class for creating and configuring JsonSerializerOptions for BrickLink API operations.
/// Provides consistent JSON serialization settings across the entire client library.
/// </summary>
public static class JsonSerializerOptionsFactory
{
    /// <summary>
    /// Creates a configured JsonSerializerOptions instance optimized for BrickLink API requirements.
    /// </summary>
    /// <returns>A JsonSerializerOptions instance with BrickLink-specific configuration.</returns>
    public static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            // Use camelCase property names to match API conventions
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

            // Allow reading from case-insensitive property names for flexibility
            PropertyNameCaseInsensitive = true,

            // Pretty print for debugging (will be overridden in production)
            WriteIndented = false,

            // Handle null values appropriately
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

            // Allow trailing commas in JSON for robustness
            AllowTrailingCommas = true,

            // Allow comments in JSON for configuration files
            ReadCommentHandling = JsonCommentHandling.Skip,

            // Custom converters for BrickLink API-specific serialization
            Converters =
            {
                new DateTimeOffsetConverter(),
                new DecimalPrecisionConverter(),
                new ItemTypeConverter(),
                new NewOrUsedConverter(),
                new CompletenessConverter(),
                new DirectionConverter(),
                new PriceGuideTypeConverter(),
                new AppearAsConverter()
            }
        };

        return options;
    }

    /// <summary>
    /// Creates a JsonSerializerOptions instance configured for debugging with indented output.
    /// </summary>
    /// <returns>A JsonSerializerOptions instance with debug-friendly formatting.</returns>
    public static JsonSerializerOptions CreateDebugOptions()
    {
        var options = CreateOptions();
        options.WriteIndented = true;
        return options;
    }

    /// <summary>
    /// Creates a JsonSerializerOptions instance configured for production with minimal output.
    /// </summary>
    /// <returns>A JsonSerializerOptions instance optimized for production performance.</returns>
    public static JsonSerializerOptions CreateProductionOptions()
    {
        var options = CreateOptions();
        options.WriteIndented = false;
        return options;
    }
}
