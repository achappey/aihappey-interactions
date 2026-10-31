using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public static class InteractionJson
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new InteractionsInputJsonConverter(),
            new InteractionTurnContentJsonConverter(),
            new InteractionStepJsonConverter(),
            new InteractionContentJsonConverter(),
            new InteractionToolJsonConverter(),
            new InteractionAgentConfigJsonConverter(),
            new InteractionStreamEventJsonConverter()
        }
    };
}
