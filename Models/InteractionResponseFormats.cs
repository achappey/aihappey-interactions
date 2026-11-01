using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

[JsonConverter(typeof(InteractionResponseFormatJsonConverter))]
public abstract class InteractionResponseFormat
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionTextResponseFormat : InteractionResponseFormat
{
    public InteractionTextResponseFormat() => Type = "text";

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }

    [JsonPropertyName("schema")]
    public JsonElement? Schema { get; set; }
}

public sealed class InteractionImageResponseFormat : InteractionResponseFormat
{
    public InteractionImageResponseFormat() => Type = "image";

    [JsonPropertyName("aspect_ratio")]
    public string? AspectRatio { get; set; }

    [JsonPropertyName("delivery")]
    public string? Delivery { get; set; }

    [JsonPropertyName("image_size")]
    public string? ImageSize { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }
}

public sealed class InteractionAudioResponseFormat : InteractionResponseFormat
{
    public InteractionAudioResponseFormat() => Type = "audio";

    [JsonPropertyName("bit_rate")]
    public int? BitRate { get; set; }

    [JsonPropertyName("delivery")]
    public string? Delivery { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }

    [JsonPropertyName("sample_rate")]
    public int? SampleRate { get; set; }
}

public sealed class InteractionVideoResponseFormat : InteractionResponseFormat
{
    public InteractionVideoResponseFormat() => Type = "video";

    [JsonPropertyName("aspect_ratio")]
    public string? AspectRatio { get; set; }

    [JsonPropertyName("delivery")]
    public string? Delivery { get; set; }

    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    [JsonPropertyName("gcs_uri")]
    public string? GcsUri { get; set; }

    [JsonPropertyName("resolution")]
    public string? Resolution { get; set; }
}

public sealed class InteractionUnknownResponseFormat : InteractionResponseFormat
{
}

[JsonConverter(typeof(InteractionResponseFormatsJsonConverter))]
public sealed class InteractionResponseFormats
{
    public InteractionResponseFormats(InteractionResponseFormat format)
    {
        Single = format ?? throw new ArgumentNullException(nameof(format));
    }

    public InteractionResponseFormats(IEnumerable<InteractionResponseFormat> formats)
    {
        Multiple = formats?.ToList() ?? throw new ArgumentNullException(nameof(formats));
    }

    public InteractionResponseFormat? Single { get; }

    public IReadOnlyList<InteractionResponseFormat>? Multiple { get; }

    public static implicit operator InteractionResponseFormats(InteractionResponseFormat format) => new(format);

    public static implicit operator InteractionResponseFormats(List<InteractionResponseFormat> formats) => new(formats);
}

public sealed class InteractionResponseFormatJsonConverter : JsonConverter<InteractionResponseFormat>
{
    private static readonly Dictionary<string, Type> FormatTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["text"] = typeof(InteractionTextResponseFormat),
        ["image"] = typeof(InteractionImageResponseFormat),
        ["audio"] = typeof(InteractionAudioResponseFormat),
        ["video"] = typeof(InteractionVideoResponseFormat)
    };

    public override InteractionResponseFormat? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("Interaction response format must be an object.");

        var type = root.TryGetProperty("type", out var typeProperty) ? typeProperty.GetString() : null;
        var targetType = type is not null && FormatTypes.TryGetValue(type, out var knownType)
            ? knownType
            : typeof(InteractionUnknownResponseFormat);

        return JsonSerializer.Deserialize(root.GetRawText(), targetType, options) as InteractionResponseFormat;
    }

    public override void Write(
        Utf8JsonWriter writer,
        InteractionResponseFormat value,
        JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}

public sealed class InteractionResponseFormatsJsonConverter : JsonConverter<InteractionResponseFormats>
{
    public override InteractionResponseFormats? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        return root.ValueKind switch
        {
            JsonValueKind.Object => new InteractionResponseFormats(
                root.Deserialize<InteractionResponseFormat>(options)
                ?? throw new JsonException("Interaction response format was empty.")),
            JsonValueKind.Array => new InteractionResponseFormats(
                root.Deserialize<List<InteractionResponseFormat>>(options) ?? []),
            _ => throw new JsonException("Interaction response_format must be an object or an array.")
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        InteractionResponseFormats value,
        JsonSerializerOptions options)
    {
        if (value.Multiple is not null)
            JsonSerializer.Serialize(writer, value.Multiple, options);
        else
            JsonSerializer.Serialize(writer, value.Single, options);
    }
}
