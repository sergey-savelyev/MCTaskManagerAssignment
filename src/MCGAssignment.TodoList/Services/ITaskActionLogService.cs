using MCGAssignment.TodoList.Lib.DataTransferObjects;
using MCGAssignment.TodoList.Lib.Enums;

namespace MCGAssignment.TodoList.Services;

public interface ITaskActionLogService
{
    Task LogTaskActionAsync(TaskAction action, Guid? entityId, object? payload, CancellationToken cancellationToken);

    Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchAsync(int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken);

    Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchByTaskAsync(Guid taskId, int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken);
}