using MCTaskManagerAssignment.Models;

namespace MCTaskManagerAssignment.Repositories;

public interface ITaskRepository
{
    Task<TaskDocument> GetTaskAsync(string id, CancellationToken cancellationToken);

    Task<IEnumerable<TaskDocument>> GetTaskBatchAsync(int take, int skip, string sortBy, bool descending, CancellationToken cancellationToken);

    Task<IEnumerable<TaskDocument>> GetChildTasksAsync(string parentId, CancellationToken cancellationToken);

    Task<TaskDocument> UpsertTaskAsync(TaskDocument task, CancellationToken cancellationToken);

    Task<bool> DeleteTaskAsync(string id, CancellationToken cancellationToken);
}