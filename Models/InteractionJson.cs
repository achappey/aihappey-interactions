using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public static class InteractionJson
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new InteractionsInputJsonConverter(),
            new InteractionStepJsonConverter(),
            new InteractionContentJsonConverter(),
            new InteractionContentDeltaJsonConverter(),
            new InteractionToolJsonConverter(),
            new InteractionResponseFormatJsonConverter(),
            new InteractionResponseFormatsJsonConverter(),
            new InteractionEnvironmentValueJsonConverter(),
            new InteractionToolChoiceValueJsonConverter(),
            new InteractionVideoProcessingJsonConverter(),
            new InteractionTranscriptionModeJsonConverter(),
            new InteractionAgentConfigJsonConverter(),
            new InteractionStreamEventJsonConverter()
        }
    };

    internal static string? ReadStringDiscriminator(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value))
            return null;

        if (value.ValueKind != JsonValueKind.String)
            throw new JsonException($"Interaction discriminator '{propertyName}' must be a string.");

        return value.GetString();
    }

    internal static string ReadRequiredStringDiscriminator(JsonElement element, string propertyName)
    {
        var value = ReadStringDiscriminator(element, propertyName);
        if (string.IsNullOrWhiteSpace(value))
            throw new JsonException($"Interaction discriminator '{propertyName}' is required.");

        return value;
    }
}
