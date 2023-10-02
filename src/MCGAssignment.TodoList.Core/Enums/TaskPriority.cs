using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskPriority : int
{
    Low,
    Normal,
    High,
    Urgent
}