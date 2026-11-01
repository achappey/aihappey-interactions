using System.Text.Json;
using AIHappey.Interactions;
using AIHappey.Interactions.Extensions;

var tests = new (string Name, Action Run)[]
{
    ("request fields and extension data", RequestFieldsAndExtensionData),
    ("typed environment, webhook and tool-choice options", TypedRequestOptions),
    ("structured response format variants", ResponseFormatVariants),
    ("interaction state fields", InteractionStateFields),
    ("speaker config object and array forms", SpeechConfigForms),
    ("speech transcription and video config", MultimodalGenerationConfig),
    ("multimodal content fields", MultimodalContent),
    ("new and unknown content discriminators", ContentDiscriminators),
    ("tool discriminator and property names", ToolDiscriminators),
    ("provider tool metadata passthrough", ProviderToolMetadata),
    ("unknown agent config discriminator", UnknownAgentConfig),
    ("interaction status update event", StatusUpdateEvent),
    ("open interaction statuses", OpenInteractionStatuses),
    ("modern SSE lifecycle", StreamingSseEvents),
    ("stateless steps and tool results", StatelessStepsAndToolResults),
    ("modern HTTP contract and endpoint parity", HttpContract),
    ("streaming errors and unknown events", StreamingErrorsAndUnknownEvents),
    ("typed streaming deltas and metadata", TypedStreamingDeltas),
    ("usage fields", UsageFields)
};

foreach (var (name, run) in tests)
{
    run();
    Console.WriteLine($"PASS {name}");
}

static void RequestFieldsAndExtensionData()
{
    var request = JsonSerializer.Deserialize<InteractionRequest>(
        """
        {
          "model":"gemini-flash-latest",
          "input":"hello",
          "continuation_token":"resume-me",
          "labels":{"trace":"test"},
          "environment":"env-1",
          "response_format":{"type":"text","mime_type":"application/json"},
          "safety_settings":[{"category":"HARM_CATEGORY_HARASSMENT","threshold":"BLOCK_ONLY_HIGH"}],
          "webhook_config":{"uris":["https://example.invalid/hook"]},
          "generation_config":{"thinking_level":"high","max_output_tokens":1024,"unknown_generation_field":true},
          "future_request_field":{"enabled":true}
        }
        """,
        InteractionJson.Default)!;

    Equal("resume-me", request.ContinuationToken);
    Equal("test", request.Labels!["trace"]);
    Equal("env-1", request.Environment!.Reference);
    Equal("high", request.GenerationConfig!.ThinkingLevel);
    Equal(1024, request.GenerationConfig.MaxOutputTokens);
    True(request.AdditionalProperties!.ContainsKey("future_request_field"));
    True(request.GenerationConfig.AdditionalProperties!.ContainsKey("unknown_generation_field"));

    using var serialized = JsonDocument.Parse(
        JsonSerializer.Serialize(request, InteractionJson.Default));
    Equal("resume-me", serialized.RootElement.GetProperty("continuation_token").GetString());
    True(serialized.RootElement.TryGetProperty("safety_settings", out _));
}

static void ResponseFormatVariants()
{
    var request = JsonSerializer.Deserialize<InteractionRequest>(
        """
        {
          "response_format":[
            {"type":"text","mime_type":"application/json","schema":{"type":"object","properties":{"answer":{"type":"string"}}}},
            {"type":"image","aspect_ratio":"16:9","image_size":"1K"},
            {"type":"audio","mime_type":"audio/wav","sample_rate":24000},
            {"type":"video","resolution":"720p","delivery":"uri"},
            {"type":"future","vendor_option":true}
          ]
        }
        """,
        InteractionJson.Default)!;

    var formats = request.ResponseFormat!.Multiple!;
    True(formats[0] is InteractionTextResponseFormat);
    var text = (InteractionTextResponseFormat)formats[0];
    Equal("application/json", text.MimeType);
    Equal("object", text.Schema!.Value.GetProperty("type").GetString());
    True(formats[1] is InteractionImageResponseFormat);
    Equal("1K", ((InteractionImageResponseFormat)formats[1]).ImageSize);
    True(formats[2] is InteractionAudioResponseFormat);
    Equal(24000, ((InteractionAudioResponseFormat)formats[2]).SampleRate);
    True(formats[3] is InteractionVideoResponseFormat);
    Equal("720p", ((InteractionVideoResponseFormat)formats[3]).Resolution);
    True(formats[4] is InteractionUnknownResponseFormat);

    using var serialized = JsonDocument.Parse(JsonSerializer.Serialize(request, InteractionJson.Default));
    var responseFormats = serialized.RootElement.GetProperty("response_format");
    Equal(5, responseFormats.GetArrayLength());
    Equal("text", responseFormats[0].GetProperty("type").GetString());
    True(responseFormats[0].GetProperty("schema").GetProperty("properties").TryGetProperty("answer", out _));
    True(responseFormats[4].GetProperty("vendor_option").GetBoolean());

    var single = JsonSerializer.Deserialize<Interaction>(
        """{"response_format":{"type":"text","mime_type":"text/plain"}}""",
        InteractionJson.Default)!;
    True(single.ResponseFormat!.Single is InteractionTextResponseFormat);
    using var singleRoundTrip = JsonDocument.Parse(JsonSerializer.Serialize(single, InteractionJson.Default));
    Equal(JsonValueKind.Object, singleRoundTrip.RootElement.GetProperty("response_format").ValueKind);
}

