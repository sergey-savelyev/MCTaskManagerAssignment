using System.Linq.Expressions;
using System.Text.Json;
using MCGAssignment.TodoList.Extensions;
using MCGAssignment.TodoList.Lib.DataTransferObjects;
using MCGAssignment.TodoList.Lib.Enums;
using MCGAssignment.TodoList.Models;
using MCGAssignment.TodoList.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCGAssignment.TodoList.Services;

public class TaskActionLogService : ITaskActionLogService
{
    private readonly ApplicationDbContext _context;

    public TaskActionLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task LogTaskActionAsync(TaskAction action, Guid? entityId, object? payload, CancellationToken cancellationToken)
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

        _context.Logs.Add(logEntry);

        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchAsync(int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken)
    {
        var views = await _context.Logs
            .Where(x => x.EntityType == nameof(TaskEntity))
            .OrderBy(ResolveOrderProperty(orderBy), descending)
            .Skip(skip)
            .Take(take)
            .Select(x => x.ToView())
            .ToListAsync(cancellationToken);

        return views;
    }

    public async Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchByTaskAsync(Guid taskId, int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken)
    {
        var views = await _context.Logs
            .Where(x => x.EntityId.HasValue && x.EntityId == taskId)
            .OrderBy(ResolveOrderProperty(orderBy), descending)
            .Skip(skip)
            .Take(take)
            .Select(x => x.ToView())
            .ToListAsync(cancellationToken);

        return views;
    }

    private static Expression<Func<LogEntity, object?>> ResolveOrderProperty(string propertyName) => propertyName switch
    {
        nameof(LogEntity.Id) => x => x.Id,
        nameof(LogEntity.Action) => x => x.Action,
        nameof(LogEntity.TimestampMsec) => x => x.TimestampMsec,
        nameof(LogEntity.EntityId) => x => x.EntityId,
        nameof(LogEntity.Payload) => x => x.Payload,
        _ => throw new ArgumentException("Unsupported order property")
    };
}