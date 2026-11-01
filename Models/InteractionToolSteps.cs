using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionFunctionCallStep : InteractionStep
{
    public InteractionFunctionCallStep()
    {
        Type = "function_call";
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("arguments")]
    public JsonElement? Arguments { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

}

public sealed class InteractionCodeExecutionCallStep : InteractionStep
{
    public InteractionCodeExecutionCallStep()
    {
        Type = "code_execution_call";
    }

    [JsonPropertyName("arguments")]
    public InteractionCodeExecutionCallArguments? Arguments { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionUrlContextCallStep : InteractionStep
{
    public InteractionUrlContextCallStep()
    {
        Type = "url_context_call";
    }

    [JsonPropertyName("arguments")]
    public InteractionUrlContextCallArguments? Arguments { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionMcpServerToolCallStep : InteractionStep
{
    public InteractionMcpServerToolCallStep()
    {
        Type = "mcp_server_tool_call";
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("server_name")]
    public string? ServerName { get; set; }

    [JsonPropertyName("arguments")]
    public JsonElement? Arguments { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionGoogleSearchCallStep : InteractionStep
{
    public InteractionGoogleSearchCallStep()
    {
        Type = "google_search_call";
    }

    [JsonPropertyName("arguments")]
    public InteractionGoogleSearchCallArguments? Arguments { get; set; }

    [JsonPropertyName("search_type")]
    public string? SearchType { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionFileSearchCallStep : InteractionStep
{
    public InteractionFileSearchCallStep()
    {
        Type = "file_search_call";
    }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionGoogleMapsCallStep : InteractionStep
{
    public InteractionGoogleMapsCallStep()
    {
        Type = "google_maps_call";
    }

    [JsonPropertyName("arguments")]
    public InteractionGoogleMapsCallArguments? Arguments { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionFunctionResultStep : InteractionStep
{
    public InteractionFunctionResultStep()
    {
        Type = "function_result";
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("is_error")]
    public bool? IsError { get; set; }

    [JsonPropertyName("call_id")]
    public string? CallId { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    [JsonPropertyName("result")]
    public JsonElement? Result { get; set; }

}

public sealed class InteractionCodeExecutionResultStep : InteractionStep
{
    public InteractionCodeExecutionResultStep()
    {
        Type = "code_execution_result";
    }

    [JsonPropertyName("result")]
    public string? Result { get; set; }

    [JsonPropertyName("is_error")]
    public bool? IsError { get; set; }

    [JsonPropertyName("call_id")]
    public string? CallId { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionUrlContextResultStep : InteractionStep
{
    public InteractionUrlContextResultStep()
    {
        Type = "url_context_result";
    }

    [JsonPropertyName("result")]
    public List<InteractionUrlContextResult>? Result { get; set; }

    [JsonPropertyName("is_error")]
    public bool? IsError { get; set; }

    [JsonPropertyName("call_id")]
    public string? CallId { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionGoogleSearchResultStep : InteractionStep
{
    public InteractionGoogleSearchResultStep()
    {
        Type = "google_search_result";
    }

    [JsonPropertyName("result")]
    public List<InteractionGoogleSearchResult>? Result { get; set; }

    [JsonPropertyName("is_error")]
    public bool? IsError { get; set; }

    [JsonPropertyName("call_id")]
    public string? CallId { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionMcpServerToolResultStep : InteractionStep
{
    public InteractionMcpServerToolResultStep()
    {
        Type = "mcp_server_tool_result";
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("server_name")]
    public string? ServerName { get; set; }

    [JsonPropertyName("call_id")]
    public string? CallId { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    [JsonPropertyName("result")]
    public JsonElement? Result { get; set; }
}

public sealed class InteractionFileSearchResultStep : InteractionStep
{
    public InteractionFileSearchResultStep()
    {
        Type = "file_search_result";
    }

    [JsonPropertyName("result")]
    public List<InteractionFileSearchResult>? Result { get; set; }

    [JsonPropertyName("call_id")]
    public string? CallId { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionGoogleMapsResultStep : InteractionStep
{
    public InteractionGoogleMapsResultStep()
    {
        Type = "google_maps_result";
    }

    [JsonPropertyName("result")]
    public List<InteractionGoogleMapsResult>? Result { get; set; }

    [JsonPropertyName("call_id")]
    public string? CallId { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionRetrievalCallStep : InteractionStep
{
    public InteractionRetrievalCallStep()
    {
        Type = "retrieval_call";
    }

    [JsonPropertyName("arguments")]
    public JsonElement? Arguments { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("retrieval_type")]
    public string? RetrievalType { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionRetrievalResultStep : InteractionStep
{
    public InteractionRetrievalResultStep()
    {
        Type = "retrieval_result";
    }

    [JsonPropertyName("call_id")]
    public string? CallId { get; set; }

    [JsonPropertyName("is_error")]
    public bool? IsError { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionProcessingCallStep : InteractionStep
{
    public InteractionProcessingCallStep()
    {
        Type = "processing_call";
    }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionProcessingResultStep : InteractionStep
{
    public InteractionProcessingResultStep()
    {
        Type = "processing_result";
    }

    [JsonPropertyName("call_id")]
    public string? CallId { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}