static void TypedRequestOptions()
{
    var request = JsonSerializer.Deserialize<InteractionRequest>(
        """
        {
          "environment":{
            "type":"remote",
            "environment_id":"env-2",
            "env":{"MODE":"test"},
            "network":"disabled",
            "sources":[{"type":"gcs","source":"gs://bucket/data","target":"/workspace/data"}],
            "future_environment_field":true
          },
          "webhook_config":{"uris":["https://example.invalid/events"],"user_metadata":{"trace":"abc"}},
          "generation_config":{"tool_choice":{"allowed_tools":{"mode":"any","tools":["search"]}}}
        }
        """,
        InteractionJson.Default)!;

    var environment = request.Environment!.Configuration!;
    Equal("env-2", environment.EnvironmentId);
    Equal("test", environment.EnvironmentVariables!["MODE"].GetString());
    Equal("disabled", environment.Network!.Value.GetString());
    Equal("/workspace/data", environment.Sources![0].Target);
    True(environment.AdditionalProperties!.ContainsKey("future_environment_field"));
    Equal("https://example.invalid/events", request.WebhookConfig!.Uris![0]);
    Equal("abc", request.WebhookConfig.UserMetadata!["trace"].GetString());
    Equal("any", request.GenerationConfig!.ToolChoice!.Configuration!.AllowedTools!.Mode);

    using var serialized = JsonDocument.Parse(JsonSerializer.Serialize(request, InteractionJson.Default));
    Equal("remote", serialized.RootElement.GetProperty("environment").GetProperty("type").GetString());
    Equal("abc", serialized.RootElement.GetProperty("webhook_config").GetProperty("user_metadata")
        .GetProperty("trace").GetString());
    Equal("any", serialized.RootElement.GetProperty("generation_config").GetProperty("tool_choice")
        .GetProperty("allowed_tools").GetProperty("mode").GetString());

    var environmentReference = JsonSerializer.Deserialize<InteractionRequest>(
        """{"environment":"env-reference"}""",
        InteractionJson.Default)!;
    Equal("env-reference", environmentReference.Environment!.Reference);

    var unknownEnvironment = JsonSerializer.Deserialize<InteractionRequest>(
        """{"environment":{"type":"future","option":1}}""",
        InteractionJson.Default)!;
    Equal(1, unknownEnvironment.Environment!.Unknown!.Value.GetProperty("option").GetInt32());
    using var unknownEnvironmentRoundTrip = JsonDocument.Parse(
        JsonSerializer.Serialize(unknownEnvironment, InteractionJson.Default));
    Equal("future", unknownEnvironmentRoundTrip.RootElement.GetProperty("environment").GetProperty("type").GetString());
}

static void InteractionStateFields()
{
    var interaction = JsonSerializer.Deserialize<Interaction>(
        """
        {
          "id":"int-1",
          "status":"failed",
          "environment_id":"env-1",
          "errors":[{"code":"https://example.invalid/error","message":"diagnostic"}],
          "steps":[{"type":"model_output","error":{"code":3,"message":"Unavailable","details":[{"reason":"busy"}]}}]
        }
        """,
        InteractionJson.Default)!;

    Equal("env-1", interaction.EnvironmentId);
    Equal("https://example.invalid/error", interaction.Errors![0].Code);
    Equal("diagnostic", interaction.Errors[0].Message);
    var outputError = ((InteractionModelOutputStep)interaction.Steps![0]).Error!;
    Equal(3, outputError.Code);
    Equal("busy", outputError.Details![0].GetProperty("reason").GetString());
}

static void OpenInteractionStatuses()
{
    var interaction = JsonSerializer.Deserialize<Interaction>(
        """{"status":"budget_exceeded"}""",
        InteractionJson.Default)!;
    Equal(InteractionStatusKind.BudgetExceeded, interaction.Status!.Value.KnownValue);

    var unknown = JsonSerializer.Deserialize<Interaction>(
        """{"status":"future_status"}""",
        InteractionJson.Default)!;
    Equal("future_status", unknown.Status!.Value.Value);
    True(unknown.Status.Value.KnownValue is null);

    using var roundTrip = JsonDocument.Parse(JsonSerializer.Serialize(unknown, InteractionJson.Default));
    Equal("future_status", roundTrip.RootElement.GetProperty("status").GetString());
}

static void SpeechConfigForms()
{
    var objectForm = JsonSerializer.Deserialize<InteractionRequest>(
        """
        {"generation_config":{"speech_config":{"speakers":[{"voice":"Kore","speaker":"narrator"}],"future":1}}}
        """,
        InteractionJson.Default)!;
    var speakerConfigs = objectForm.GenerationConfig!.SpeechConfig;
    True(speakerConfigs is InteractionSpeakerConfigCollection);
    Equal("Kore", speakerConfigs![0].Voice);

    using var objectRoundTrip = JsonDocument.Parse(
        JsonSerializer.Serialize(objectForm.GenerationConfig, InteractionJson.Default));
    var objectSpeech = objectRoundTrip.RootElement.GetProperty("speech_config");
    True(objectSpeech.ValueKind == JsonValueKind.Object);
    Equal("narrator", objectSpeech.GetProperty("speakers")[0].GetProperty("speaker").GetString());
    Equal(1, objectSpeech.GetProperty("future").GetInt32());

    var arrayForm = JsonSerializer.Deserialize<InteractionRequest>(
        """{"generation_config":{"speech_config":[{"voice":"Kore"}]}}""",
        InteractionJson.Default)!;
    using var arrayRoundTrip = JsonDocument.Parse(
        JsonSerializer.Serialize(arrayForm.GenerationConfig, InteractionJson.Default));
    True(arrayRoundTrip.RootElement.GetProperty("speech_config").ValueKind == JsonValueKind.Array);
}

