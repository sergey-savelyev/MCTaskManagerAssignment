namespace MCGAssignment.TodoList.DataTransferObjects;

public record LogEntryView(Guid Id, string Action, long TimestampMsec, Guid? EntityId, string EntityType, string? Payload);