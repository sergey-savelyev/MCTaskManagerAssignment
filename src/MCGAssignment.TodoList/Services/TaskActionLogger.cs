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

    private readonly TodoListContext _context;

    public TaskActionLogger(TodoListContext context)
    {
        _context = context;
    }

    public async Task LogCreateAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var entry = new LogEntity
        {
            Id = Guid.NewGuid(),
            Action = TaskActionCreate,
            TimestampMsec = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            EntityId = taskId,
            EntityType = typeof(TaskEntity).Name,
        };

        await _context.Logs.AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task LogDeleteAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var entry = new LogEntity
        {
            Id = Guid.NewGuid(),
            Action = TaskActionDelete,
            TimestampMsec = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            EntityId = taskId,
            EntityType = typeof(TaskEntity).Name,
        };

        await _context.Logs.AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task LogRootChangedAsync(Guid taskId, Guid? rootId, CancellationToken cancellationToken)
    {
        var entry = new LogEntity
        {
            Id = Guid.NewGuid(),
            Action = TaskActionRootChanged,
            TimestampMsec = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            EntityId = taskId,
            EntityType = typeof(TaskEntity).Name,
            Payload = JsonSerializer.Serialize(new { RootId = rootId })
        };

        await _context.Logs.AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task LogUpdateAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var entry = new LogEntity
        {
            Id = Guid.NewGuid(),
            Action = TaskActionUpdate,
            TimestampMsec = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            EntityId = taskId,
            EntityType = typeof(TaskEntity).Name,
        };

        await _context.Logs.AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}