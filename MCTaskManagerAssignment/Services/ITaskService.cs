using MCTaskManagerAssignment.DataTransferObjects;

namespace MCTaskManagerAssignment.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskViewDetailed>> GetTaskBatchAsync(int take, int skip, string sortBy, bool descending, CancellationToken cancellationToken);

    Task<TaskViewFull> GetTaskAsync(string taskId, CancellationToken cancellationToken);

    Task<string> CreateOrUpdateTaskAsync(UpsertTaskData taskDetails, CancellationToken cancellationToken);

    Task UpdateTaskRootAsync(string taskId, string newRootId, CancellationToken cancellationToken);

    Task DeleteTaskAsync(string taskId, CancellationToken cancellationToken);

    Task<IEnumerable<TaskSearchView>> SearchTasksAsync(string keyPhrase, string[] searchBy, int take, int skip, CancellationToken cancellationToken);
}