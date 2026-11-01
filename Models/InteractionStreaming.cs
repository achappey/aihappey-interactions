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
 
    [JsonPropertyName("status")]
    public string? Status { get; set; }
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

    [JsonPropertyName("rate")]
    [Obsolete("Use SampleRate; Google marks rate as deprecated.")]
    public int? Rate { get; set; }

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
