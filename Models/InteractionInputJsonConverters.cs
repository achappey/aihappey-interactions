using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionsInputJsonConverter : JsonConverter<InteractionsInput>
{
    private static readonly HashSet<string> ContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "text", "image", "audio", "document", "video"
    };

    private static readonly HashSet<string> StepTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "user_input", "model_output", "thought", "function_call", "code_execution_call",
        "url_context_call", "mcp_server_tool_call", "google_search_call", "file_search_call",
        "google_maps_call", "function_result", "code_execution_result", "url_context_result",
        "google_search_result", "mcp_server_tool_result", "file_search_result",
        "google_maps_result", "retrieval_call", "retrieval_result", "processing_call",
        "processing_result"
    };

    public override InteractionsInput? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        return root.ValueKind switch
        {
            JsonValueKind.String => new InteractionsInput(root.GetString() ?? string.Empty),
            JsonValueKind.Object => root.TryGetProperty("type", out _)
                ? new InteractionsInput(root.Deserialize<InteractionContent>(options) ?? new InteractionTextContent())
                : new InteractionsInput(root.Clone()),
            JsonValueKind.Array => ParseArray(root, options),
            JsonValueKind.Null or JsonValueKind.Undefined => new InteractionsInput(),
            _ => new InteractionsInput(root.Clone())
        };
    }

    public override void Write(Utf8JsonWriter writer, InteractionsInput value, JsonSerializerOptions options)
    {
        if (value.IsText)
        {
            writer.WriteStringValue(value.Text);
            return;
        }

        if (value.IsSingleContent)
        {
            JsonSerializer.Serialize(writer, value.SingleContent, options);
            return;
        }

        if (value.IsContent)
        {
            JsonSerializer.Serialize(writer, value.Content, options);
            return;
        }
 
        if (value.IsSteps)
        {
            JsonSerializer.Serialize(writer, value.Steps, options);
            return;
        }

#pragma warning disable CS0618
        if (value.IsTurns)
        {
            JsonSerializer.Serialize(writer, value.Turns, options);
            return;
        }
#pragma warning restore CS0618

        if (value.IsRaw)
        {
            value.Raw!.Value.WriteTo(writer);
            return;
        }

        writer.WriteNullValue();
    }

    private static InteractionsInput ParseArray(JsonElement root, JsonSerializerOptions options)
    {
        if (root.GetArrayLength() == 0)
            return new InteractionsInput(new List<InteractionContent>());

        var isContent = true;
        var isSteps = true;
        var isTurns = true;
        foreach (var item in root.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Object &&
                item.TryGetProperty("type", out var typeProperty) &&
                typeProperty.ValueKind == JsonValueKind.String)
            {
                var type = typeProperty.GetString()!;
                isContent &= ContentTypes.Contains(type);
                isSteps &= StepTypes.Contains(type);
            }
            else
            {
                isContent = false;
                isSteps = false;
            }
 
            if (item.ValueKind != JsonValueKind.Object || !item.TryGetProperty("role", out _) || !item.TryGetProperty("content", out _))
            {
                isTurns = false;
            }
        }
 
        if (isSteps)
            return new InteractionsInput(root.Deserialize<List<InteractionStep>>(options) ?? [], true);

#pragma warning disable CS0618
        if (isTurns)
            return new InteractionsInput(root.Deserialize<List<InteractionTurn>>(options) ?? [], true);
#pragma warning restore CS0618

        if (isContent)
            return new InteractionsInput(root.Deserialize<List<InteractionContent>>(options) ?? []);

        return new InteractionsInput(root.Clone());
    }
}

public sealed class InteractionTurnContentJsonConverter : JsonConverter<InteractionTurnContent>
{
    public override InteractionTurnContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        return root.ValueKind switch
        {
            JsonValueKind.String => new InteractionTurnContent(root.GetString() ?? string.Empty),
            JsonValueKind.Array => new InteractionTurnContent(root.Deserialize<List<InteractionContent>>(options) ?? []),
            JsonValueKind.Null or JsonValueKind.Undefined => new InteractionTurnContent(),
            _ => new InteractionTurnContent(root.Clone())
        };
    }

    public override void Write(Utf8JsonWriter writer, InteractionTurnContent value, JsonSerializerOptions options)
    {
        if (value.IsText)
        {
            writer.WriteStringValue(value.Text);
            return;
        }

        if (value.IsParts)
        {
            JsonSerializer.Serialize(writer, value.Parts, options);
            return;
        }

        if (value.IsRaw)
        {
            value.Raw!.Value.WriteTo(writer);
            return;
        }

        writer.WriteNullValue();
    }
}
