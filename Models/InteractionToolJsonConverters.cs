using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionToolJsonConverter : JsonConverter<InteractionTool>
{
    private static readonly Dictionary<string, Type> ToolTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["function"] = typeof(InteractionFunctionTool),
        ["code_execution"] = typeof(InteractionCodeExecutionTool),
        ["url_context"] = typeof(InteractionUrlContextTool),
        ["computer_use"] = typeof(InteractionComputerUseTool),
        ["mcp_server"] = typeof(InteractionMcpServerTool),
        ["google_search"] = typeof(InteractionGoogleSearchTool),
        ["file_search"] = typeof(InteractionFileSearchTool),
        ["google_maps"] = typeof(InteractionGoogleMapsTool),
        ["retrieval"] = typeof(InteractionRetrievalTool)
    };

    public override InteractionTool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("Interaction tool must be an object.");

        var type = InteractionJson.ReadRequiredStringDiscriminator(root, "type");
        if (!ToolTypes.TryGetValue(type, out var targetType))
            targetType = typeof(InteractionUnknownTool);

        return JsonSerializer.Deserialize(root.GetRawText(), targetType, options) as InteractionTool;
    }

    public override void Write(Utf8JsonWriter writer, InteractionTool value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}

public sealed class InteractionSpeechConfigListJsonConverter : JsonConverter<List<InteractionSpeechConfig>>
{
    public override List<InteractionSpeechConfig>? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
            return root.Deserialize<List<InteractionSpeechConfig>>(options);

        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty("speakers", out var speakers) ||
            speakers.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException("speech_config must be an array or an object containing a speakers array.");
        }

        var result = new InteractionSpeakerConfigCollection();
        result.AddRange(speakers.Deserialize<List<InteractionSpeechConfig>>(options) ?? []);
        result.AdditionalProperties = root.EnumerateObject()
            .Where(property => !string.Equals(property.Name, "speakers", StringComparison.Ordinal))
            .ToDictionary(property => property.Name, property => property.Value.Clone());
        return result;
    }

    public override void Write(
        Utf8JsonWriter writer,
        List<InteractionSpeechConfig> value,
        JsonSerializerOptions options)
    {
        if (value is not InteractionSpeakerConfigCollection speakerConfig)
        {
            JsonSerializer.Serialize(writer, value.ToArray(), options);
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("speakers");
        JsonSerializer.Serialize(writer, value.ToArray(), options);
        if (speakerConfig.AdditionalProperties is not null)
        {
            foreach (var (name, propertyValue) in speakerConfig.AdditionalProperties)
            {
                if (!string.Equals(name, "speakers", StringComparison.Ordinal))
                {
                    writer.WritePropertyName(name);
                    propertyValue.WriteTo(writer);
                }
            }
        }
        writer.WriteEndObject();
    }
}

public sealed class InteractionAgentConfigJsonConverter : JsonConverter<InteractionAgentConfig>
{
    public override InteractionAgentConfig? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("Interaction agent_config must be an object.");

        var type = InteractionJson.ReadRequiredStringDiscriminator(root, "type");
        var targetType = type.ToLowerInvariant() switch
        {
            "antigravity" => typeof(InteractionAntigravityAgentConfig),
            "code-mender" => typeof(InteractionCodeMenderAgentConfig),
            "deep-research" => typeof(InteractionDeepResearchAgentConfig),
            "dynamic" => typeof(InteractionDynamicAgentConfig),
            _ => typeof(InteractionUnknownAgentConfig)
        };

        return JsonSerializer.Deserialize(root.GetRawText(), targetType, options) as InteractionAgentConfig;
    }

    public override void Write(Utf8JsonWriter writer, InteractionAgentConfig value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}
