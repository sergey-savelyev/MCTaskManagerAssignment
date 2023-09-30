namespace MCGAssignment.TodoList.Services;

public interface ITaskActionLogger
{
    Task LogCreateAsync(Guid taskId, CancellationToken cancellationToken);

    Task LogUpdateAsync(Guid taskId, CancellationToken cancellationToken);

    Task LogDeleteAsync(Guid taskId, CancellationToken cancellationToken);

    Task LogRootChangedAsync(Guid taskId, Guid? rootId, CancellationToken cancellationToken);
}