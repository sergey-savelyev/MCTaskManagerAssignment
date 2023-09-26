using MCGAssignment.TodoList.Models;

namespace MCGAssignment.TodoList.Repositories;

public interface ITaskRepository
{
    Task<TaskEntity> GetTaskAsync(string id, CancellationToken cancellationToken);

    Task<IEnumerable<TaskEntity>> GetRootTaskBatchAsync(int take, int skip, string sortBy, bool descending, CancellationToken cancellationToken);

    Task<IEnumerable<TaskEntity>> GetSubtasksAsync(string parentId, CancellationToken cancellationToken);

    Task<TaskEntity> UpdateTaskAsync(TaskEntity task, CancellationToken cancellationToken);

    Task<TaskEntity> CreateTaskAsync(TaskEntity task, CancellationToken cancellationToken);

    Task DeleteTaskAsync(string id, CancellationToken cancellationToken);

    Task<IEnumerable<TaskEntity>> SearchTasksAsync(string keyPhrase, int take, int skip, CancellationToken cancellationToken);
}