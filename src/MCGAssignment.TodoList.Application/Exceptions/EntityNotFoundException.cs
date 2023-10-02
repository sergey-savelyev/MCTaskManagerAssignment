namespace MCGAssignment.TodoList.Application.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(Guid id) : base($"Entity with id {id} cannot be found") { }
}