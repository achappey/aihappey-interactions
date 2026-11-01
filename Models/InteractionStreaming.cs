using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

[JsonConverter(typeof(InteractionStreamEventJsonConverter))]
public abstract class InteractionStreamEventPart
{
    [JsonPropertyName("event_type")]
    public string EventType { get; set; } = default!;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionCreatedEvent : InteractionStreamEventPart
{
    public InteractionCreatedEvent()
    {
        EventType = "interaction.created";
    }

    [JsonPropertyName("interaction")]
    public Interaction? Interaction { get; set; }

    [JsonPropertyName("event_id")]
    public string? EventId { get; set; }
}

public sealed class InteractionCompletedEvent : InteractionStreamEventPart
{
    public InteractionCompletedEvent()
    {
        EventType = "interaction.completed";
    }

    [JsonPropertyName("interaction")]
    public Interaction? Interaction { get; set; }

    [JsonPropertyName("event_id")]
    public string? EventId { get; set; }
}

public sealed class InteractionStatusEvent : InteractionStreamEventPart
{
    public InteractionStatusEvent()
    {
        EventType = "interaction.status_update";
    }

    [JsonPropertyName("interaction_id")]
    public string? InteractionId { get; set; }

    [JsonPropertyName("status")]
    public InteractionStatus? Status { get; set; }

    [JsonPropertyName("event_id")]
    public string? EventId { get; set; }

    [JsonPropertyName("continuation_token")]
    public string? ContinuationToken { get; set; }

    [JsonPropertyName("usage")]
    public InteractionUsage? Usage { get; set; }
}

public sealed class InteractionStepStartEvent : InteractionStreamEventPart
{
    public InteractionStepStartEvent()
    {
        EventType = "step.start";
    }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("step")]
    public InteractionStep? Step { get; set; }

    [JsonPropertyName("event_id")]
    public string? EventId { get; set; }
}

public sealed class InteractionStepDeltaEvent : InteractionStreamEventPart
{
    public InteractionStepDeltaEvent()
    {
        EventType = "step.delta";
    }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("delta")]
    public InteractionContentDeltaData? Delta { get; set; }

    [JsonPropertyName("metadata")]
    public InteractionStepDeltaMetadata? Metadata { get; set; }

    [JsonPropertyName("event_id")]
    public string? EventId { get; set; }
}

public sealed class InteractionStepStopEvent : InteractionStreamEventPart
{
    public InteractionStepStopEvent()
    {
        EventType = "step.stop";
    }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("event_id")]
    public string? EventId { get; set; }
 
    [JsonPropertyName("step_usage")]
    public InteractionUsage? StepUsage { get; set; }

    [JsonPropertyName("usage")]
    public InteractionUsage? Usage { get; set; }
}

public sealed class InteractionErrorEvent : InteractionStreamEventPart
{
    public InteractionErrorEvent()
    {
        EventType = "error";
    }

    [JsonPropertyName("error")]
    public InteractionErrorInfo? Error { get; set; }

    [JsonPropertyName("event_id")]
    public string? EventId { get; set; }
}

public sealed class InteractionUnknownStreamEvent : InteractionStreamEventPart
{
}

[JsonConverter(typeof(InteractionContentDeltaJsonConverter))]
public abstract class InteractionContentDeltaData
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionTextDeltaData : InteractionContentDeltaData
{
    public InteractionTextDeltaData() => Type = "text";

    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

public sealed class InteractionArgumentsDeltaData : InteractionContentDeltaData
{
    public InteractionArgumentsDeltaData() => Type = "arguments_delta";

    [JsonPropertyName("arguments")]
    public string? Arguments { get; set; }
}

public sealed class InteractionThoughtSummaryDeltaData : InteractionContentDeltaData
{
    public InteractionThoughtSummaryDeltaData() => Type = "thought_summary";

    [JsonPropertyName("content")]
    public InteractionContent? Content { get; set; }
}

public sealed class InteractionThoughtSignatureDeltaData : InteractionContentDeltaData
{
    public InteractionThoughtSignatureDeltaData() => Type = "thought_signature";

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionAudioDeltaData : InteractionContentDeltaData
{
    public InteractionAudioDeltaData() => Type = "audio";

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }

    [JsonPropertyName("sample_rate")]
    public int? SampleRate { get; set; }

    [JsonPropertyName("channels")]
    public int? Channels { get; set; }
}

public sealed class InteractionImageDeltaData : InteractionContentDeltaData
{
    public InteractionImageDeltaData() => Type = "image";

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }

    [JsonPropertyName("resolution")]
    public string? Resolution { get; set; }
}

public sealed class InteractionVideoDeltaData : InteractionContentDeltaData
{
    public InteractionVideoDeltaData() => Type = "video";

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }

    [JsonPropertyName("resolution")]
    public string? Resolution { get; set; }
}

public sealed class InteractionDocumentDeltaData : InteractionContentDeltaData
{
    public InteractionDocumentDeltaData() => Type = "document";

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }
}

