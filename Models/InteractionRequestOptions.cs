using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

[JsonConverter(typeof(InteractionEnvironmentValueJsonConverter))]
public sealed class InteractionEnvironmentValue
{
    public InteractionEnvironmentValue(string reference) => Reference = reference;

    public InteractionEnvironmentValue(InteractionRemoteEnvironment configuration)
        => Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    internal InteractionEnvironmentValue(JsonElement unknown) => Unknown = unknown;

    public string? Reference { get; }

    public InteractionRemoteEnvironment? Configuration { get; }

    public JsonElement? Unknown { get; }

    public static implicit operator InteractionEnvironmentValue(string reference) => new(reference);

    public static implicit operator InteractionEnvironmentValue(InteractionRemoteEnvironment configuration) => new(configuration);
}

public sealed class InteractionRemoteEnvironment
{
    public InteractionRemoteEnvironment() => Type = "remote";

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("environment_id")]
    public string? EnvironmentId { get; set; }

    [JsonPropertyName("env")]
    public Dictionary<string, JsonElement>? EnvironmentVariables { get; set; }

    [JsonPropertyName("network")]
    public JsonElement? Network { get; set; }

    [JsonPropertyName("sources")]
    public List<InteractionEnvironmentSource>? Sources { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionEnvironmentSource
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("target")]
    public string? Target { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("encoding")]
    public string? Encoding { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionEnvironmentValueJsonConverter : JsonConverter<InteractionEnvironmentValue>
{
    public override InteractionEnvironmentValue? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        return root.ValueKind switch
        {
            JsonValueKind.String => new InteractionEnvironmentValue(root.GetString()!),
            JsonValueKind.Object when string.Equals(
                                          InteractionJson.ReadStringDiscriminator(root, "type"),
                                          "remote",
                                          StringComparison.OrdinalIgnoreCase)
                => new InteractionEnvironmentValue(
                    root.Deserialize<InteractionRemoteEnvironment>(options)
                    ?? throw new JsonException("Remote environment configuration was empty.")),
            JsonValueKind.Object => new InteractionEnvironmentValue(root.Clone()),
            _ => throw new JsonException("Interaction environment must be a string or an object.")
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        InteractionEnvironmentValue value,
        JsonSerializerOptions options)
    {
        if (value.Reference is not null)
            writer.WriteStringValue(value.Reference);
        else if (value.Configuration is not null)
            JsonSerializer.Serialize(writer, value.Configuration, options);
        else if (value.Unknown is { } unknown)
            unknown.WriteTo(writer);
        else
            writer.WriteNullValue();
    }
}

public sealed class InteractionWebhookConfig
{
    [JsonPropertyName("uris")]
    public List<string>? Uris { get; set; }

    [JsonPropertyName("user_metadata")]
    public Dictionary<string, JsonElement>? UserMetadata { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

[JsonConverter(typeof(InteractionToolChoiceValueJsonConverter))]
public sealed class InteractionToolChoiceValue
{
    public InteractionToolChoiceValue(string mode) => Mode = mode;

    public InteractionToolChoiceValue(InteractionToolChoiceConfiguration configuration)
        => Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    internal InteractionToolChoiceValue(JsonElement unknown) => Unknown = unknown;

    public string? Mode { get; }

    public InteractionToolChoiceConfiguration? Configuration { get; }

    public JsonElement? Unknown { get; }

    public static implicit operator InteractionToolChoiceValue(string mode) => new(mode);

    public static implicit operator InteractionToolChoiceValue(InteractionToolChoiceConfiguration configuration)
        => new(configuration);
}

public sealed class InteractionToolChoiceConfiguration
{
    [JsonPropertyName("allowed_tools")]
    public InteractionAllowedTools? AllowedTools { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionToolChoiceValueJsonConverter : JsonConverter<InteractionToolChoiceValue>
{
    public override InteractionToolChoiceValue? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        if (root.ValueKind == JsonValueKind.String)
            return new InteractionToolChoiceValue(root.GetString()!);

        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("Interaction tool_choice must be a string or an object.");

        if (root.TryGetProperty("allowed_tools", out _))
        {
            return new InteractionToolChoiceValue(
                root.Deserialize<InteractionToolChoiceConfiguration>(options)
                ?? throw new JsonException("Tool choice configuration was empty."));
        }

        return new InteractionToolChoiceValue(root.Clone());
    }

    public override void Write(
        Utf8JsonWriter writer,
        InteractionToolChoiceValue value,
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
