# Interactions API compatibility

This library models Google's Gemini Interactions API and leaves endpoint-version selection to callers. The API reference and versioning documentation were checked for both `v1` and `v1beta`; `v1` is the stable contract, while `v1beta` may expose preview capabilities. The DTOs model shared documented interaction shapes, preserve unrecognized fields and discriminator values, and do not claim that every optional feature is enabled on every API version or model.

References:

- [Gemini Interactions API reference](https://ai.google.dev/api/interactions-api)
- [Gemini API versions](https://ai.google.dev/gemini-api/docs/api-versions)
- [Interactions API breaking-change migration guide](https://ai.google.dev/gemini-api/docs/interactions-breaking-changes-may-2026)
- [Google Gen AI Python SDK generated Interactions models](https://github.com/googleapis/python-genai/tree/main/google/genai/_gaos/types/interactions)

## Breaking changes and consumer updates

The following public property types have changed to represent the published JSON contracts rather than arbitrary CLR objects:

| Member | Previous type | Current type | Consumer update |
|---|---|---|---|
| `InteractionRequest.ResponseFormat`, `Interaction.ResponseFormat` | `object?` | `InteractionResponseFormats?` | Use a typed `InteractionTextResponseFormat`, `InteractionImageResponseFormat`, `InteractionAudioResponseFormat`, or `InteractionVideoResponseFormat`. Construct `InteractionResponseFormats` with one variant for an object, or a list for the array form. JSON schemas are represented by `JsonElement`. |
| `Interaction.Status`, `InteractionStatusEvent.Status` | `string?` | `InteractionStatus?` | Read `.Value` for the wire value or `.KnownValue` for `InteractionStatusKind?`. Unknown future status strings remain available and round-trip unchanged. |
| `InteractionRequest.Environment`, `Interaction.Environment` | `object?` | `InteractionEnvironmentValue?` | Use a string reference or `InteractionRemoteEnvironment` configuration. |
| `InteractionRequest.WebhookConfig`, `Interaction.WebhookConfig` | `object?` | `InteractionWebhookConfig?` | Use the typed `Uris` and `UserMetadata` properties. |
| `InteractionGenerationConfig.ToolChoice` | `object?` | `InteractionToolChoiceValue?` | Use a mode string or `InteractionToolChoiceConfiguration` with `AllowedTools`. |
| `InteractionVideoContent.Processing`, `InteractionTranscriptionConfig.Mode` | `object?` | `InteractionVideoProcessing?`, `InteractionTranscriptionMode?` | Use the string mode or the typed discriminated configuration variants. |
| Tool parameter, call-argument, and result payloads | `object?` | `JsonElement?` | Use `JsonSerializer.SerializeToElement` when assigning arbitrary JSON, and inspect the JSON value explicitly when reading it. MCP headers are now `Dictionary<string, string>?`. |
| `InteractionModelOutputStep.Error` | `object?` | `InteractionModelOutputError?` | Read the numeric RPC `Code`, `Message`, and structured `Details`. This remains distinct from `InteractionErrorInfo` used by interaction-level diagnostics. |
| `InteractionStepDeltaEvent.Delta` | concrete `InteractionContentDeltaData` | abstract discriminated union | Handle the typed text, arguments, thought, audio, image, video, document, and annotation delta variants; use `InteractionUnknownContentDeltaData` for unrecognized variants. Unknown members remain in `AdditionalProperties`. |
| Tool-call/result and thought variants in `Interaction.Steps` | `Interaction*Content` | `Interaction*Step` | Handle these as top-level interaction steps. `InteractionContent` now represents only text, image, audio, document, and video blocks. Thought summaries are steps, not content. |

`InteractionTurn` and the role/content input constructor remain temporarily available but are marked obsolete. Prefer a string, content block(s), or typed step(s), which are the current documented Interactions input forms. `InteractionThoughtContent` is also obsolete; deserialize thoughts as `InteractionThoughtStep`. Unsupported future input arrays are retained as `JsonElement` rather than being incorrectly deserialized as content blocks.

## Deprecated request fields

The current Google-generated Interactions model marks `cached_content`, `response_mime_type`, `response_modalities`, `temperature`, `top_p`, `image_config`, `audio.rate`, and several transcription settings as deprecated. Their DTO properties remain available and continue to serialize for compatibility, but now produce compiler warnings; prefer `response_format` for output modality/format configuration and `language_codes` in place of `language_hints`. Deprecation does not itself imply that a field is rejected by every server version.

## Version scope and remaining limits

The v1 and v1beta references do not provide a complete field-by-field feature-availability matrix for every model. Callers should select the version and model that support the desired capability; this library intentionally does not gate fields or silently remove version-specific payloads. Streaming deltas beyond the explicitly typed variants use the open unknown-delta representation and preserve their JSON for forward-compatible handling.
