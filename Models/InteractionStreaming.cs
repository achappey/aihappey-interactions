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
    public string? Status { get; set; }

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

public sealed class InteractionContentDeltaData
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

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
