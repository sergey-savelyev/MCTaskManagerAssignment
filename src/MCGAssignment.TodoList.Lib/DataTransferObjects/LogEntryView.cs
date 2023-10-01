using MCGAssignment.TodoList.Lib.Enums;

namespace MCGAssignment.TodoList.Lib.DataTransferObjects;

public record LogEntryView(Guid Id, TaskAction Action, long TimestampMsec, Guid? EntityId, string EntityType, string? Payload);