static void MultimodalGenerationConfig()
{
    var request = JsonSerializer.Deserialize<InteractionRequest>(
        """
        {"generation_config":{
          "transcription_config":{"language_codes":["en-US"],"mode":"verbatim","future_transcription_field":true},
          "video_config":{"task":"text_to_video","future_video_field":"preserved"}
        }}
        """,
        InteractionJson.Default)!;

    Equal("en-US", request.GenerationConfig!.TranscriptionConfig!.LanguageCodes![0]);
    Equal("verbatim", request.GenerationConfig.TranscriptionConfig.Mode!.Mode);
    Equal("text_to_video", request.GenerationConfig.VideoConfig!.Task);
    True(request.GenerationConfig.TranscriptionConfig.AdditionalProperties!
        .ContainsKey("future_transcription_field"));
    True(request.GenerationConfig.VideoConfig.AdditionalProperties!.ContainsKey("future_video_field"));

    var modeConfig = JsonSerializer.Deserialize<InteractionRequest>(
        """
        {"generation_config":{"transcription_config":{"mode":{"type":"verbatim","diarization_mode":"speaker","timestamp_granularities":["word"]}}}}
        """,
        InteractionJson.Default)!;
    var verbatim = (InteractionVerbatimTranscriptionMode)modeConfig.GenerationConfig!
        .TranscriptionConfig!.Mode!.Configuration!;
    Equal("speaker", verbatim.DiarizationMode);
    Equal("word", verbatim.TimestampGranularities![0]);
}

static void MultimodalContent()
{
    var interaction = JsonSerializer.Deserialize<Interaction>(
        """
        {"steps":[{"type":"model_output","content":[
          {"type":"audio","data":"AQID","mime_type":"audio/pcm","sample_rate":24000,"channels":1},
          {"type":"video","uri":"gs://bucket/clip.mp4","name":"clip","processing":{"type":"static","fps":1.5}}
        ]}]}
        """,
        InteractionJson.Default)!;
    var output = (InteractionModelOutputStep)interaction.Steps![0];
    var audio = (InteractionAudioContent)output.Content![0];
    Equal(24000, audio.SampleRate);
    var video = (InteractionVideoContent)output.Content[1];
    Equal("clip", video.Name);
    Equal(1.5, video.Processing!.StaticConfiguration!.Fps);

    using var serialized = JsonDocument.Parse(JsonSerializer.Serialize(interaction, InteractionJson.Default));
    var audioJson = serialized.RootElement.GetProperty("steps")[0].GetProperty("content")[0];
    Equal(24000, audioJson.GetProperty("sample_rate").GetInt32());
    True(!audioJson.TryGetProperty("rate", out _));
    True(serialized.RootElement.GetProperty("steps")[0].GetProperty("content")[1]
        .TryGetProperty("processing", out _));
}

static void ContentDiscriminators()
{
    var step = JsonSerializer.Deserialize<InteractionStep>(
        """{"type":"retrieval_call","id":"call-1","retrieval_type":"vertex_ai_search","arguments":{"query":"x"}}""",
        InteractionJson.Default)!;
    True(step is InteractionRetrievalCallStep);

    var unknown = JsonSerializer.Deserialize<InteractionContent>(
        """{"type":"future_media","text":"not text","payload":{"x":1}}""",
        InteractionJson.Default)!;
    True(unknown is InteractionUnknownContent);

    using var roundTrip = JsonDocument.Parse(JsonSerializer.Serialize(unknown, InteractionJson.Default));
    Equal("future_media", roundTrip.RootElement.GetProperty("type").GetString());
    True(roundTrip.RootElement.TryGetProperty("payload", out _));

    var contentInput = JsonSerializer.Deserialize<InteractionRequest>(
        """{"input":[{"type":"text","text":"hello"}]}""",
        InteractionJson.Default)!;
    True(contentInput.Input!.IsContent);
    True(contentInput.Input.Content![0] is InteractionTextContent);

    var thoughtStep = JsonSerializer.Deserialize<Interaction>(
        """{"steps":[{"type":"thought","signature":"sig","summary":[{"type":"text","text":"summary"}]}]}""",
        InteractionJson.Default)!;
    True(thoughtStep.Steps![0] is InteractionThoughtStep);
    Equal("summary", ((InteractionTextContent)((InteractionThoughtStep)thoughtStep.Steps[0]).Summary![0]).Text);

    var futureInput = JsonSerializer.Deserialize<InteractionRequest>(
        """{"input":[{"type":"future_input","data":{"x":1}}]}""",
        InteractionJson.Default)!;
    True(futureInput.Input!.IsRaw);
    using var futureInputRoundTrip = JsonDocument.Parse(
        JsonSerializer.Serialize(futureInput, InteractionJson.Default));
    True(futureInputRoundTrip.RootElement.GetProperty("input")[0].GetProperty("data").TryGetProperty("x", out _));
}

