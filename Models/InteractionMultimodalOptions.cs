using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

[JsonConverter(typeof(InteractionVideoProcessingJsonConverter))]
public sealed class InteractionVideoProcessing
{
    public InteractionVideoProcessing(string mode) => Mode = mode;

    public InteractionVideoProcessing(InteractionStaticVideoProcessing configuration)
        => StaticConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    internal InteractionVideoProcessing(JsonElement unknown) => Unknown = unknown;

    public string? Mode { get; }

    public InteractionStaticVideoProcessing? StaticConfiguration { get; }

    public JsonElement? Unknown { get; }

    public static implicit operator InteractionVideoProcessing(string mode) => new(mode);

    public static implicit operator InteractionVideoProcessing(InteractionStaticVideoProcessing configuration)
        => new(configuration);
}

public sealed class InteractionStaticVideoProcessing
{
    public InteractionStaticVideoProcessing() => Type = "static";

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("start_offset")]
    public string? StartOffset { get; set; }

    [JsonPropertyName("end_offset")]
    public string? EndOffset { get; set; }

    [JsonPropertyName("fps")]
    public double? Fps { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionVideoProcessingJsonConverter : JsonConverter<InteractionVideoProcessing>
{
    public override InteractionVideoProcessing? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        if (root.ValueKind == JsonValueKind.String)
            return new InteractionVideoProcessing(root.GetString()!);

        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("Video processing must be a string or an object.");

        if (string.Equals(
                InteractionJson.ReadStringDiscriminator(root, "type"),
                "static",
                StringComparison.OrdinalIgnoreCase))
        {
            return new InteractionVideoProcessing(
                root.Deserialize<InteractionStaticVideoProcessing>(options)
                ?? throw new JsonException("Static video processing configuration was empty."));
        }

        return new InteractionVideoProcessing(root.Clone());
    }

    public override void Write(
        Utf8JsonWriter writer,
        InteractionVideoProcessing value,
        JsonSerializerOptions options)
    {
        if (value.Mode is not null)
            writer.WriteStringValue(value.Mode);
        else if (value.StaticConfiguration is not null)
            JsonSerializer.Serialize(writer, value.StaticConfiguration, options);
        else if (value.Unknown is { } unknown)
            unknown.WriteTo(writer);
        else
            writer.WriteNullValue();
    }
}

[JsonConverter(typeof(InteractionTranscriptionModeJsonConverter))]
public sealed class InteractionTranscriptionMode
{
    public InteractionTranscriptionMode(string mode) => Mode = mode;

    public InteractionTranscriptionMode(InteractionTranscriptionModeConfiguration configuration)
        => Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    internal InteractionTranscriptionMode(JsonElement unknown) => Unknown = unknown;

    public string? Mode { get; }

    public InteractionTranscriptionModeConfiguration? Configuration { get; }

    public JsonElement? Unknown { get; }

    public static implicit operator InteractionTranscriptionMode(string mode) => new(mode);

    public static implicit operator InteractionTranscriptionMode(InteractionTranscriptionModeConfiguration configuration)
        => new(configuration);
}

public abstract class InteractionTranscriptionModeConfiguration
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionSmartTranscriptionMode : InteractionTranscriptionModeConfiguration
{
    public InteractionSmartTranscriptionMode() => Type = "smart";
}

public sealed class InteractionVerbatimTranscriptionMode : InteractionTranscriptionModeConfiguration
{
    public InteractionVerbatimTranscriptionMode() => Type = "verbatim";

    [JsonPropertyName("diarization_mode")]
    public string? DiarizationMode { get; set; }

    [JsonPropertyName("timestamp_granularities")]
    public List<string>? TimestampGranularities { get; set; }
}

public sealed class InteractionUnknownTranscriptionMode : InteractionTranscriptionModeConfiguration
{
}

public sealed class InteractionTranscriptionModeJsonConverter : JsonConverter<InteractionTranscriptionMode>
{
    public override InteractionTranscriptionMode? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        if (root.ValueKind == JsonValueKind.String)
            return new InteractionTranscriptionMode(root.GetString()!);

        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("Transcription mode must be a string or an object.");

        var configurationType = InteractionJson.ReadStringDiscriminator(root, "type")?.ToLowerInvariant() switch
        {
            "smart" => typeof(InteractionSmartTranscriptionMode),
            "verbatim" => typeof(InteractionVerbatimTranscriptionMode),
            _ => typeof(InteractionUnknownTranscriptionMode)
        };

        return new InteractionTranscriptionMode(
            (InteractionTranscriptionModeConfiguration?)JsonSerializer.Deserialize(
                root.GetRawText(), configurationType, options)
            ?? throw new JsonException("Transcription mode configuration was empty."));
    }

    public override void Write(
        Utf8JsonWriter writer,
        InteractionTranscriptionMode value,
        JsonSerializerOptions options)
    {
        if (value.Mode is not null)
            writer.WriteStringValue(value.Mode);
        else if (value.Configuration is not null)
            JsonSerializer.Serialize(writer, value.Configuration, options);
        else if (value.Unknown is { } unknown)
            unknown.WriteTo(writer);
        else
            writer.WriteNullValue();
    }
}
