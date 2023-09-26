namespace MCGAssignment.TodoList.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string id) : base($"Entity with id {id} cannot be found in the repository") { }
}