static void ToolDiscriminators()
{
    var unknown = JsonSerializer.Deserialize<InteractionTool>(
        """{"type":"future_tool","name":"future","configuration":{"enabled":true}}""",
        InteractionJson.Default)!;
    True(unknown is InteractionUnknownTool);

    var computerUse = new InteractionComputerUseTool
    {
        Environment = "browser",
        ExcludedPredefinedFunctions = ["click"]
    };
    using var serialized = JsonDocument.Parse(JsonSerializer.Serialize<InteractionTool>(
        computerUse,
        InteractionJson.Default));
    True(serialized.RootElement.TryGetProperty("excluded_predefined_functions", out _));
    True(!serialized.RootElement.TryGetProperty("excludedPredefinedFunctions", out _));

    using var unknownRoundTrip = JsonDocument.Parse(JsonSerializer.Serialize(unknown, InteractionJson.Default));
    Equal("future_tool", unknownRoundTrip.RootElement.GetProperty("type").GetString());
    True(unknownRoundTrip.RootElement.TryGetProperty("configuration", out _));
}

static void UnknownAgentConfig()
{
    var request = JsonSerializer.Deserialize<InteractionRequest>(
        """{"agent_config":{"type":"future_agent","setting":{"enabled":true}}}""",
        InteractionJson.Default)!;
    True(request.AgentConfig is InteractionUnknownAgentConfig);

    using var serialized = JsonDocument.Parse(
        JsonSerializer.Serialize(request.AgentConfig, InteractionJson.Default));
    Equal("future_agent", serialized.RootElement.GetProperty("type").GetString());
    True(serialized.RootElement.TryGetProperty("setting", out _));
}

static void ProviderToolMetadata()
{
    var providerJson = JsonSerializer.SerializeToElement(new
    {
        tools = new object[]
        {
            new { type = "function", name = "lookup", parameters = new { type = "object" }, provider_option = true },
            new { type = "future_tool", provider_config = new { enabled = true } }
        }
    });
    var metadata = new Dictionary<string, object?> { ["google"] = providerJson };
    var tools = metadata.GetInteractionToolDefinitions("google")!;
    Equal(2, tools.Count);
    Equal("lookup", ((InteractionFunctionTool)tools[0]).Name);
    True(tools[0].AdditionalProperties!["provider_option"].GetBoolean());
    True(tools[1] is InteractionUnknownTool);
    using var roundTrip = JsonDocument.Parse(JsonSerializer.Serialize(tools, InteractionJson.Default));
    True(JsonElement.DeepEquals(providerJson.GetProperty("tools"), roundTrip.RootElement));

    foreach (var invalid in new[] { """{"tools":[42]}""", """{"tools":[null]}""" })
    {
        using var document = JsonDocument.Parse(invalid);
        metadata["google"] = document.RootElement.Clone();
        Throws<JsonException>(() => metadata.GetInteractionToolDefinitions("google"));
    }
}

static void StatusUpdateEvent()
{
    var statusEvent = JsonSerializer.Deserialize<InteractionStreamEventPart>(
        """
        {"event_type":"interaction.status_update","interaction_id":"int-1","status":"requires_continuation","continuation_token":"next","usage":{"initial_input_tokens":4}}
        """,
        InteractionJson.Default)!;
    True(statusEvent is InteractionStatusEvent);
    var status = (InteractionStatusEvent)statusEvent;
    Equal("next", status.ContinuationToken);
    Equal(4, status.Usage!.InitialInputTokens);

    using var serialized = JsonDocument.Parse(JsonSerializer.Serialize(status, InteractionJson.Default));
    Equal("interaction.status_update", serialized.RootElement.GetProperty("event_type").GetString());

    var actionEvent = JsonSerializer.Deserialize<InteractionStreamEventPart>(
        """{"event_type":"interaction.status_update","interaction_id":"int-1","status":"requires_action"}""",
        InteractionJson.Default)!;
    Equal(InteractionStatusKind.RequiresAction, ((InteractionStatusEvent)actionEvent).Status!.Value.KnownValue);
}

