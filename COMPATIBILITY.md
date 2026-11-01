# Interactions API contract

This library exclusively models Google's modern steps-based Gemini Interactions API. Requests default to `v1beta/interactions` without an `Api-Revision` header. Callers may provide `v1/interactions` through the same HTTP methods; there is no separate version-specific implementation, schema translation, or compatibility parser.

Responses contain `steps`: user inputs, model outputs, thoughts, function calls, and tool results are top-level steps. Multimodal content belongs inside those steps. Stateless history uses `InteractionsInput(List<InteractionStep>)`; replay the steps unchanged to preserve call IDs, signatures, and provider metadata. Stateful continuation uses `previous_interaction_id` or `continuation_token`.

References:

- [Gemini Interactions API reference](https://ai.google.dev/api/interactions-api)
- [Streaming interactions](https://ai.google.dev/gemini-api/docs/streaming)
- [Google Gen AI Python SDK generated Interactions models](https://github.com/googleapis/python-genai/tree/main/google/genai/_gaos/types/interactions)

## Typed contract

| Member | Type | Usage |
|---|---|---|
| `InteractionRequest.ResponseFormat`, `Interaction.ResponseFormat` | `InteractionResponseFormats?` | Use typed text, image, audio, or video response formats. Construct one variant for an object, or a list for the array form. JSON schemas use `JsonElement`. |
| `Interaction.Status`, `InteractionStatusEvent.Status` | `InteractionStatus?` | Read `.Value` for the wire value or `.KnownValue` for `InteractionStatusKind?`. Unknown status strings round-trip unchanged. |
| `InteractionRequest.Environment`, `Interaction.Environment` | `InteractionEnvironmentValue?` | Use a string reference or `InteractionRemoteEnvironment` configuration. |
| `InteractionRequest.WebhookConfig`, `Interaction.WebhookConfig` | `InteractionWebhookConfig?` | Use `Uris` and `UserMetadata`. |
| `InteractionGenerationConfig.ToolChoice` | `InteractionToolChoiceValue?` | Use a mode string or `InteractionToolChoiceConfiguration` with `AllowedTools`. |
| `InteractionRequest.AgentConfig`, `Interaction.AgentConfig` | `InteractionAgentConfig?` | Current typed variants include dynamic, deep research, antigravity, and CodeMender. CodeMender's find/fix request payloads remain `JsonElement` to preserve their evolving shapes. |
| `InteractionVideoContent.Processing`, `InteractionTranscriptionConfig.Mode` | `InteractionVideoProcessing?`, `InteractionTranscriptionMode?` | Use the documented string mode or typed configuration variants. |
| Tool parameter, call-argument, and result payloads | `JsonElement?` | Assign arbitrary JSON with `JsonSerializer.SerializeToElement`. MCP headers use `Dictionary<string, string>?`. |
| `InteractionModelOutputStep.Error` | `InteractionModelOutputError?` | Read numeric RPC `Code`, `Message`, and structured `Details`, distinct from interaction-level `InteractionErrorInfo`. |
| `InteractionStepDeltaEvent.Delta` | `InteractionContentDeltaData?` | Typed variants cover text, arguments, thought, multimodal, annotations, function and built-in tool calls/results, retrieval, and processing. Future variants retain their JSON in `InteractionUnknownContentDeltaData`. |
| `Interaction.Steps` | `List<InteractionStep>?` | Tool calls, results, and thoughts are top-level steps. Content blocks are text, image, audio, document, or video; thought steps contain summary content and signatures. |

Input accepts a string, a content block, content blocks, or steps. The role/content turn models, turn converters, thought content model, and unused step-selection constructor flags have been removed. Thoughts use `InteractionThoughtStep`. Unknown input arrays remain raw `JsonElement` values rather than being guessed into a known variant.

## Streaming

Supported typed events are `interaction.created`, `interaction.status_update`, `step.start`, `step.delta`, `step.stop`, `interaction.completed`, and `error`, discriminated by `event_type`. Status changes, including `in_progress` and `requires_action`, are values of `interaction.status_update`, not separate event aliases. Step-stop events expose both `step_usage` and cumulative `usage`. Reasoning summaries, signatures, arguments, multimodal data, and usage metadata are preserved in step deltas. The SSE reader joins multiple `data:` lines in a frame and dispatches a final frame at EOF.

## Provider passthrough

Unrecognized members, discriminator values, tool payloads, and streaming delta variants round-trip without schema translation. `AdditionalProperties` and raw input retain provider-specific JSON; this does not imply support for legacy schemas. There is no typed `outputs` model or conversion from outputs to steps.

Provider tool metadata uses the same modern tool converter as requests. Malformed or null tool definitions raise `JsonException` instead of being silently omitted.

Deprecated configuration facades have been removed: `cached_content`, `response_mime_type`, `response_modalities`, `temperature`, `top_p`, `image_config`, `audio.rate`, and deprecated top-level transcription settings. Use `response_format`, `sample_rate`, `language_codes`, `custom_vocabulary`, and structured transcription `mode` settings where applicable. Explicit provider JSON can still pass through extension data; the library neither synthesizes nor translates deprecated fields. The documented object/array response-format and speech-configuration forms remain supported.
