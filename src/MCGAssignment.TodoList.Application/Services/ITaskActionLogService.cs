using MCGAssignment.TodoList.Application.DataTransferObjects;
using MCGAssignment.TodoList.Core.Enums;

namespace MCGAssignment.TodoList.Application.Services;

public interface ITaskActionLogService
{
    Task LogTaskActionAsync(TaskAction action, Guid? entityId, object? payload, CancellationToken cancellationToken);

    Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchAsync(object continuationToken, int take, bool descending, CancellationToken cancellationToken);

    Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchByTaskAsync(Guid taskId, object continuationToken, int take, bool descending, CancellationToken cancellationToken);
}