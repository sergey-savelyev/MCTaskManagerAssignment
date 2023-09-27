using System.Linq.Expressions;
using MCGAssignment.TodoList.Exceptions;
using MCGAssignment.TodoList.Models;
using Microsoft.EntityFrameworkCore;

namespace MCGAssignment.TodoList.Repositories;

public class LogRepository : ILogRepository
{
    private readonly TodoListContext _context;

    public LogRepository(TodoListContext context)
    {
        _context = context;
    }

    public async Task CreateLogEntryAsync(LogEntity entity, CancellationToken cancellationToken)
    {
        await _context.LogEntry.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<LogEntity> GetLogEntryAsync(string entityId, CancellationToken cancellationToken)
    {
        var entity = await _context.LogEntry.FindAsync(entityId, cancellationToken);

        if (entity is null)
        {
            throw new EntityNotFoundException(entityId);
        }

        return entity;
    }

    public async Task<IEnumerable<LogEntity>> GetLogEntryBatchAsync<TEntity>(int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken)
    {
        var entityType = typeof(TEntity).Name;
        var orderByProperty = ResolveOrderProperty(orderBy);
        var query = _context.LogEntry.Where(x => x.EntityType == entityType);
        var queryOrdered = descending 
            ? query.OrderByDescending(orderByProperty)
            : query.OrderBy(orderByProperty);

        var entities = await queryOrdered.Skip(skip).Take(take).ToListAsync(cancellationToken).ConfigureAwait(false);

        return entities;
    }

    public async Task<IEnumerable<LogEntity>> GetLogEntryBatchByEntityIdAsync(string entityId, int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken)
    {
        var orderByProperty = ResolveOrderProperty(orderBy);
        var query = _context.LogEntry.Where(x => x.EntityId == entityId);
        var queryOrdered = descending 
            ? query.OrderByDescending(orderByProperty)
            : query.OrderBy(orderByProperty);

        var entities = await queryOrdered.Skip(skip).Take(take).ToListAsync(cancellationToken).ConfigureAwait(false);

        return entities;
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