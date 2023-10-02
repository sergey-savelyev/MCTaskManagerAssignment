using MCGAssignment.TodoList.Core.Entities;

namespace MCGAssignment.TodoList.Application.Repositories;

public interface ITasksRepository : IRepository<TaskEntity>
{
    Task<IEnumerable<TaskEntity>> GetSubTasksAsync(Guid taskId, CancellationToken cancellationToken);

    Task<IEnumerable<TaskEntity>> GetRootTaskBatchAsync(int take, object continuationToken, string sortBy, bool descending, CancellationToken cancellationToken);

    Task<IEnumerable<TaskSearchEntity>> SearchTasksAsync(string keyPhrase, int take, object continuationToken, CancellationToken cancellationToken);

    Task<IEnumerable<Guid>> GetAllSubtaskIdsRecursivelyAsync(Guid taskId, CancellationToken cancellationToken);

    Task UpdateTaskRootAsync(Guid taskId, Guid? newRootId, CancellationToken cancellationToken);
}