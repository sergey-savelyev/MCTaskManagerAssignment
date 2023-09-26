using MCGAssignment.TodoList.DataTransferObjects;

namespace MCGAssignment.TodoList.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskViewDetailed>> GetRootTaskBatchAsync(int take, int skip, string sortBy, bool descending, CancellationToken cancellationToken);

    Task<TaskViewFull> GetTaskAsync(string taskId, CancellationToken cancellationToken);

    Task<string> CreateOrUpdateTaskAsync(UpsertTaskData taskDetails, CancellationToken cancellationToken);

    Task UpdateTaskRootAsync(string taskId, string? newRootId, CancellationToken cancellationToken);

    Task DeleteTaskAsync(string taskId, CancellationToken cancellationToken);

    Task<IEnumerable<TaskSearchView>> SearchTasksAsync(string keyPhrase, int take, int skip, CancellationToken cancellationToken);
}