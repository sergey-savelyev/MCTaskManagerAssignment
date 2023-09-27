using System.Text.Json;

namespace MCGAssignment.TodoList.DataTransferObjects;

public record LogEntryView(string Id, string Action, long TimestampMsec, string EntityId, string EntityType, string? Payload);