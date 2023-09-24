namespace MCTaskManagerAssignment.DataTransferObjects;

public record TaskViewBase
{
    public string Id { get; set; } = default!;

    public string? ParentId { get; set; }

    public string? Summary { get; set; }

    public TaskPriority Priority { get; set; }

    public TaskStatus Status { get; set; }
}

public record TaskViewDetailed : TaskViewBase
{
    public DateTime CreateDate { get; set; }

    public DateTime DueDate { get; set; }
}

public record TaskViewFull : TaskViewDetailed
{
    public List<TaskViewBase> ChildTasks { get; set; } = new();

    public string? Description { get; set; }
}
