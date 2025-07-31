using System.Text.Json;
using BrickLink.Client.Models;

namespace BrickLink.Client.Serialization;

/// <summary>
/// Provides utility methods for JSON serialization and deserialization specific to BrickLink API operations.
/// Encapsulates common serialization patterns and error handling for API responses.
/// </summary>
public static class JsonSerializationHelper
{
    private static readonly Lazy<JsonSerializerOptions> _defaultOptions = 
        new(() => JsonSerializerOptionsFactory.CreateOptions());
    
    private static readonly Lazy<JsonSerializerOptions> _debugOptions = 
        new(() => JsonSerializerOptionsFactory.CreateDebugOptions());
    
    private static readonly Lazy<JsonSerializerOptions> _productionOptions = 
        new(() => JsonSerializerOptionsFactory.CreateProductionOptions());

    /// <summary>
    /// Gets the default JsonSerializerOptions instance for BrickLink API operations.
    /// </summary>
    public static JsonSerializerOptions DefaultOptions => _defaultOptions.Value;
    
    /// <summary>
    /// Gets the debug JsonSerializerOptions instance with indented formatting.
    /// </summary>
    public static JsonSerializerOptions DebugOptions => _debugOptions.Value;
    
    /// <summary>
    /// Gets the production JsonSerializerOptions instance optimized for performance.
    /// </summary>
    public static JsonSerializerOptions ProductionOptions => _productionOptions.Value;

    /// <summary>
    /// Serializes an object to JSON string using BrickLink-specific options.
    /// </summary>
    /// <typeparam name="T">The type of object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="options">Optional custom JsonSerializerOptions. If null, uses default options.</param>
    /// <returns>The JSON string representation of the object.</returns>
    /// <exception cref="JsonException">Thrown when serialization fails.</exception>
    public static string Serialize<T>(T value, JsonSerializerOptions? options = null)
    {
        try
        {
            return JsonSerializer.Serialize(value, options ?? DefaultOptions);
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new JsonException($"Failed to serialize object of type {typeof(T).Name}.", ex);
        }
    }

    /// <summary>
    /// Deserializes a JSON string to the specified type using BrickLink-specific options.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="options">Optional custom JsonSerializerOptions. If null, uses default options.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ArgumentException">Thrown when the JSON string is null or empty.</exception>
    /// <exception cref="JsonException">Thrown when deserialization fails.</exception>
    public static T Deserialize<T>(string json, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentException("JSON string cannot be null or empty.", nameof(json));
        }

        try
        {
            var result = JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
            if (result == null)
            {
                throw new JsonException($"Deserialization resulted in null value for type {typeof(T).Name}.");
            }
            return result;
        }
        catch (Exception ex) when (ex is not JsonException && ex is not ArgumentException)
        {
            throw new JsonException($"Failed to deserialize JSON to type {typeof(T).Name}.", ex);
        }
    }

    /// <summary>
    /// Attempts to deserialize a JSON string to the specified type, returning null on failure.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="options">Optional custom JsonSerializerOptions. If null, uses default options.</param>
    /// <returns>The deserialized object, or null if deserialization fails.</returns>
    public static T? TryDeserialize<T>(string json, JsonSerializerOptions? options = null) where T : class
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Deserializes a BrickLink API response wrapper, handling both successful and error responses.
    /// </summary>
    /// <typeparam name="T">The type of data expected in the response.</typeparam>
    /// <param name="json">The JSON response from the BrickLink API.</param>
    /// <param name="options">Optional custom JsonSerializerOptions. If null, uses default options.</param>
    /// <returns>The deserialized API response.</returns>
    /// <exception cref="ArgumentException">Thrown when the JSON string is null or empty.</exception>
    /// <exception cref="JsonException">Thrown when deserialization fails.</exception>
    public static ApiResponse<T> DeserializeApiResponse<T>(string json, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentException("JSON response cannot be null or empty.", nameof(json));
        }

        try
        {
            var response = JsonSerializer.Deserialize<ApiResponse<T>>(json, options ?? DefaultOptions);
            if (response == null)
            {
                throw new JsonException("Deserialization of API response resulted in null value.");
            }
            return response;
        }
        catch (Exception ex) when (ex is not JsonException && ex is not ArgumentException)
        {
            throw new JsonException($"Failed to deserialize BrickLink API response for data type {typeof(T).Name}.", ex);
        }
    }

    /// <summary>
    /// Validates that a JSON string is well-formed without performing full deserialization.
    /// </summary>
    /// <param name="json">The JSON string to validate.</param>
    /// <returns>True if the JSON is well-formed, false otherwise.</returns>
    public static bool IsValidJson(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Pretty-prints a JSON string with proper indentation for debugging purposes.
    /// </summary>
    /// <param name="json">The JSON string to format.</param>
    /// <returns>The formatted JSON string, or the original string if formatting fails.</returns>
    public static string PrettyPrint(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return json;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(document, DebugOptions);
        }
        catch
        {
            // Return original string if pretty-printing fails
            return json;
        }
    }
}