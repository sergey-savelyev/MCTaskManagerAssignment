namespace MCGAssignment.TodoList.Services;

public interface ITaskActionLogger
{
    Task LogCreateAsync(string taskId, CancellationToken cancellationToken);

    Task LogUpdateAsync(string taskId, CancellationToken cancellationToken);

    Task LogDeleteAsync(string taskId, CancellationToken cancellationToken);

    Task LogRootChangedAsync(string taskId, string? rootId, CancellationToken cancellationToken);
}