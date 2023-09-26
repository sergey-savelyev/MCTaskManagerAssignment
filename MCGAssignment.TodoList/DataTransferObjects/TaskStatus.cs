using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.DataTransferObjects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskStatus
{
    Reserved,
    Ongoing,
    Done,
    Pending
}