static void TypedStreamingDeltas()
{
    var textEvent = JsonSerializer.Deserialize<InteractionStreamEventPart>(
        """
        {"event_type":"step.delta","index":1,"delta":{"type":"text","text":"hello"},"metadata":{"total_usage":{"total_tokens":17}}}
        """,
        InteractionJson.Default)!;
    var textDeltaEvent = (InteractionStepDeltaEvent)textEvent;
    True(textDeltaEvent.Delta is InteractionTextDeltaData);
    Equal("hello", ((InteractionTextDeltaData)textDeltaEvent.Delta!).Text);
    Equal(17, textDeltaEvent.Metadata!.TotalUsage!.TotalTokens);

    var argumentsEvent = JsonSerializer.Deserialize<InteractionStreamEventPart>(
        """{"event_type":"step.delta","index":2,"delta":{"type":"arguments_delta","arguments":"{\"city\":"}}""",
        InteractionJson.Default)!;
    Equal("{\"city\":", ((InteractionArgumentsDeltaData)((InteractionStepDeltaEvent)argumentsEvent).Delta!).Arguments);

    var audioEvent = JsonSerializer.Deserialize<InteractionStreamEventPart>(
        """{"event_type":"step.delta","index":4,"delta":{"type":"audio","data":"AQID","mime_type":"audio/wav","sample_rate":24000,"channels":1}}""",
        InteractionJson.Default)!;
    var audioDelta = (InteractionAudioDeltaData)((InteractionStepDeltaEvent)audioEvent).Delta!;
    Equal(24000, audioDelta.SampleRate);

    var unknownEvent = JsonSerializer.Deserialize<InteractionStreamEventPart>(
        """{"event_type":"step.delta","index":3,"delta":{"type":"future_delta","vendor_data":{"x":1}}}""",
        InteractionJson.Default)!;
    var unknown = ((InteractionStepDeltaEvent)unknownEvent).Delta!;
    True(unknown is InteractionUnknownContentDeltaData);
    using var roundTrip = JsonDocument.Parse(JsonSerializer.Serialize(unknownEvent, InteractionJson.Default));
    Equal("future_delta", roundTrip.RootElement.GetProperty("delta").GetProperty("type").GetString());
    True(roundTrip.RootElement.GetProperty("delta").TryGetProperty("vendor_data", out _));

    foreach (var (expectedType, deltaJson) in new (Type, string)[]
    {
        (typeof(InteractionImageDeltaData), """{"type":"image","data":"AQID","mime_type":"image/png","provider":{"x":1}}"""),
        (typeof(InteractionVideoDeltaData), """{"type":"video","uri":"gs://bucket/clip.mp4","mime_type":"video/mp4","provider":{"x":1}}"""),
        (typeof(InteractionDocumentDeltaData), """{"type":"document","uri":"gs://bucket/doc.pdf","mime_type":"application/pdf","provider":{"x":1}}"""),
        (typeof(InteractionTextAnnotationDeltaData), """{"type":"text_annotation_delta","annotations":[{"type":"citation","url":"https://example.invalid","provider":true}],"provider":{"x":1}}"""),
        (typeof(InteractionFunctionResultDeltaData), """{"type":"function_result","name":"lookup","result":[{"type":"text","text":"Found"},{"type":"image","data":"AQID"}],"is_error":false,"provider":{"x":1}}"""),
        (typeof(InteractionUnknownContentDeltaData), """{"type":"google_search_result","result":[{"url":"https://example.invalid","provider":true}],"provider":{"x":1}}""")
    })
    {
        using var originalDelta = JsonDocument.Parse(deltaJson);
        var delta = JsonSerializer.Deserialize<InteractionContentDeltaData>(deltaJson, InteractionJson.Default)!;
        Equal(expectedType, delta.GetType());
        using var deltaRoundTrip = JsonDocument.Parse(JsonSerializer.Serialize(delta, InteractionJson.Default));
        True(JsonElement.DeepEquals(originalDelta.RootElement, deltaRoundTrip.RootElement));
    }
}

