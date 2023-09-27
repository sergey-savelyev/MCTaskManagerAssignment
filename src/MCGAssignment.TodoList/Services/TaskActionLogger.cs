using System.Text.Json;
using MCGAssignment.TodoList.Models;
using MCGAssignment.TodoList.Repositories;

namespace MCGAssignment.TodoList.Services;

public class TaskActionLogger : ITaskActionLogger
{
    private const string TaskActionCreate = "TaskCreate";
    private const string TaskActionDelete = "TaskDelete";
    private const string TaskActionUpdate = "TaskUpdate";
    private const string TaskActionRootChanged = "TaskRootChanged";

    private readonly ILogRepository _logRepository;

    public TaskActionLogger(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public Task LogCreateAsync(string taskId, CancellationToken cancellationToken)
    {
        var entry = new LogEntity
        {
            Id = Guid.NewGuid().ToString(),
            Action = TaskActionCreate,
            TimestampMsec = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            EntityId = taskId,
            EntityType = typeof(TaskEntity).Name,
            Payload = new JsonElement()
        };

        return _logRepository.CreateLogEntryAsync(entry, cancellationToken);
    }

    public Task LogDeleteAsync(string taskId, CancellationToken cancellationToken)
    {
        var entry = new LogEntity
        {
            Id = Guid.NewGuid().ToString(),
            Action = TaskActionDelete,
            TimestampMsec = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            EntityId = taskId,
            EntityType = typeof(TaskEntity).Name,
            Payload = new JsonElement()
        };

        return _logRepository.CreateLogEntryAsync(entry, cancellationToken);
    }

    public Task LogRootChangedAsync(string taskId, string? rootId, CancellationToken cancellationToken)
    {
        var entry = new LogEntity
        {
            Id = Guid.NewGuid().ToString(),
            Action = TaskActionRootChanged,
            TimestampMsec = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            EntityId = taskId,
            EntityType = typeof(TaskEntity).Name,
            Payload = JsonSerializer.SerializeToElement(new { RootId = rootId })
        };

        return _logRepository.CreateLogEntryAsync(entry, cancellationToken);
    }

    public Task LogUpdateAsync(string taskId, CancellationToken cancellationToken)
    {
        var entry = new LogEntity
        {
            Id = Guid.NewGuid().ToString(),
            Action = TaskActionUpdate,
            TimestampMsec = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            EntityId = taskId,
            EntityType = typeof(TaskEntity).Name,
            Payload = new JsonElement()
        };

        return _logRepository.CreateLogEntryAsync(entry, cancellationToken);
    }
}