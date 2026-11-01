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
        ["video"] = typeof(InteractionVideoContent)
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
        ["thought"] = typeof(InteractionThoughtStep),
        ["function_call"] = typeof(InteractionFunctionCallStep),
        ["code_execution_call"] = typeof(InteractionCodeExecutionCallStep),
        ["url_context_call"] = typeof(InteractionUrlContextCallStep),
        ["mcp_server_tool_call"] = typeof(InteractionMcpServerToolCallStep),
        ["google_search_call"] = typeof(InteractionGoogleSearchCallStep),
        ["file_search_call"] = typeof(InteractionFileSearchCallStep),
        ["google_maps_call"] = typeof(InteractionGoogleMapsCallStep),
        ["function_result"] = typeof(InteractionFunctionResultStep),
        ["code_execution_result"] = typeof(InteractionCodeExecutionResultStep),
        ["url_context_result"] = typeof(InteractionUrlContextResultStep),
        ["google_search_result"] = typeof(InteractionGoogleSearchResultStep),
        ["mcp_server_tool_result"] = typeof(InteractionMcpServerToolResultStep),
        ["file_search_result"] = typeof(InteractionFileSearchResultStep),
        ["google_maps_result"] = typeof(InteractionGoogleMapsResultStep),
        ["retrieval_call"] = typeof(InteractionRetrievalCallStep),
        ["retrieval_result"] = typeof(InteractionRetrievalResultStep),
        ["processing_call"] = typeof(InteractionProcessingCallStep),
        ["processing_result"] = typeof(InteractionProcessingResultStep)
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
