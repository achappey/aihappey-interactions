using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

[JsonConverter(typeof(InteractionStepJsonConverter))]
public abstract class InteractionStep
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
 
public sealed class InteractionUserInputStep : InteractionStep
{
    public InteractionUserInputStep()
    {
        Type = "user_input";
    }
 
    [JsonPropertyName("content")]
    public List<InteractionContent>? Content { get; set; }
}
 
public sealed class InteractionModelOutputStep : InteractionStep
{
    public InteractionModelOutputStep()
    {
        Type = "model_output";
    }
 
    [JsonPropertyName("content")]
    public List<InteractionContent>? Content { get; set; }

    [JsonPropertyName("error")]
    public object? Error { get; set; }
}
 
public sealed class InteractionUnknownStep : InteractionStep
{
}
 
[JsonConverter(typeof(InteractionContentJsonConverter))]
public abstract class InteractionContent : InteractionStep
{
}

public sealed class InteractionUnknownContent : InteractionContent
{
}

public sealed class InteractionTextContent : InteractionContent
{
    public InteractionTextContent()
    {
        Type = "text";
    }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("annotations")]
    public List<InteractionAnnotation>? Annotations { get; set; }
}

public sealed class InteractionImageContent : InteractionContent
{
    public InteractionImageContent()
    {
        Type = "image";
    }

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }

    [JsonPropertyName("resolution")]
    public string? Resolution { get; set; }
}

public sealed class InteractionAudioContent : InteractionContent
{
    public InteractionAudioContent()
    {
        Type = "audio";
    }

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }

    [JsonPropertyName("sample_rate")]
    public int? SampleRate { get; set; }

    [JsonPropertyName("rate")]
    [Obsolete("Use SampleRate, which matches the Interactions API field sample_rate.")]
    [JsonIgnore]
    public int? Rate { get; set; }

    [JsonPropertyName("channels")]
    public int? Channels { get; set; }
}

public sealed class InteractionDocumentContent : InteractionContent
{
    public InteractionDocumentContent()
    {
        Type = "document";
    }

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }
}

public sealed class InteractionVideoContent : InteractionContent
{
    public InteractionVideoContent()
    {
        Type = "video";
    }

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }

    [JsonPropertyName("resolution")]
    public string? Resolution { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("processing")]
    public object? Processing { get; set; }
}

public sealed class InteractionThoughtContent : InteractionContent
{
    public InteractionThoughtContent()
    {
        Type = "thought";
    }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    [JsonPropertyName("summary")]
    public List<InteractionContent>? Summary { get; set; }
}
