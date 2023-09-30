using MCGAssignment.TodoList.DataTransferObjects;

namespace MCGAssignment.TodoList.Services;

public interface ITaskActionLogService
{
    Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchAsync(int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken);

    Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchByTaskAsync(Guid taskId, int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken);
}