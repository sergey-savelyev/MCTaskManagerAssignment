using System.Text.Json.Serialization;
using MCGAssignment.TodoList.Lib.Enums;

namespace MCGAssignment.TodoList.Lib.DataTransferObjects;

public record UpsertTaskData
{
    public required string Summary { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskPriority Priority { get; init;}

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Enums.TaskStatus Status { get; init; }

    public string? Description { get; init; }
    
    public DateTime DueDate { get; init; }
}