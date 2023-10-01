using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.Lib.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskStatus
{
    Reserved,
    Ongoing,
    Done,
    Pending
}