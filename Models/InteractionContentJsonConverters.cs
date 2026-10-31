using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public sealed class InteractionContentJsonConverter : JsonConverter<InteractionContent>
{
    private static readonly Dictionary<string, Type> ContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["text"] = typeof(InteractionTextContent),
        ["image"] = typeof(InteractionImageContent),
        ["audio"] = typeof(InteractionAudioContent),
        ["document"] = typeof(InteractionDocumentContent),
        ["video"] = typeof(InteractionVideoContent),
        ["thought"] = typeof(InteractionThoughtContent),
        ["function_call"] = typeof(InteractionFunctionCallContent),
        ["code_execution_call"] = typeof(InteractionCodeExecutionCallContent),
        ["url_context_call"] = typeof(InteractionUrlContextCallContent),
        ["mcp_server_tool_call"] = typeof(InteractionMcpServerToolCallContent),
        ["google_search_call"] = typeof(InteractionGoogleSearchCallContent),
        ["file_search_call"] = typeof(InteractionFileSearchCallContent),
        ["google_maps_call"] = typeof(InteractionGoogleMapsCallContent),
        ["function_result"] = typeof(InteractionFunctionResultContent),
        ["code_execution_result"] = typeof(InteractionCodeExecutionResultContent),
        ["url_context_result"] = typeof(InteractionUrlContextResultContent),
        ["google_search_result"] = typeof(InteractionGoogleSearchResultContent),
        ["mcp_server_tool_result"] = typeof(InteractionMcpServerToolResultContent),
        ["file_search_result"] = typeof(InteractionFileSearchResultContent),
        ["google_maps_result"] = typeof(InteractionGoogleMapsResultContent),
        ["retrieval_call"] = typeof(InteractionRetrievalCallContent),
        ["retrieval_result"] = typeof(InteractionRetrievalResultContent),
        ["processing_call"] = typeof(InteractionProcessingCallContent),
        ["processing_result"] = typeof(InteractionProcessingResultContent)
    };

    public override InteractionContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("Interaction content must be an object.");

        var type = root.TryGetProperty("type", out var typeProp) ? typeProp.GetString() : null;
        if (string.IsNullOrWhiteSpace(type) || !ContentTypes.TryGetValue(type, out var targetType))
            targetType = typeof(InteractionUnknownContent);

        return JsonSerializer.Deserialize(root.GetRawText(), targetType, options) as InteractionContent;
    }

    public override void Write(Utf8JsonWriter writer, InteractionContent value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}
 
public sealed class InteractionStepJsonConverter : JsonConverter<InteractionStep>
{
    private static readonly Dictionary<string, Type> StepTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["user_input"] = typeof(InteractionUserInputStep),
        ["model_output"] = typeof(InteractionModelOutputStep),
        ["text"] = typeof(InteractionTextContent),
        ["image"] = typeof(InteractionImageContent),
        ["audio"] = typeof(InteractionAudioContent),
        ["document"] = typeof(InteractionDocumentContent),
        ["video"] = typeof(InteractionVideoContent),
        ["thought"] = typeof(InteractionThoughtContent),
        ["function_call"] = typeof(InteractionFunctionCallContent),
        ["code_execution_call"] = typeof(InteractionCodeExecutionCallContent),
        ["url_context_call"] = typeof(InteractionUrlContextCallContent),
        ["mcp_server_tool_call"] = typeof(InteractionMcpServerToolCallContent),
        ["google_search_call"] = typeof(InteractionGoogleSearchCallContent),
        ["file_search_call"] = typeof(InteractionFileSearchCallContent),
        ["google_maps_call"] = typeof(InteractionGoogleMapsCallContent),
        ["function_result"] = typeof(InteractionFunctionResultContent),
        ["code_execution_result"] = typeof(InteractionCodeExecutionResultContent),
        ["url_context_result"] = typeof(InteractionUrlContextResultContent),
        ["google_search_result"] = typeof(InteractionGoogleSearchResultContent),
        ["mcp_server_tool_result"] = typeof(InteractionMcpServerToolResultContent),
        ["file_search_result"] = typeof(InteractionFileSearchResultContent),
        ["google_maps_result"] = typeof(InteractionGoogleMapsResultContent),
        ["retrieval_call"] = typeof(InteractionRetrievalCallContent),
        ["retrieval_result"] = typeof(InteractionRetrievalResultContent),
        ["processing_call"] = typeof(InteractionProcessingCallContent),
        ["processing_result"] = typeof(InteractionProcessingResultContent)
    };
 
    public override InteractionStep? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
 
        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("Interaction step must be an object.");
 
        var type = root.TryGetProperty("type", out var typeProp) ? typeProp.GetString() : null;
        if (string.IsNullOrWhiteSpace(type) || !StepTypes.TryGetValue(type, out var targetType))
            targetType = typeof(InteractionUnknownStep);
 
        return JsonSerializer.Deserialize(root.GetRawText(), targetType, options) as InteractionStep;
    }
 
    public override void Write(Utf8JsonWriter writer, InteractionStep value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}
