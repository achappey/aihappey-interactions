using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionsInputJsonConverter : JsonConverter<InteractionsInput>
{
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

        if (value.IsTurns)
        {
            JsonSerializer.Serialize(writer, value.Turns, options);
            return;
        }

        if (value.IsRaw)
        {
            value.Raw!.Value.WriteTo(writer);
            return;
        }

        writer.WriteNullValue();
    }

    private static InteractionsInput ParseArray(JsonElement root, JsonSerializerOptions options)
    {
        var isSteps = true;
        var isTurns = true;
        foreach (var item in root.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object || !item.TryGetProperty("type", out _))
            {
                isSteps = false;
            }
 
            if (item.ValueKind != JsonValueKind.Object || !item.TryGetProperty("role", out _) || !item.TryGetProperty("content", out _))
            {
                isTurns = false;
            }
        }
 
        if (isSteps)
            return new InteractionsInput(root.Deserialize<List<InteractionStep>>(options) ?? [], true);

        if (isTurns)
            return new InteractionsInput(root.Deserialize<List<InteractionTurn>>(options) ?? [], true);

        return new InteractionsInput(root.Deserialize<List<InteractionContent>>(options) ?? []);
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
