using MCGAssignment.TodoList.DataTransferObjects;
using MCGAssignment.TodoList.Models;
using MCGAssignment.TodoList.Repositories;

namespace MCGAssignment.TodoList.Services;

public class TaskActionLogService : ITaskActionLogService
{
    private readonly ILogRepository _logRepository;

    public TaskActionLogService(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchAsync(int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken)
    {
        var entities = await _logRepository.GetLogEntryBatchAsync<TaskEntity>(skip, take, orderBy, descending, cancellationToken);
        var views = entities.Select(x => x.ToView()).ToList();

        return views;
    }

    public async Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchByTaskAsync(string taskId, int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken)
    {
        var entities = await _logRepository.GetLogEntryBatchByEntityIdAsync(taskId, skip, take, orderBy, descending, cancellationToken);
        var views = entities.Select(x => x.ToView()).ToList();

        return views;
    }
}