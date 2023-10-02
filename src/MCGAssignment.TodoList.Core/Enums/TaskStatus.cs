using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskStatus
{
    Reserved,
    Ongoing,
    Done,
    Pending
}