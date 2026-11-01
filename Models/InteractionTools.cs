using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionSafetySetting
{
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("threshold")]
    public string? Threshold { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

[JsonConverter(typeof(InteractionToolJsonConverter))]
public abstract class InteractionTool
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionUnknownTool : InteractionTool
{
}



public static class ProviderMetadataExtensions
{
    public static List<InteractionTool>? GetInteractionToolDefinitions(
        this Dictionary<string, object?>? metadata,
        string providerId)
    {
        if (metadata is null)
            return null;

        if (!metadata.TryGetValue(providerId, out var providerObj))
            return null;

        if (providerObj is not JsonElement providerJson ||
            providerJson.ValueKind != JsonValueKind.Object)
            return null;

        if (!providerJson.TryGetProperty("tools", out var toolsEl) ||
            toolsEl.ValueKind != JsonValueKind.Array)
            return null;

        var result = new List<InteractionTool>();

        foreach (var toolEl in toolsEl.EnumerateArray())
        {
            var def = toolEl.Deserialize<InteractionTool>(InteractionJson.Default)
                ?? throw new JsonException("Interaction tool definition must be a non-null object.");
            result.Add(def);
        }

        return result.Count > 0 ? result : null;
    }
}

public sealed class InteractionFunctionTool : InteractionTool
{
    public InteractionFunctionTool()
    {
        Type = "function";
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("parameters")]
    public JsonElement? Parameters { get; set; }
}

public sealed class InteractionCodeExecutionTool : InteractionTool
{
    public InteractionCodeExecutionTool()
    {
        Type = "code_execution";
    }
}

public sealed class InteractionUrlContextTool : InteractionTool
{
    public InteractionUrlContextTool()
    {
        Type = "url_context";
    }
}

public sealed class InteractionComputerUseTool : InteractionTool
{
    public InteractionComputerUseTool()
    {
        Type = "computer_use";
    }

    [JsonPropertyName("environment")]
    public string? Environment { get; set; }

    [JsonPropertyName("excluded_predefined_functions")]
    public List<string>? ExcludedPredefinedFunctions { get; set; }

    [JsonPropertyName("disabled_safety_policies")]
    public List<string>? DisabledSafetyPolicies { get; set; }

    [JsonPropertyName("enable_prompt_injection_detection")]
    public bool? EnablePromptInjectionDetection { get; set; }
}

public sealed class InteractionMcpServerTool : InteractionTool
{
    public InteractionMcpServerTool()
    {
        Type = "mcp_server";
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; set; }

    [JsonPropertyName("allowed_tools")]
    public List<InteractionAllowedTools>? AllowedTools { get; set; }
}

public sealed class InteractionGoogleSearchTool : InteractionTool
{
    public InteractionGoogleSearchTool()
    {
        Type = "google_search";
    }

    [JsonPropertyName("search_types")]
    public List<string>? SearchTypes { get; set; }
}

public sealed class InteractionFileSearchTool : InteractionTool
{
    public InteractionFileSearchTool()
    {
        Type = "file_search";
    }

    [JsonPropertyName("file_search_store_names")]
    public List<string>? FileSearchStoreNames { get; set; }

    [JsonPropertyName("top_k")]
    public int? TopK { get; set; }

    [JsonPropertyName("metadata_filter")]
    public string? MetadataFilter { get; set; }
}

public sealed class InteractionGoogleMapsTool : InteractionTool
{
    public InteractionGoogleMapsTool()
    {
        Type = "google_maps";
    }

    [JsonPropertyName("enable_widget")]
    public bool? EnableWidget { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }
}

public sealed class InteractionRetrievalTool : InteractionTool
{
    public InteractionRetrievalTool()
    {
        Type = "retrieval";
    }

    [JsonPropertyName("retrieval_types")]
    public List<string>? RetrievalTypes { get; set; }

    [JsonPropertyName("vertex_ai_search_config")]
    public InteractionVertexAiSearchConfig? VertexAiSearchConfig { get; set; }
}

public sealed class InteractionAllowedTools
{
    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("tools")]
    public List<string>? Tools { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

public sealed class InteractionVertexAiSearchConfig
{
    [JsonPropertyName("engine")]
    public string? Engine { get; set; }

    [JsonPropertyName("datastores")]
    public List<string>? Datastores { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}
