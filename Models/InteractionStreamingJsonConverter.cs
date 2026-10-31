using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionStreamEventJsonConverter : JsonConverter<InteractionStreamEventPart>
{
    private static readonly Dictionary<string, Type> EventTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["interaction.created"] = typeof(InteractionCreatedEvent),
        ["interaction.completed"] = typeof(InteractionCompletedEvent),
        ["interaction.in_progress"] = typeof(InteractionStatusEvent),
        ["interaction.requires_action"] = typeof(InteractionStatusEvent),
        ["interaction.status_update"] = typeof(InteractionStatusEvent),
        ["step.start"] = typeof(InteractionStepStartEvent),
        ["step.delta"] = typeof(InteractionStepDeltaEvent),
        ["step.stop"] = typeof(InteractionStepStopEvent),
        ["error"] = typeof(InteractionErrorEvent)
    };

    public override InteractionStreamEventPart? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("Interaction stream event must be an object.");

        var eventType = root.TryGetProperty("event_type", out var typeProp)
            ? typeProp.GetString()
            : root.TryGetProperty("type", out var altTypeProp)
                ? altTypeProp.GetString()
                : null;
        if (string.IsNullOrWhiteSpace(eventType) || !EventTypes.TryGetValue(eventType, out var targetType))
            targetType = typeof(InteractionUnknownStreamEvent);

        return JsonSerializer.Deserialize(root.GetRawText(), targetType, options) as InteractionStreamEventPart;
    }

    public override void Write(Utf8JsonWriter writer, InteractionStreamEventPart value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}
