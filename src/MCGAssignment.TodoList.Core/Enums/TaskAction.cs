using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskAction
    {
        Create,
        Delete,
        Update,
        RootChanged
    }
}