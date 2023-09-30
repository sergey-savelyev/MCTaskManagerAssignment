using MCGAssignment.TodoList.DataTransferObjects;

namespace MCGAssignment.TodoList.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskViewDetailed>> GetRootTaskBatchAsync(int take, int skip, string sortBy, bool descending, CancellationToken cancellationToken);

    Task<TaskViewFull> GetTaskAsync(Guid taskId, CancellationToken cancellationToken);

    Task<Guid> CreateTaskAsync(UpsertTaskData taskDetails, CancellationToken cancellationToken);

    Task UpdateTaskAsync(Guid taskId, UpsertTaskData taskDetails, CancellationToken cancellationToken);

    Task UpdateTaskRootAsync(Guid taskId, Guid? newRootId, CancellationToken cancellationToken);

    Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken);

    Task<IEnumerable<TaskSearchView>> SearchTasksAsync(string keyPhrase, int take, int skip, CancellationToken cancellationToken);
}