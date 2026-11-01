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
    ("unknown agent config discriminator", UnknownAgentConfig),
    ("interaction status update event", StatusUpdateEvent),
    ("open interaction statuses", OpenInteractionStatuses),
    ("SSE status update parsing", StreamingSseEvents),
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
          "response_mime_type":"application/json",
          "safety_settings":[{"category":"HARM_CATEGORY_HARASSMENT","threshold":"BLOCK_ONLY_HIGH"}],
          "webhook_config":{"url":"https://example.invalid/hook"},
          "generation_config":{"temperature":0.2,"top_p":0.8,"unknown_generation_field":true},
          "future_request_field":{"enabled":true}
        }
        """,
        InteractionJson.Default)!;

    Equal("resume-me", request.ContinuationToken);
    Equal("test", request.Labels!["trace"]);
    Equal("env-1", request.Environment!.Reference);
#pragma warning disable CS0618
    Equal(0.2, request.GenerationConfig!.Temperature);
    Equal(0.8, request.GenerationConfig.TopP);
#pragma warning restore CS0618
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

    var legacyEvent = JsonSerializer.Deserialize<InteractionStreamEventPart>(
        """{"event_type":"interaction.in_progress","interaction_id":"int-1","status":"in_progress"}""",
        InteractionJson.Default);
    True(legacyEvent is InteractionStatusEvent);
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
}

static void StreamingSseEvents()
{
    const string body = """
        data: {"event_type":"interaction.status_update","interaction_id":"int-stream","status":"in_progress","event_id":"cursor-1"}

        data: [DONE]

        """;
    using var client = new HttpClient(new StaticResponseHandler(body));
    var events = client.GetInteractions(
        new InteractionRequest { Input = new InteractionsInput("hello"), Stream = true },
        "provider",
        relativeUrl: "https://example.invalid/v1beta/interactions");
    var enumerator = events.GetAsyncEnumerator();
    try
    {
        True(enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult());
        True(enumerator.Current is InteractionStatusEvent);
        Equal("cursor-1", ((InteractionStatusEvent)enumerator.Current).EventId);
        True(!enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult());
    }
    finally
    {
        enumerator.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
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

sealed class StaticResponseHandler(string body) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(body, System.Text.Encoding.UTF8, "text/event-stream")
        });
}
