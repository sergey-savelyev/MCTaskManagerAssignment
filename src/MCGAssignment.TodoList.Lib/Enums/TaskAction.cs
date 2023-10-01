using System.Text.Json.Serialization;

namespace MCGAssignment.TodoList.Lib.Enums
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