public sealed class InteractionTextAnnotationDeltaData : InteractionContentDeltaData
{
    public InteractionTextAnnotationDeltaData() => Type = "text_annotation_delta";

    [JsonPropertyName("annotations")]
    public List<InteractionAnnotation>? Annotations { get; set; }
}

public sealed class InteractionFunctionResultDeltaData : InteractionContentDeltaData
{
    public InteractionFunctionResultDeltaData() => Type = "function_result";

    [JsonPropertyName("result")]
    public JsonElement? Result { get; set; }

    [JsonPropertyName("is_error")]
    public bool? IsError { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public sealed class InteractionCodeExecutionCallDeltaData : InteractionContentDeltaData
{
    public InteractionCodeExecutionCallDeltaData() => Type = "code_execution_call";

    [JsonPropertyName("arguments")]
    public InteractionCodeExecutionCallArguments? Arguments { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionCodeExecutionResultDeltaData : InteractionContentDeltaData
{
    public InteractionCodeExecutionResultDeltaData() => Type = "code_execution_result";

    [JsonPropertyName("result")]
    public string? Result { get; set; }

    [JsonPropertyName("is_error")]
    public bool? IsError { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionFileSearchCallDeltaData : InteractionContentDeltaData
{
    public InteractionFileSearchCallDeltaData() => Type = "file_search_call";

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionFileSearchResultDeltaData : InteractionContentDeltaData
{
    public InteractionFileSearchResultDeltaData() => Type = "file_search_result";

    [JsonPropertyName("result")]
    public List<InteractionFileSearchResult>? Result { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionGoogleMapsCallDeltaData : InteractionContentDeltaData
{
    public InteractionGoogleMapsCallDeltaData() => Type = "google_maps_call";

    [JsonPropertyName("arguments")]
    public InteractionGoogleMapsCallArguments? Arguments { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionGoogleMapsResultDeltaData : InteractionContentDeltaData
{
    public InteractionGoogleMapsResultDeltaData() => Type = "google_maps_result";

    [JsonPropertyName("result")]
    public List<InteractionGoogleMapsResult>? Result { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionGoogleSearchCallDeltaData : InteractionContentDeltaData
{
    public InteractionGoogleSearchCallDeltaData() => Type = "google_search_call";

    [JsonPropertyName("arguments")]
    public InteractionGoogleSearchCallArguments? Arguments { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionGoogleSearchResultDeltaData : InteractionContentDeltaData
{
    public InteractionGoogleSearchResultDeltaData() => Type = "google_search_result";

    [JsonPropertyName("result")]
    public List<InteractionGoogleSearchResult>? Result { get; set; }

    [JsonPropertyName("is_error")]
    public bool? IsError { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionMcpServerToolCallDeltaData : InteractionContentDeltaData
{
    public InteractionMcpServerToolCallDeltaData() => Type = "mcp_server_tool_call";

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("server_name")]
    public string? ServerName { get; set; }

    [JsonPropertyName("arguments")]
    public JsonElement? Arguments { get; set; }

    [JsonPropertyName("retrieval_type")]
    public string? RetrievalType { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionMcpServerToolResultDeltaData : InteractionContentDeltaData
{
    public InteractionMcpServerToolResultDeltaData() => Type = "mcp_server_tool_result";

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("server_name")]
    public string? ServerName { get; set; }

    [JsonPropertyName("result")]
    public JsonElement? Result { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionProcessingCallDeltaData : InteractionContentDeltaData
{
    public InteractionProcessingCallDeltaData() => Type = "processing_call";

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionProcessingResultDeltaData : InteractionContentDeltaData
{
    public InteractionProcessingResultDeltaData() => Type = "processing_result";

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionRetrievalCallDeltaData : InteractionContentDeltaData
{
    public InteractionRetrievalCallDeltaData() => Type = "retrieval_call";

    [JsonPropertyName("arguments")]
    public JsonElement? Arguments { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionRetrievalResultDeltaData : InteractionContentDeltaData
{
    public InteractionRetrievalResultDeltaData() => Type = "retrieval_result";

    [JsonPropertyName("is_error")]
    public bool? IsError { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionUrlContextCallDeltaData : InteractionContentDeltaData
{
    public InteractionUrlContextCallDeltaData() => Type = "url_context_call";

    [JsonPropertyName("arguments")]
    public InteractionUrlContextCallArguments? Arguments { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionUrlContextResultDeltaData : InteractionContentDeltaData
{
    public InteractionUrlContextResultDeltaData() => Type = "url_context_result";

    [JsonPropertyName("result")]
    public List<InteractionUrlContextResult>? Result { get; set; }

    [JsonPropertyName("is_error")]
    public bool? IsError { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionUnknownContentDeltaData : InteractionContentDeltaData
{
}

public sealed class InteractionStepDeltaMetadata
{
    [JsonPropertyName("total_usage")]
    public InteractionUsage? TotalUsage { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionErrorInfo
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
