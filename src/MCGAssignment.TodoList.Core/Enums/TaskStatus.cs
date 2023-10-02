using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskStatus : byte
{
    Reserved,
    Ongoing,
    Done,
    Pending
}