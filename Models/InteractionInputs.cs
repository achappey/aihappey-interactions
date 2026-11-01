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
 
    public InteractionsInput(List<InteractionStep> steps)
    {
        Steps = steps;
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

    public JsonElement? Raw { get; init; }

    public bool IsText => Text is not null;

    public bool IsSingleContent => SingleContent is not null;

    public bool IsContent => Content is not null;
 
    public bool IsSteps => Steps is not null;

    public bool IsRaw => Raw is not null && Raw.Value.ValueKind is not JsonValueKind.Null and not JsonValueKind.Undefined;
}
