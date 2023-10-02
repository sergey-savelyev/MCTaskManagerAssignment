using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskPriority : byte
{
    Low,
    Normal,
    High,
    Urgent
}