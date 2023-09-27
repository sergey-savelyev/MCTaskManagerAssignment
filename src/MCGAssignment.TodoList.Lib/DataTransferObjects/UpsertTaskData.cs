using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.DataTransferObjects;

public record UpsertTaskData
{
    public string? Id { get; init; }

    public string? RootId { get; init; }

    public required string Summary { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskPriority Priority { get; init;}

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskStatus Status { get; init; }

    public string? Description { get; init; }
    
    public DateTime DueDate { get; init; }
};