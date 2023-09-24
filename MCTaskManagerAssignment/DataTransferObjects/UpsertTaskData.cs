namespace MCTaskManagerAssignment.DataTransferObjects;

public record UpsertTaskData(string? Id, string? Summary, TaskPriority Priority, TaskStatus Status, string? Description, DateTime DueDate, string? ParentId);