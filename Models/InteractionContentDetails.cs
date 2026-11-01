using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionModelOutputError
{
    [JsonPropertyName("code")]
    public int? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("details")]
    public List<JsonElement>? Details { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionAnnotation
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("start_index")]
    public int? StartIndex { get; set; }

    [JsonPropertyName("end_index")]
    public int? EndIndex { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionCodeExecutionCallArguments
{
    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionUrlContextCallArguments
{
    [JsonPropertyName("urls")]
    public List<string>? Urls { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionGoogleSearchCallArguments
{
    [JsonPropertyName("queries")]
    public List<string>? Queries { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionGoogleMapsCallArguments
{
    [JsonPropertyName("queries")]
    public List<string>? Queries { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionUrlContextResult
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionGoogleSearchResult
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("search_suggestions")]
    public string? SearchSuggestions { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionFileSearchResult
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("file_search_store")]
    public string? FileSearchStore { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionGoogleMapsResult
{
    [JsonPropertyName("places")]
    public List<InteractionPlace>? Places { get; set; }

    [JsonPropertyName("widget_context_token")]
    public string? WidgetContextToken { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionPlace
{
    [JsonPropertyName("place_id")]
    public string? PlaceId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("review_snippets")]
    public List<InteractionReviewSnippet>? ReviewSnippets { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionReviewSnippet
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("review_id")]
    public string? ReviewId { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
