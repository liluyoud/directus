using System.Text.Json;
using System.Text.Json.Serialization;
using Qute.Directus.Models;

namespace Qute.Directus.Serialization;

/// <summary>
/// Handles (de)serialization of <see cref="DirectusListResponse{T}"/>.
/// </summary>
/// <remarks>
/// <see cref="DirectusListResponse{T}"/> implements <see cref="IReadOnlyList{T}"/> for ergonomic
/// enumeration/indexing (see the README). Without this converter, <see cref="JsonSerializer"/>'s default
/// reflection-based resolution treats any type implementing <c>IEnumerable&lt;T&gt;</c> as a JSON array
/// and ignores its regular properties entirely — so it would try to read the wrapper itself as
/// <c>[...]</c> instead of <c>{ "data": [...], "meta": {...} }</c>, throwing a <see cref="JsonException"/>
/// on every list response. This factory/converter pair restores the intended object-shaped (de)serialization.
/// </remarks>
internal sealed class DirectusListResponseConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(DirectusListResponse<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(DirectusListResponseConverter<>).MakeGenericType(itemType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

internal sealed class DirectusListResponseConverter<T> : JsonConverter<DirectusListResponse<T>>
{
    public override DirectusListResponse<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"Expected a JSON object for {typeof(DirectusListResponse<T>).Name}, found {reader.TokenType}.");

        List<T>? data = null;
        ResponseMeta? meta = null;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected a property name.");

            var propertyName = reader.GetString();
            reader.Read();

            if (string.Equals(propertyName, "data", StringComparison.OrdinalIgnoreCase))
                data = JsonSerializer.Deserialize<List<T>>(ref reader, options);
            else if (string.Equals(propertyName, "meta", StringComparison.OrdinalIgnoreCase))
                meta = JsonSerializer.Deserialize<ResponseMeta>(ref reader, options);
            else
                reader.Skip();
        }

        return new DirectusListResponse<T> { Data = data ?? [], Meta = meta };
    }

    public override void Write(Utf8JsonWriter writer, DirectusListResponse<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("data");
        JsonSerializer.Serialize(writer, value.Data, options);

        if (value.Meta is not null)
        {
            writer.WritePropertyName("meta");
            JsonSerializer.Serialize(writer, value.Meta, options);
        }

        writer.WriteEndObject();
    }
}
