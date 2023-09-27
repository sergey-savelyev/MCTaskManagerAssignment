using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.DataTransferObjects;

public record TaskViewBase
{
    public required string Id { get; init; }

    public required string Summary { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskPriority Priority { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskStatus Status { get; init; }
}

public record TaskViewDetailed : TaskViewBase
{
    public string? RootId { get; init; }

    public DateTime CreateDate { get; init; }

    public DateTime DueDate { get; init; }
}

public record TaskViewFull : TaskViewDetailed
{
    public TaskViewBase? RootTask { get; init; }

    public List<TaskViewBase> Subtasks { get; init; } = new();

    public string? Description { get; init; }
}

public record TaskSearchView(string Id, string? Summary, string? Description);