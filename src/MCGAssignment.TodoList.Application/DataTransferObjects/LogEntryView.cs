using MCGAssignment.TodoList.Core.Enums;

namespace MCGAssignment.TodoList.Application.DataTransferObjects;

public record LogEntryView(Guid Id, TaskAction Action, long TimestampMsec, Guid? EntityId, string EntityType, string? Payload);