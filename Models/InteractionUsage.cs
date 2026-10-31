using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionUsage
{
    [JsonPropertyName("initial_input_tokens")]
    public int? InitialInputTokens { get; set; }

    [JsonPropertyName("total_input_tokens")]
    public int? TotalInputTokens { get; set; }

    [JsonPropertyName("input_tokens_by_modality")]
    public List<InteractionModalityTokens>? InputTokensByModality { get; set; }

    [JsonPropertyName("total_cached_tokens")]
    public int? TotalCachedTokens { get; set; }

    [JsonPropertyName("cached_tokens_by_modality")]
    public List<InteractionModalityTokens>? CachedTokensByModality { get; set; }

    [JsonPropertyName("total_output_tokens")]
    public int? TotalOutputTokens { get; set; }

    [JsonPropertyName("output_tokens_by_modality")]
    public List<InteractionModalityTokens>? OutputTokensByModality { get; set; }

    [JsonPropertyName("total_tool_use_tokens")]
    public int? TotalToolUseTokens { get; set; }

    [JsonPropertyName("tool_use_tokens_by_modality")]
    public List<InteractionModalityTokens>? ToolUseTokensByModality { get; set; }

    [JsonPropertyName("total_thought_tokens")]
    public int? TotalThoughtTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int? TotalTokens { get; set; }

    [JsonPropertyName("grounding_tool_count")]
    public List<InteractionGroundingToolCount>? GroundingToolCount { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionGroundingToolCount
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionModalityTokens
{
    [JsonPropertyName("modality")]
    public string? Modality { get; set; }

    [JsonPropertyName("tokens")]
    public int? Tokens { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
