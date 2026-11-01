using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionGenerationConfig
{
    [JsonPropertyName("seed")]
    public int? Seed { get; set; }

    [JsonPropertyName("stop_sequences")]
    public List<string>? StopSequences { get; set; }

    [JsonPropertyName("thinking_level")]
    public string? ThinkingLevel { get; set; }

    [JsonPropertyName("thinking_summaries")]
    public string? ThinkingSummaries { get; set; }

    [JsonPropertyName("max_output_tokens")]
    public int? MaxOutputTokens { get; set; }

    [JsonPropertyName("speech_config")]
    [JsonConverter(typeof(InteractionSpeechConfigListJsonConverter))]
    public List<InteractionSpeechConfig>? SpeechConfig { get; set; }

    [JsonPropertyName("transcription_config")]
    public InteractionTranscriptionConfig? TranscriptionConfig { get; set; }

    [JsonPropertyName("video_config")]
    public InteractionVideoConfig? VideoConfig { get; set; }

    [JsonPropertyName("tool_choice")]
    public InteractionToolChoiceValue? ToolChoice { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionSpeechConfig
{
    [JsonPropertyName("voice")]
    public string? Voice { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("speaker")]
    public string? Speaker { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionSpeakerConfigCollection : List<InteractionSpeechConfig>
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionTranscriptionConfig
{
    [JsonPropertyName("custom_vocabulary")]
    public List<string>? CustomVocabulary { get; set; }

    [JsonPropertyName("language_codes")]
    public List<string>? LanguageCodes { get; set; }

    [JsonPropertyName("mode")]
    public InteractionTranscriptionMode? Mode { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionVideoConfig
{
    [JsonPropertyName("task")]
    public string? Task { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

[JsonConverter(typeof(InteractionAgentConfigJsonConverter))]
public abstract class InteractionAgentConfig
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionDynamicAgentConfig : InteractionAgentConfig
{
    public InteractionDynamicAgentConfig()
    {
        Type = "dynamic";
    }
}

public sealed class InteractionAntigravityAgentConfig : InteractionAgentConfig
{
    public InteractionAntigravityAgentConfig()
    {
        Type = "antigravity";
    }

    [JsonPropertyName("max_total_tokens")]
    public int? MaxTotalTokens { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }
}

public sealed class InteractionUnknownAgentConfig : InteractionAgentConfig
{
}

public sealed class InteractionDeepResearchAgentConfig : InteractionAgentConfig
{
    public InteractionDeepResearchAgentConfig()
    {
        Type = "deep-research";
    }

    [JsonPropertyName("thinking_summaries")]
    public string? ThinkingSummaries { get; set; }
}

public sealed class InteractionCodeMenderAgentConfig : InteractionAgentConfig
{
    public InteractionCodeMenderAgentConfig()
    {
        Type = "code-mender";
    }

    [JsonPropertyName("find_request")]
    public JsonElement? FindRequest { get; set; }

    [JsonPropertyName("fix_request")]
    public JsonElement? FixRequest { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("session_config")]
    public InteractionCodeMenderSessionConfig? SessionConfig { get; set; }

    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }
}

public sealed class InteractionCodeMenderSessionConfig
{
    [JsonPropertyName("max_rounds")]
    public int? MaxRounds { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
