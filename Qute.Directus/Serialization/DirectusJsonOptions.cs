using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qute.Directus.Serialization;

/// <summary>
/// Provides pre-configured <see cref="JsonSerializerOptions"/> for Directus API serialization.
/// </summary>
public static class DirectusJsonOptions
{
    private static JsonSerializerOptions? _instance;

    /// <summary>
    /// Gets the shared <see cref="JsonSerializerOptions"/> configured for Directus API communication.
    /// Uses snake_case naming policy and ignores null values on serialization.
    /// </summary>
    public static JsonSerializerOptions Default => _instance ??= Create();

    /// <summary>
    /// Creates a new <see cref="JsonSerializerOptions"/> instance configured for Directus.
    /// </summary>
    public static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));

        return options;
    }
}
