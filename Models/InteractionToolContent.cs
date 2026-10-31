using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionFunctionCallContent : InteractionContent
{
    public InteractionFunctionCallContent()
    {
        Type = "function_call";
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("arguments")]
    public object? Arguments { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

}

public sealed class InteractionCodeExecutionCallContent : InteractionContent
{
    public InteractionCodeExecutionCallContent()
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

public sealed class InteractionUrlContextCallContent : InteractionContent
{
    public InteractionUrlContextCallContent()
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

public sealed class InteractionMcpServerToolCallContent : InteractionContent
{
    public InteractionMcpServerToolCallContent()
    {
        Type = "mcp_server_tool_call";
    }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("server_name")]
    public string? ServerName { get; set; }

    [JsonPropertyName("arguments")]
    public object? Arguments { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionGoogleSearchCallContent : InteractionContent
{
    public InteractionGoogleSearchCallContent()
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

public sealed class InteractionFileSearchCallContent : InteractionContent
{
    public InteractionFileSearchCallContent()
    {
        Type = "file_search_call";
    }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionGoogleMapsCallContent : InteractionContent
{
    public InteractionGoogleMapsCallContent()
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

public sealed class InteractionFunctionResultContent : InteractionContent
{
    public InteractionFunctionResultContent()
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
    public object? Result { get; set; }

}

public sealed class InteractionCodeExecutionResultContent : InteractionContent
{
    public InteractionCodeExecutionResultContent()
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

public sealed class InteractionUrlContextResultContent : InteractionContent
{
    public InteractionUrlContextResultContent()
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

public sealed class InteractionGoogleSearchResultContent : InteractionContent
{
    public InteractionGoogleSearchResultContent()
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

public sealed class InteractionMcpServerToolResultContent : InteractionContent
{
    public InteractionMcpServerToolResultContent()
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
    public object? Result { get; set; }
}

public sealed class InteractionFileSearchResultContent : InteractionContent
{
    public InteractionFileSearchResultContent()
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

public sealed class InteractionGoogleMapsResultContent : InteractionContent
{
    public InteractionGoogleMapsResultContent()
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

public sealed class InteractionRetrievalCallContent : InteractionContent
{
    public InteractionRetrievalCallContent()
    {
        Type = "retrieval_call";
    }

    [JsonPropertyName("arguments")]
    public object? Arguments { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("retrieval_type")]
    public string? RetrievalType { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionRetrievalResultContent : InteractionContent
{
    public InteractionRetrievalResultContent()
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

public sealed class InteractionProcessingCallContent : InteractionContent
{
    public InteractionProcessingCallContent()
    {
        Type = "processing_call";
    }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public sealed class InteractionProcessingResultContent : InteractionContent
{
    public InteractionProcessingResultContent()
    {
        Type = "processing_result";
    }

    [JsonPropertyName("call_id")]
    public string? CallId { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}
