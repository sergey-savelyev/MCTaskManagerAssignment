using MCGAssignment.TodoList.Core.Enums;

namespace MCGAssignment.TodoList.Core.Entities;

public class TaskEntity
{
    public required Guid Id { get; init; }

    public Guid? RootTaskId { get; set; }

    public virtual TaskEntity? RootTask { get; init; }

    public required string Summary { get; set; }

    public string? Description { get; set; }

    public DateTime CreateDate { get; init; }

    public DateTime DueDate { get; set; }

    public TaskPriority Priority { get; set; }

    public Core.Enums.TaskStatus Status { get; set; }
}