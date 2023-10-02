using System.Text.Json.Serialization;
using MCGAssignment.TodoList.Core.Enums;

namespace MCGAssignment.TodoList.Application.DataTransferObjects;

public record LogEntryView
{
    public Guid Id { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskAction Action { get; init; }

    public long TimestampMsec { get; init; }

    public Guid? EntityId { get; init; }

    public string? EntityType { get; init; }

    public string? Payload { get; init; }

    public LogEntryView(Guid id, TaskAction action, long timestampMsec, Guid? entityId, string? entityType, string? payload)
    {
        Id = id;
        Action = action;
        TimestampMsec = timestampMsec;
        EntityId = entityId;
        EntityType = entityType;
        Payload = payload;
    }
}
