using MCTaskManagerAssignment.DataTransferObjects;

namespace MCTaskManagerAssignment.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskViewDetailed>> GetTaskBatchAsync(int take, int skip, string sortBy, bool descending, CancellationToken cancellationToken);

    Task<TaskViewFull> GetTaskAsync(string taskId, CancellationToken cancellationToken);

    Task<string> CreateOrUpdateTaskAsync(UpsertTaskData taskDetails, CancellationToken cancellationToken);

    Task UpdateTaskParentAsync(string taskId, string newParentId, CancellationToken cancellationToken);

    Task DeleteTaskAsync(string taskId, CancellationToken cancellationToken);
}