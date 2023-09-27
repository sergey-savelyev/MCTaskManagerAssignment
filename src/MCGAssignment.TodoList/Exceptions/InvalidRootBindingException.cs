namespace MCGAssignment.TodoList.Exceptions;

public class InvalidRootBindingException : Exception
{
    public InvalidRootBindingException()
        : base($"Tasks cannot be bound because the suggested root task is already a subtask of the entity")
    { }
}