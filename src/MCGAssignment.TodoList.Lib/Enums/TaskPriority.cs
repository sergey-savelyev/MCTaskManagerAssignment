using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.Lib.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskPriority : int
{
    Low,
    Normal,
    High,
    Urgent
}