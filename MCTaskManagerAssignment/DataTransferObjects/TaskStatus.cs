using System.Text.Json.Serialization;

namespace MCTaskManagerAssignment.DataTransferObjects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskStatus
{
    Reserved,
    Ongoing,
    Done,
    Pending
}