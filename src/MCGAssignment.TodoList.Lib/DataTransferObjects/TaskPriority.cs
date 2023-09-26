using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.DataTransferObjects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskPriority : int
{
    Low,
    Normal,
    High,
    Urgent
}