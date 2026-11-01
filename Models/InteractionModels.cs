using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionRequest
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("agent")]
    public string? Agent { get; set; }

    [JsonPropertyName("input")]
    public InteractionsInput? Input { get; set; }

    [JsonPropertyName("system_instruction")]
    public string? SystemInstruction { get; set; }

    [JsonPropertyName("tools")]
    public List<InteractionTool>? Tools { get; set; }

    [JsonPropertyName("response_format")]
    public InteractionResponseFormats? ResponseFormat { get; set; }

    [JsonPropertyName("stream")]
    public bool? Stream { get; set; }

    [JsonPropertyName("store")]
    public bool? Store { get; set; }

    [JsonPropertyName("background")]
    public bool? Background { get; set; }

    [JsonPropertyName("generation_config")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InteractionGenerationConfig? GenerationConfig { get; set; }

    [JsonPropertyName("agent_config")]
    public InteractionAgentConfig? AgentConfig { get; set; }

    [JsonPropertyName("previous_interaction_id")]
    public string? PreviousInteractionId { get; set; }

    [JsonPropertyName("continuation_token")]
    public string? ContinuationToken { get; set; }

    [JsonPropertyName("cached_content")]
    [Obsolete("Google marks cached_content as deprecated in the current Interactions API schema.")]
    public string? CachedContent { get; set; }

    [JsonPropertyName("environment")]
    public InteractionEnvironmentValue? Environment { get; set; }

    [JsonPropertyName("labels")]
    public Dictionary<string, string>? Labels { get; set; }

    [JsonPropertyName("response_mime_type")]
    [Obsolete("Use ResponseFormat; Google marks response_mime_type as deprecated.")]
    public string? ResponseMimeType { get; set; }

    [JsonPropertyName("safety_settings")]
    public List<InteractionSafetySetting>? SafetySettings { get; set; }

    [JsonPropertyName("webhook_config")]
    public InteractionWebhookConfig? WebhookConfig { get; set; }

    [JsonPropertyName("response_modalities")]
    [Obsolete("Use ResponseFormat; Google marks response_modalities as deprecated.")]
    public List<string>? ResponseModalities { get; set; }

    [JsonPropertyName("service_tier")]
    public string? ServiceTier { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }

    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object?>? Metadata { get; set; }
}

public sealed class Interaction
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("agent")]
    public string? Agent { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("status")]
    public InteractionStatus? Status { get; set; }

    [JsonPropertyName("created")]
    public string? Created { get; set; }

    [JsonPropertyName("updated")]
    public string? Updated { get; set; }

    [JsonPropertyName("steps")]
    public List<InteractionStep>? Steps { get; set; }

    [JsonPropertyName("system_instruction")]
    public string? SystemInstruction { get; set; }

    [JsonPropertyName("tools")]
    public List<InteractionTool>? Tools { get; set; }

    [JsonPropertyName("usage")]
    public InteractionUsage? Usage { get; set; }

    [JsonPropertyName("response_modalities")]
    [Obsolete("Use ResponseFormat; Google marks response_modalities as deprecated.")]
    public List<string>? ResponseModalities { get; set; }

    [JsonPropertyName("response_format")]
    public InteractionResponseFormats? ResponseFormat { get; set; }

    [JsonPropertyName("previous_interaction_id")]
    public string? PreviousInteractionId { get; set; }

    [JsonPropertyName("continuation_token")]
    public string? ContinuationToken { get; set; }

    [JsonPropertyName("cached_content")]
    [Obsolete("Google marks cached_content as deprecated in the current Interactions API schema.")]
    public string? CachedContent { get; set; }

    [JsonPropertyName("environment")]
    public InteractionEnvironmentValue? Environment { get; set; }

    [JsonPropertyName("environment_id")]
    public string? EnvironmentId { get; set; }

    [JsonPropertyName("errors")]
    public List<InteractionErrorInfo>? Errors { get; set; }

    [JsonPropertyName("labels")]
    public Dictionary<string, string>? Labels { get; set; }

    [JsonPropertyName("response_mime_type")]
    [Obsolete("Use ResponseFormat; Google marks response_mime_type as deprecated.")]
    public string? ResponseMimeType { get; set; }

    [JsonPropertyName("safety_settings")]
    public List<InteractionSafetySetting>? SafetySettings { get; set; }

    [JsonPropertyName("webhook_config")]
    public InteractionWebhookConfig? WebhookConfig { get; set; }

    [JsonPropertyName("service_tier")]
    public string? ServiceTier { get; set; }

    [JsonPropertyName("input")]
    public InteractionsInput? Input { get; set; }

    [JsonPropertyName("generation_config")]
    public InteractionGenerationConfig? GenerationConfig { get; set; }

    [JsonPropertyName("agent_config")]
    public InteractionAgentConfig? AgentConfig { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object?>? Metadata { get; set; }
}
