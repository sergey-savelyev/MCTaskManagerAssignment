using MCGAssignment.TodoList.Application.DataTransferObjects;

namespace MCGAssignment.TodoList.Application.Services;

public interface ITaskService
{
    Task<(IEnumerable<TaskViewDetailed> Entities, object ContinuationToken)> GetRootTaskBatchAsync(int take, object continuationToken, string sortBy, bool descending, CancellationToken cancellationToken);

    Task<TaskViewFull> GetTaskAsync(Guid taskId, CancellationToken cancellationToken);

    Task<Guid> CreateTaskAsync(UpsertTaskDto taskDetails, CancellationToken cancellationToken);

    Task UpdateTaskAsync(Guid taskId, UpsertTaskDto taskDetails, CancellationToken cancellationToken);

    Task UpdateTaskRootAsync(Guid taskId, Guid? newRootId, CancellationToken cancellationToken);

    Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken);

    Task<(IEnumerable<TaskSearchView> Entities, object ContinuationToken)> SearchTasksAsync(string keyPhrase, int take, object continuationToken, CancellationToken cancellationToken);
}