static void StreamingSseEvents()
{
    const string body = """
        : keepalive

        event: interaction.created
        data: {"event_type":"interaction.created","interaction":{"id":"int-stream","model":"gemini-flash-latest","status":"in_progress","provider_state":{"x":1}},"event_id":"cursor-1"}

        data: {"event_type":"step.start","index":0,"step":{"type":"thought","signature":"initial-signature"}}

        data: {"event_type":"step.delta","index":0,"delta":{"type":"thought_summary","content":{"type":"text","text":"Checking the weather","provider_data":true}}}

        data: {"event_type":"step.delta","index":0,"delta":{"type":"thought_signature","signature":"signed-thought"}}

        data: {"event_type":"step.stop","index":0,"step_usage":{"total_thought_tokens":2},"usage":{"total_tokens":7},"provider_stop":{"x":1}}

        data: {"event_type":"step.start","index":1,"step":{"type":"function_call","id":"call-1","name":"weather","signature":"signed-call","arguments":{}}}

        data: {"event_type":"step.delta","index":1,"delta":{"type":"arguments_delta","arguments":"{\"city\":\"Paris\"}"}}

        data: {"event_type":"step.stop","index":1}

        data: {"event_type":"interaction.status_update","interaction_id":"int-stream","status":"requires_action","event_id":"cursor-2","usage":{"total_tokens":12}}

        data: {"event_type":"interaction.completed","interaction":{"id":"int-stream","model":"provider/gemini-flash-latest","status":"requires_action","steps":[{"type":"thought","signature":"signed-thought"},{"type":"function_call","id":"call-1","name":"weather","signature":"signed-call","arguments":{"city":"Paris"}}],"usage":{"total_tokens":12,"provider_usage":true}},"event_id":"cursor-3"}

        """;
    var observer = new RecordingObserver();
    using var client = new HttpClient(new StaticResponseHandler(body)) { BaseAddress = new Uri("https://example.invalid/") };
    var events = ReadEvents(client.GetInteractions(
        new InteractionRequest { Input = new InteractionsInput("hello"), Stream = true },
        "provider",
        observer));

    Equal(10, events.Count);
    var created = (InteractionCreatedEvent)events[0];
    Equal("int-stream", created.Interaction!.Id);
    Equal("provider/gemini-flash-latest", created.Interaction.Model);
    Equal("cursor-1", created.EventId);
    Equal(1, created.Interaction.AdditionalProperties!["provider_state"].GetProperty("x").GetInt32());
    True(((InteractionStepStartEvent)events[1]).Step is InteractionThoughtStep);
    var summary = (InteractionThoughtSummaryDeltaData)((InteractionStepDeltaEvent)events[2]).Delta!;
    Equal("Checking the weather", ((InteractionTextContent)summary.Content!).Text);
    True(summary.Content.AdditionalProperties!["provider_data"].GetBoolean());
    Equal("signed-thought", ((InteractionThoughtSignatureDeltaData)((InteractionStepDeltaEvent)events[3]).Delta!).Signature);
    var stop = (InteractionStepStopEvent)events[4];
    Equal(2, stop.StepUsage!.TotalThoughtTokens);
    Equal(7, stop.Usage!.TotalTokens);
    Equal(1, stop.AdditionalProperties!["provider_stop"].GetProperty("x").GetInt32());
    var call = (InteractionFunctionCallStep)((InteractionStepStartEvent)events[5]).Step!;
    Equal("call-1", call.Id);
    Equal("signed-call", call.Signature);
    Equal("{\"city\":\"Paris\"}", ((InteractionArgumentsDeltaData)((InteractionStepDeltaEvent)events[6]).Delta!).Arguments);
    Equal(1, ((InteractionStepStopEvent)events[7]).Index);
    Equal(InteractionStatusKind.RequiresAction, ((InteractionStatusEvent)events[8]).Status!.Value.KnownValue);
    var completed = (InteractionCompletedEvent)events[9];
    Equal("int-stream", completed.Interaction!.Id);
    Equal("provider/gemini-flash-latest", completed.Interaction.Model);
    Equal(12, completed.Interaction.Usage!.TotalTokens);
    True(completed.Interaction.Usage.AdditionalProperties!["provider_usage"].GetBoolean());
    Equal("Paris", ((InteractionFunctionCallStep)completed.Interaction.Steps![1]).Arguments!.Value.GetProperty("city").GetString());
    Equal("cursor-3", completed.EventId);
    Equal(1, observer.Bodies.Count);
    Equal("request-body", observer.Bodies[0].Kind);
    Equal("text/event-stream", observer.StreamMediaType);

    using var serializedStop = JsonDocument.Parse(JsonSerializer.Serialize(stop, InteractionJson.Default));
    Equal(2, serializedStop.RootElement.GetProperty("step_usage").GetProperty("total_thought_tokens").GetInt32());
    Equal(7, serializedStop.RootElement.GetProperty("usage").GetProperty("total_tokens").GetInt32());
}

static void StatelessStepsAndToolResults()
{
    const string json = """
        {
          "id":"int-history",
          "steps":[
            {"type":"user_input","content":[{"type":"text","text":"Describe this"},{"type":"image","uri":"gs://bucket/image.png","mime_type":"image/png","provider_image":{"x":1}}],"provider_step":"user"},
            {"type":"thought","signature":"signed-thought","summary":[{"type":"text","text":"Summary"},{"type":"image","data":"AQID","mime_type":"image/png"}],"provider_thought":true},
            {"type":"function_call","id":"call-1","name":"lookup","arguments":{"query":"x","nested":{"enabled":true}},"signature":"signed-call","provider_call":[1,2]},
            {"type":"function_result","call_id":"call-1","name":"lookup","result":{"answer":42,"nested":{"items":[1,null,true]}},"is_error":false,"signature":"signed-result","provider_result":true},
            {"type":"function_result","call_id":"call-2","result":[{"type":"text","text":"Found"},{"type":"image","data":"AQID","mime_type":"image/png"}]},
            {"type":"function_result","call_id":"call-3","result":"plain tool result"},
            {"type":"model_output","content":[{"type":"text","text":"Answer","annotations":[{"type":"citation","url":"https://example.invalid/source","provider_annotation":true}]},{"type":"audio","data":"AQID","sample_rate":24000},{"type":"video","uri":"gs://bucket/clip.mp4"},{"type":"document","uri":"gs://bucket/doc.pdf","mime_type":"application/pdf"},{"type":"future_media","payload":{"x":1}}],"provider_output":{"x":1}},
            {"type":"future_step","provider_future":{"x":1}}
          ],
          "provider_interaction":true
        }
        """;
    var interaction = JsonSerializer.Deserialize<Interaction>(json, InteractionJson.Default)!;
    Equal("int-history", interaction.Id);
    var call = (InteractionFunctionCallStep)interaction.Steps![2];
    Equal("call-1", call.Id);
    Equal("signed-call", call.Signature);
    Equal("x", call.Arguments!.Value.GetProperty("query").GetString());
    var result = (InteractionFunctionResultStep)interaction.Steps[3];
    Equal(call.Id, result.CallId);
    Equal(42, result.Result!.Value.GetProperty("answer").GetInt32());
    Equal(JsonValueKind.Array, ((InteractionFunctionResultStep)interaction.Steps[4]).Result!.Value.ValueKind);
    Equal("plain tool result", ((InteractionFunctionResultStep)interaction.Steps[5]).Result!.Value.GetString());
    True(interaction.Steps[7] is InteractionUnknownStep);

    var request = new InteractionRequest
    {
        Model = "gemini-flash-latest",
        Store = false,
        Input = new InteractionsInput(interaction.Steps)
    };
    using var serialized = JsonDocument.Parse(JsonSerializer.Serialize(request, InteractionJson.Default));
    using var original = JsonDocument.Parse(json);
    True(JsonElement.DeepEquals(original.RootElement.GetProperty("steps"), serialized.RootElement.GetProperty("input")));
    True(!serialized.RootElement.GetProperty("store").GetBoolean());
    var replay = JsonSerializer.Deserialize<InteractionRequest>(serialized.RootElement, InteractionJson.Default)!;
    True(replay.Input!.IsRaw);
    using var replayJson = JsonDocument.Parse(JsonSerializer.Serialize(replay, InteractionJson.Default));
    True(JsonElement.DeepEquals(serialized.RootElement, replayJson.RootElement));

    request.Input = new InteractionsInput(interaction.Steps.Take(7).ToList());
    var knownReplay = JsonSerializer.Deserialize<InteractionRequest>(
        JsonSerializer.Serialize(request, InteractionJson.Default), InteractionJson.Default)!;
    True(knownReplay.Input!.IsSteps);
    Equal("signed-thought", ((InteractionThoughtStep)knownReplay.Input.Steps![1]).Signature);
    Equal("signed-result", ((InteractionFunctionResultStep)knownReplay.Input.Steps[3]).Signature);

    foreach (var toolResult in new[]
    {
        """{"type":"code_execution_result","call_id":"c","result":"stdout","signature":"s","provider":true}""",
        """{"type":"mcp_server_tool_result","call_id":"c","result":{"value":42},"signature":"s","provider":true}""",
        """{"type":"google_search_result","call_id":"c","result":[{"url":"https://example.invalid","provider":true}],"provider":true}""",
        """{"type":"url_context_result","call_id":"c","result":[{"url":"https://example.invalid","status":"success","provider":true}],"provider":true}"""
    })
    {
        using var originalResult = JsonDocument.Parse(toolResult);
        var step = JsonSerializer.Deserialize<InteractionStep>(toolResult, InteractionJson.Default)!;
        True(step is not InteractionUnknownStep);
        using var roundTrip = JsonDocument.Parse(JsonSerializer.Serialize(step, InteractionJson.Default));
        True(JsonElement.DeepEquals(originalResult.RootElement, roundTrip.RootElement));
    }
}

