using MCTaskManagerAssignment.DataTransferObjects;

namespace MCTaskManagerAssignment.Models;

public record TaskDocument
{
    public string Id { get; set; } = default!;

    public string? RootId { get; set; }

    public string? Summary { get; set; }

    public string? Description { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime DueDate { get; set; }

    public TaskPriority Priority { get; set; }

    public DataTransferObjects.TaskStatus Status { get; set; }

    public TaskViewDetailed ToDetailedView() => new TaskViewDetailed
    {
        Id = Id,
        RootId = RootId,
        Summary = Summary,
        Priority = Priority,
        Status = Status,
        CreateDate = CreateDate,
        DueDate = DueDate
    };

    public TaskViewFull ToFullView(IEnumerable<TaskViewBase> subtasks) => new TaskViewFull
    {
        Id = Id,
        RootId = RootId,
        Summary = Summary,
        Priority = Priority,
        Status = Status,
        Description = Description,
        CreateDate = CreateDate,
        DueDate = DueDate,
        Subtasks = subtasks.ToList()
    };
}

public record TaskSearchDocument(string Id, string? RootId, string? Summary, string? Description);