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
        ["code_execution_call"] = typeof(InteractionCodeExecutionCallDeltaData),
        ["code_execution_result"] = typeof(InteractionCodeExecutionResultDeltaData),
        ["document"] = typeof(InteractionDocumentDeltaData),
        ["file_search_call"] = typeof(InteractionFileSearchCallDeltaData),
        ["file_search_result"] = typeof(InteractionFileSearchResultDeltaData),
        ["function_result"] = typeof(InteractionFunctionResultDeltaData),
        ["google_maps_call"] = typeof(InteractionGoogleMapsCallDeltaData),
        ["google_maps_result"] = typeof(InteractionGoogleMapsResultDeltaData),
        ["google_search_call"] = typeof(InteractionGoogleSearchCallDeltaData),
        ["google_search_result"] = typeof(InteractionGoogleSearchResultDeltaData),
        ["image"] = typeof(InteractionImageDeltaData),
        ["mcp_server_tool_call"] = typeof(InteractionMcpServerToolCallDeltaData),
        ["mcp_server_tool_result"] = typeof(InteractionMcpServerToolResultDeltaData),
        ["processing_call"] = typeof(InteractionProcessingCallDeltaData),
        ["processing_result"] = typeof(InteractionProcessingResultDeltaData),
        ["retrieval_call"] = typeof(InteractionRetrievalCallDeltaData),
        ["retrieval_result"] = typeof(InteractionRetrievalResultDeltaData),
        ["text_annotation_delta"] = typeof(InteractionTextAnnotationDeltaData),
        ["thought_summary"] = typeof(InteractionThoughtSummaryDeltaData),
        ["thought_signature"] = typeof(InteractionThoughtSignatureDeltaData),
        ["url_context_call"] = typeof(InteractionUrlContextCallDeltaData),
        ["url_context_result"] = typeof(InteractionUrlContextResultDeltaData),
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

        var type = InteractionJson.ReadRequiredStringDiscriminator(root, "type");
        var targetType = DeltaTypes.TryGetValue(type, out var knownType)
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

        var eventType = InteractionJson.ReadStringDiscriminator(root, "event_type");
        if (string.IsNullOrWhiteSpace(eventType))
            throw new JsonException("Interaction stream event requires an event_type string.");

        if (!EventTypes.TryGetValue(eventType, out var targetType))
            targetType = typeof(InteractionUnknownStreamEvent);

        return JsonSerializer.Deserialize(root.GetRawText(), targetType, options) as InteractionStreamEventPart;
    }

    public override void Write(Utf8JsonWriter writer, InteractionStreamEventPart value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}