static void HttpContract()
{
    foreach (var path in new[] { "v1beta/interactions", "v1/interactions" })
    {
        var handler = new StaticResponseHandler(
            """{"id":"int-http","model":"gemini-flash-latest","steps":[{"type":"model_output","content":[{"type":"text","text":"Hello","provider_content":true}],"provider_step":true}],"usage":{"total_tokens":5},"provider_response":{"x":1}}""",
            "application/json");
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.invalid/") };
        var request = new InteractionRequest
        {
            Input = new InteractionsInput(new List<InteractionStep>
            {
                new InteractionFunctionResultStep
                {
                    CallId = "call-1",
                    Result = JsonSerializer.SerializeToElement(new { answer = 42 })
                }
            }),
            PreviousInteractionId = "int-previous",
            ContinuationToken = "checkpoint",
            AdditionalProperties = new() { ["provider_request"] = JsonSerializer.SerializeToElement(new { enabled = true }) }
        };
        var observer = new RecordingObserver();
        var response = path == "v1beta/interactions"
            ? client.GetInteraction(request, "provider", observer).GetAwaiter().GetResult()
            : client.GetInteraction(request, "provider", observer, relativeUrl: path).GetAwaiter().GetResult();
        Equal(HttpMethod.Post, handler.Method);
        Equal($"/{path}", handler.RequestUri!.AbsolutePath);
        True(!handler.HasApiRevision);
        Equal("application/json", handler.Accept);
        Equal("int-http", response.Id);
        Equal("provider/gemini-flash-latest", response.Model);
        Equal(1, response.AdditionalProperties!["provider_response"].GetProperty("x").GetInt32());
        Equal(2, observer.Bodies.Count);
        Equal("response-body", observer.Bodies[1].Kind);
        using var observedResponse = JsonDocument.Parse(observer.Bodies[1].Content);
        Equal("gemini-flash-latest", observedResponse.RootElement.GetProperty("model").GetString());
        using var sent = JsonDocument.Parse(handler.RequestBody!);
        Equal("function_result", sent.RootElement.GetProperty("input")[0].GetProperty("type").GetString());
        Equal(42, sent.RootElement.GetProperty("input")[0].GetProperty("result").GetProperty("answer").GetInt32());
        Equal("int-previous", sent.RootElement.GetProperty("previous_interaction_id").GetString());
        Equal("checkpoint", sent.RootElement.GetProperty("continuation_token").GetString());
        True(sent.RootElement.GetProperty("provider_request").GetProperty("enabled").GetBoolean());
        True(!sent.RootElement.TryGetProperty("outputs", out _));

        var streamingHandler = new StaticResponseHandler(
            """data: {"event_type":"interaction.completed","interaction":{"id":"int-http","model":"gemini-flash-latest","steps":[]}}""" + "\n\n");
        using var streamingClient = new HttpClient(streamingHandler) { BaseAddress = client.BaseAddress };
        request.Stream = true;
        var events = ReadEvents(path == "v1beta/interactions"
            ? streamingClient.GetInteractions(request, "provider")
            : streamingClient.GetInteractions(request, "provider", relativeUrl: path));
        Equal(1, events.Count);
        Equal("provider/gemini-flash-latest", ((InteractionCompletedEvent)events[0]).Interaction!.Model);
        Equal($"/{path}", streamingHandler.RequestUri!.AbsolutePath);
        True(!streamingHandler.HasApiRevision);
        Equal("text/event-stream", streamingHandler.Accept);
        using var streamedRequest = JsonDocument.Parse(streamingHandler.RequestBody!);
        True(streamedRequest.RootElement.GetProperty("stream").GetBoolean());
    }
}

