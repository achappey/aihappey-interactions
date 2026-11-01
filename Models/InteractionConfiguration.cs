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

    [JsonPropertyName("temperature")]
    [Obsolete("Google marks temperature as deprecated in the current Interactions API schema.")]
    public double? Temperature { get; set; }

    [JsonPropertyName("top_p")]
    [Obsolete("Google marks top_p as deprecated in the current Interactions API schema.")]
    public double? TopP { get; set; }

    [JsonPropertyName("speech_config")]
    [JsonConverter(typeof(InteractionSpeechConfigListJsonConverter))]
    public List<InteractionSpeechConfig>? SpeechConfig { get; set; }

    [JsonPropertyName("image_config")]
    [Obsolete("Google marks image_config as deprecated in the current Interactions API schema; use an image response_format.")]
    public InteractionImageConfig? ImageConfig { get; set; }

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
    [JsonPropertyName("adaptation_phrases")]
    [Obsolete("Google marks adaptation_phrases as deprecated in the current Interactions API schema.")]
    public List<string>? AdaptationPhrases { get; set; }

    [JsonPropertyName("custom_vocabulary")]
    public List<string>? CustomVocabulary { get; set; }

    [JsonPropertyName("diarization_mode")]
    [Obsolete("Google marks diarization_mode as deprecated in the current Interactions API schema.")]
    public string? DiarizationMode { get; set; }

    [JsonPropertyName("language_codes")]
    public List<string>? LanguageCodes { get; set; }

    [JsonPropertyName("language_hints")]
    [Obsolete("Use LanguageCodes; Google marks language_hints as deprecated.")]
    public List<string>? LanguageHints { get; set; }

    [JsonPropertyName("mode")]
    public InteractionTranscriptionMode? Mode { get; set; }

    [JsonPropertyName("timestamp_granularities")]
    [Obsolete("Google marks timestamp_granularities as deprecated in the current Interactions API schema.")]
    public List<string>? TimestampGranularities { get; set; }

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

public sealed class InteractionImageConfig
{
    [JsonPropertyName("aspect_ratio")]
    public string? AspectRatio { get; set; }

    [JsonPropertyName("image_size")]
    public string? ImageSize { get; set; }

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
