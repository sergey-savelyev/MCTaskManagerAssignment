using MCTaskManagerAssignment.DataTransferObjects;

namespace MCTaskManagerAssignment.Models;

public record TaskDocument
{
    public string Id { get; set; } = default!;

    public string? ParentId { get; set; }

    public string? Summary { get; set; }

    public string? Description { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime DueDate { get; set; }

    public TaskPriority Priority { get; set; }

    public DataTransferObjects.TaskStatus Status { get; set; }

    public TaskViewDetailed ToDetailedView() => new TaskViewDetailed
    {
        Id = Id,
        ParentId = ParentId,
        Summary = Summary,
        Priority = Priority,
        Status = Status,
        CreateDate = CreateDate,
        DueDate = DueDate
    };

    public TaskViewFull ToFullView(IEnumerable<TaskViewBase> childTasks) => new TaskViewFull
    {
        Id = Id,
        ParentId = ParentId,
        Summary = Summary,
        Priority = Priority,
        Status = Status,
        Description = Description,
        CreateDate = CreateDate,
        DueDate = DueDate,
        ChildTasks = childTasks.ToList()
    };
}