static void StreamingErrorsAndUnknownEvents()
{
    const string unknownJson = """{"event_type":"interaction.future","event_id":"cursor","provider_data":{"x":1}}""";
    var unknown = JsonSerializer.Deserialize<InteractionStreamEventPart>(unknownJson, InteractionJson.Default)!;
    True(unknown is InteractionUnknownStreamEvent);
    using var original = JsonDocument.Parse(unknownJson);
    using var roundTrip = JsonDocument.Parse(JsonSerializer.Serialize(unknown, InteractionJson.Default));
    True(JsonElement.DeepEquals(original.RootElement, roundTrip.RootElement));

    foreach (var malformed in new[] { "{}", """{"event_type":42}""", """{"event_type":""}""" })
        Throws<JsonException>(() => JsonSerializer.Deserialize<InteractionStreamEventPart>(malformed, InteractionJson.Default));

    using var errorClient = new HttpClient(new StaticResponseHandler(
        """data: {"event_type":"error","error":{"code":"unavailable","message":"Try again","provider_error":true},"event_id":"error-cursor"}""" + "\n\n"))
    { BaseAddress = new Uri("https://example.invalid/") };
    var error = (InteractionErrorEvent)ReadEvents(errorClient.GetInteractions(
        new InteractionRequest { Input = new InteractionsInput("hello"), Stream = true }, "provider"))[0];
    Equal("unavailable", error.Error!.Code);
    True(error.Error.AdditionalProperties!["provider_error"].GetBoolean());
    Equal("error-cursor", error.EventId);

    foreach (var invalid in new[] { "{invalid}", "null", "{}" })
    {
        using var malformedClient = new HttpClient(new StaticResponseHandler($"data: {invalid}\n\n"))
        { BaseAddress = new Uri("https://example.invalid/") };
        var failure = Throws<InvalidOperationException>(() => ReadEvents(malformedClient.GetInteractions(
            new InteractionRequest { Input = new InteractionsInput("hello"), Stream = true }, "provider")));
        True(failure.InnerException is JsonException);
        True(failure.Message.Contains(invalid, StringComparison.Ordinal));
    }
}

static List<InteractionStreamEventPart> ReadEvents(IAsyncEnumerable<InteractionStreamEventPart> source)
{
    var result = new List<InteractionStreamEventPart>();
    var enumerator = source.GetAsyncEnumerator();
    try
    {
        while (enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult())
            result.Add(enumerator.Current);
    }
    finally
    {
        enumerator.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
    return result;
}

static TException Throws<TException>(Action action) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException exception)
    {
        return exception;
    }
    throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
}

static void UsageFields()
{
    var usage = JsonSerializer.Deserialize<InteractionUsage>(
        """{"initial_input_tokens":3,"grounding_tool_count":[{"type":"google_search","count":2}]}""",
        InteractionJson.Default)!;
    Equal(3, usage.InitialInputTokens);
    Equal("google_search", usage.GroundingToolCount![0].Type);
    Equal(2, usage.GroundingToolCount[0].Count);
}

static void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new InvalidOperationException($"Expected '{expected}', got '{actual}'.");
}

static void True(bool condition)
{
    if (!condition)
        throw new InvalidOperationException("Expected condition to be true.");
}

sealed class StaticResponseHandler(string body, string mediaType = "text/event-stream") : HttpMessageHandler
{
    public HttpMethod? Method { get; private set; }
    public Uri? RequestUri { get; private set; }
    public string? RequestBody { get; private set; }
    public string? Accept { get; private set; }
    public bool HasApiRevision { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Method = request.Method;
        RequestUri = request.RequestUri;
        RequestBody = await request.Content!.ReadAsStringAsync(cancellationToken);
        Accept = request.Headers.Accept.Single().MediaType;
        HasApiRevision = request.Headers.Contains("Api-Revision");
        return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(body, System.Text.Encoding.UTF8, mediaType)
        };
    }
}

sealed class RecordingObserver : IInteractionTransportObserver
{
    public List<(string Kind, string Content, string MediaType)> Bodies { get; } = [];
    public string? StreamMediaType { get; private set; }

    public ValueTask OnBodyAsync(string kind, string content, string mediaType, CancellationToken cancellationToken)
    {
        Bodies.Add((kind, content, mediaType));
        return ValueTask.CompletedTask;
    }

    public Stream ObserveStream(Stream stream, string mediaType)
    {
        StreamMediaType = mediaType;
        return stream;
    }
}
