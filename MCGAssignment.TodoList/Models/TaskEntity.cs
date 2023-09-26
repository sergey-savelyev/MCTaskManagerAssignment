using MCGAssignment.TodoList.DataTransferObjects;

namespace MCGAssignment.TodoList.Models;

public record TaskEntity
{
    public required string Id { get; init; }

    public string? RootTaskId { get; init; }

    public virtual TaskEntity? RootTask { get; init; }

    public required string Summary { get; init; }

    public string? Description { get; init; }

    public DateTime CreateDate { get; init; }

    public DateTime DueDate { get; init; }

    public TaskPriority Priority { get; init; }

    public DataTransferObjects.TaskStatus Status { get; init; }
}