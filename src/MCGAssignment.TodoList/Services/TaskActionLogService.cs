using System.Linq.Expressions;
using MCGAssignment.TodoList.DataTransferObjects;
using MCGAssignment.TodoList.Models;
using MCGAssignment.TodoList.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCGAssignment.TodoList.Services;

public class TaskActionLogService : ITaskActionLogService
{
    private readonly TodoListContext _context;

    public TaskActionLogService(TodoListContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchAsync(int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken)
    {
        var entityType = typeof(TaskEntity).Name;
        var orderByProperty = ResolveOrderProperty(orderBy);
        var query = _context.Logs.Where(x => x.EntityType == entityType);
        var queryOrdered = descending 
            ? query.OrderByDescending(orderByProperty)
            : query.OrderBy(orderByProperty);

        var entities = queryOrdered.Skip(skip).Take(take);
        var views = await entities.Select(x => x.ToView()).ToListAsync(cancellationToken);

        return views;
    }

    public async Task<IEnumerable<LogEntryView>> GetTaskActionLogBatchByTaskAsync(Guid taskId, int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken)
    {
        var orderByProperty = ResolveOrderProperty(orderBy);
        var query = _context.Logs.Where(x => x.EntityId.HasValue && x.EntityId == taskId);
        var queryOrdered = descending 
            ? query.OrderByDescending(orderByProperty)
            : query.OrderBy(orderByProperty);

        var entities = queryOrdered.Skip(skip).Take(take);

        var views = await entities.Select(x => x.ToView()).ToListAsync(cancellationToken);

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