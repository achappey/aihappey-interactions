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
                ? new InteractionsInput(root.Deserialize<InteractionContent>(options)
                    ?? throw new JsonException("Interaction input content was empty."))
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
        foreach (var item in root.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Object &&
                item.TryGetProperty("type", out var typeProperty) &&
                typeProperty.ValueKind == JsonValueKind.String)
            {
                var type = typeProperty.GetString()!;
                isContent &= InteractionContentJsonConverter.ContentTypes.ContainsKey(type);
                isSteps &= InteractionStepJsonConverter.StepTypes.ContainsKey(type);
            }
            else
            {
                isContent = false;
                isSteps = false;
            }
        }
 
        if (isSteps)
            return new InteractionsInput(root.Deserialize<List<InteractionStep>>(options)
                ?? throw new JsonException("Interaction input steps were empty."));

        if (isContent)
            return new InteractionsInput(root.Deserialize<List<InteractionContent>>(options)
                ?? throw new JsonException("Interaction input content was empty."));

        return new InteractionsInput(root.Clone());
    }
}
