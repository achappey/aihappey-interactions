using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIHappey.Interactions;

public enum InteractionStatusKind
{
    InProgress,
    RequiresAction,
    Completed,
    Failed,
    Cancelled,
    Incomplete,
    BudgetExceeded,
    Queued,
    RequiresContinuation
}

[JsonConverter(typeof(InteractionStatusJsonConverter))]
public readonly record struct InteractionStatus
{
    public InteractionStatus(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public InteractionStatusKind? KnownValue => Value switch
    {
        "in_progress" => InteractionStatusKind.InProgress,
        "requires_action" => InteractionStatusKind.RequiresAction,
        "completed" => InteractionStatusKind.Completed,
        "failed" => InteractionStatusKind.Failed,
        "cancelled" => InteractionStatusKind.Cancelled,
        "incomplete" => InteractionStatusKind.Incomplete,
        "budget_exceeded" => InteractionStatusKind.BudgetExceeded,
        "queued" => InteractionStatusKind.Queued,
        "requires_continuation" => InteractionStatusKind.RequiresContinuation,
        _ => null
    };

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator InteractionStatus(string value) => new(value);

    public static implicit operator InteractionStatus(InteractionStatusKind value) => value switch
    {
        InteractionStatusKind.InProgress => new("in_progress"),
        InteractionStatusKind.RequiresAction => new("requires_action"),
        InteractionStatusKind.Completed => new("completed"),
        InteractionStatusKind.Failed => new("failed"),
        InteractionStatusKind.Cancelled => new("cancelled"),
        InteractionStatusKind.Incomplete => new("incomplete"),
        InteractionStatusKind.BudgetExceeded => new("budget_exceeded"),
        InteractionStatusKind.Queued => new("queued"),
        InteractionStatusKind.RequiresContinuation => new("requires_continuation"),
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };
}

public sealed class InteractionStatusJsonConverter : JsonConverter<InteractionStatus>
{
    public override InteractionStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Interaction status must be a string.");

        return new InteractionStatus(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, InteractionStatus value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
