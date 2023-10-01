using MCGAssignment.TodoList.Lib.Enums;

namespace MCGAssignment.TodoList.Models;

public class LogEntity
{
    public required Guid Id { get; init; }

    public required TaskAction Action { get; init; }

    public long TimestampMsec { get; init; }

    public Guid? EntityId { get; init; }

    public required string EntityType { get; init; }

    public string? Payload { get; init; }
}