using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionContentDeltaJsonConverter : JsonConverter<InteractionContentDeltaData>
{
    private static readonly Dictionary<string, Type> DeltaTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["text"] = typeof(InteractionTextDeltaData),
        ["arguments_delta"] = typeof(InteractionArgumentsDeltaData),
        ["audio"] = typeof(InteractionAudioDeltaData),
        ["document"] = typeof(InteractionDocumentDeltaData),
        ["function_result"] = typeof(InteractionFunctionResultDeltaData),
        ["image"] = typeof(InteractionImageDeltaData),
        ["text_annotation_delta"] = typeof(InteractionTextAnnotationDeltaData),
        ["thought_summary"] = typeof(InteractionThoughtSummaryDeltaData),
        ["thought_signature"] = typeof(InteractionThoughtSignatureDeltaData),
        ["video"] = typeof(InteractionVideoDeltaData)
    };

    public override InteractionContentDeltaData? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("Interaction content delta must be an object.");

        var type = root.TryGetProperty("type", out var typeProperty) ? typeProperty.GetString() : null;
        var targetType = type is not null && DeltaTypes.TryGetValue(type, out var knownType)
            ? knownType
            : typeof(InteractionUnknownContentDeltaData);

        return JsonSerializer.Deserialize(root.GetRawText(), targetType, options) as InteractionContentDeltaData;
    }

    public override void Write(
        Utf8JsonWriter writer,
        InteractionContentDeltaData value,
        JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}

public sealed class InteractionStreamEventJsonConverter : JsonConverter<InteractionStreamEventPart>
{
    private static readonly Dictionary<string, Type> EventTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["interaction.created"] = typeof(InteractionCreatedEvent),
        ["interaction.completed"] = typeof(InteractionCompletedEvent),
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

        if (!root.TryGetProperty("event_type", out var typeProp) ||
            typeProp.ValueKind != JsonValueKind.String ||
            string.IsNullOrWhiteSpace(typeProp.GetString()))
            throw new JsonException("Interaction stream event requires an event_type string.");

        var eventType = typeProp.GetString()!;
        if (!EventTypes.TryGetValue(eventType, out var targetType))
            targetType = typeof(InteractionUnknownStreamEvent);

        return JsonSerializer.Deserialize(root.GetRawText(), targetType, options) as InteractionStreamEventPart;
    }

    public override void Write(Utf8JsonWriter writer, InteractionStreamEventPart value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}
