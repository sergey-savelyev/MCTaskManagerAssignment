using System.Text.Json.Serialization;

namespace MCTaskManagerAssignment.DataTransferObjects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskPriority : int
{
    Low,
    Normal,
    High,
    Urgent
}