using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

[JsonConverter(typeof(InteractionsInputJsonConverter))]
public sealed class InteractionsInput
{
    public InteractionsInput()
    {
    }

    public InteractionsInput(string text)
    {
        Text = text;
    }

    public InteractionsInput(List<InteractionContent> content)
    {
        Content = content;
    }
 
    public InteractionsInput(List<InteractionStep> steps, bool useSteps, string? _ = null)
    {
        Steps = steps;
    }

    [Obsolete("Prefer the current Interactions API input forms: text, content blocks, or steps.")]
    public InteractionsInput(List<InteractionTurn> turns, bool useTurns)
    {
#pragma warning disable CS0618
        Turns = turns;
#pragma warning restore CS0618
    }

    public InteractionsInput(InteractionContent singleContent)
    {
        SingleContent = singleContent;
    }

    public InteractionsInput(JsonElement raw)
    {
        Raw = raw;
    }

    public string? Text { get; init; }

    public InteractionContent? SingleContent { get; init; }

    public List<InteractionContent>? Content { get; init; }
 
    public List<InteractionStep>? Steps { get; init; }

    [Obsolete("Prefer the current Interactions API input forms: text, content blocks, or steps.")]
    public List<InteractionTurn>? Turns { get; init; }

    public JsonElement? Raw { get; init; }

    public bool IsText => Text is not null;

    public bool IsSingleContent => SingleContent is not null;

    public bool IsContent => Content is not null;
 
    public bool IsSteps => Steps is not null;

#pragma warning disable CS0618
    public bool IsTurns => Turns is not null;
#pragma warning restore CS0618

    public bool IsRaw => Raw is not null && Raw.Value.ValueKind is not JsonValueKind.Null and not JsonValueKind.Undefined;
}

[Obsolete("Prefer the current Interactions API input forms: text, content blocks, or steps.")]
public sealed class InteractionTurn
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public InteractionTurnContent? Content { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

[JsonConverter(typeof(InteractionTurnContentJsonConverter))]
public sealed class InteractionTurnContent
{
    public InteractionTurnContent()
    {
    }

    public InteractionTurnContent(string text)
    {
        Text = text;
    }

    public InteractionTurnContent(List<InteractionContent> parts)
    {
        Parts = parts;
    }

    public InteractionTurnContent(JsonElement raw)
    {
        Raw = raw;
    }

    public string? Text { get; init; }

    public List<InteractionContent>? Parts { get; init; }

    public JsonElement? Raw { get; init; }

    public bool IsText => Text is not null;

    public bool IsParts => Parts is not null;

    public bool IsRaw => Raw is not null && Raw.Value.ValueKind is not JsonValueKind.Null and not JsonValueKind.Undefined;
}
