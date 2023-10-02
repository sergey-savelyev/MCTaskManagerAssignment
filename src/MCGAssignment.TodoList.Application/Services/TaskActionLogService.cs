using System.Text.Json;
using MCGAssignment.TodoList.Application.Extensions;
using MCGAssignment.TodoList.Application.DataTransferObjects;
using MCGAssignment.TodoList.Application.Repositories;
using MCGAssignment.TodoList.Core.Entities;
using MCGAssignment.TodoList.Core.Enums;

namespace MCGAssignment.TodoList.Application.Services;

public class TaskActionLogService : ITaskActionLogService
{
    private readonly ILogsRepository _repository;

    public TaskActionLogService(ILogsRepository repository)
    {
        _repository = repository;
    }

    public async Task LogTaskActionAsync(TaskAction action, Guid? entityId, object? payload, CancellationToken cancellationToken)
    {
        var logEntry = new LogEntity
        {
            Id = Guid.NewGuid(),
            Action = action,
            EntityType = nameof(TaskEntity),
            EntityId = entityId,
            Payload = payload is not null ? JsonSerializer.Serialize(payload) : null,
            TimestampMsec = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        await _repository.AddAsync(logEntry, cancellationToken);
    }

    public async Task<(IEnumerable<LogEntryView> Entities, object ContinuationToken)> GetTaskActionLogBatchAsync(object continuationToken, int take, bool descending, CancellationToken cancellationToken)
    {
        var result = await _repository.GetLogBatchByEntityTypeAsync(nameof(TaskEntity), continuationToken, take, descending, cancellationToken);

        return (result.Entities.Select(x => x.ToView()), result.ContinuationToken);
    }

    public async Task<(IEnumerable<LogEntryView> Entities, object ContinuationToken)> GetTaskActionLogBatchByTaskAsync(Guid taskId, object continuationToken, int take,bool descending, CancellationToken cancellationToken)
    {
        var result = await _repository.GetLogBatchByEntityAsync(taskId, continuationToken, take, descending, cancellationToken);

        return (result.Entities.Select(x => x.ToView()), result.ContinuationToken);
    }
}