using System.Text.Json;

namespace MCGAssignment.TodoList.Models;

public class LogEntity
{
    public required string Id { get; init; }

    public required string Action { get; init; }

    public long TimestampMsec { get; init; }

    public required string EntityId { get; init; }

    public required string EntityType { get; init; }

    public required JsonElement Payload { get; init; }
}