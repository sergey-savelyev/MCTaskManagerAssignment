using System.Linq.Expressions;
using MCGAssignment.TodoList.Application.Exceptions;
using MCGAssignment.TodoList.Application.Repositories;
using MCGAssignment.TodoList.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MCGAssignment.TodoList.Infrastructure.MySQL.Repositories;

public class LogsRepository : ILogsRepository
{
    private ApplicationDbContext _context;

    public LogsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LogEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Logs.FindAsync(id, cancellationToken);

        if (entity is null)
        {
            throw new EntityNotFoundException(id);
        }

        return entity;
    }

    public async Task<LogEntity> AddAsync(LogEntity entity, CancellationToken cancellationToken)
    {
        await _context.Logs.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<LogEntity> UpdateAsync(LogEntity entity, CancellationToken cancellationToken)
    {
        _context.Logs.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Logs.FindAsync(id, cancellationToken);

        if (entity is null)
        {
            throw new EntityNotFoundException(id);
        }

        _context.Logs.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<LogEntity>> GetLogBatchByEntityTypeAsync<TEntity>(int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken)
    {
        var entities = await _context.Logs
            .Where(x => x.EntityType == nameof(TEntity))
            .OrderBy(ResolveOrderProperty(orderBy), descending)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return entities;
    }

    public async Task<IEnumerable<LogEntity>> GetLogBatchByEntityAsync(Guid entityId, int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken)
    {
        var entities = await _context.Logs
            .Where(x => x.EntityId.HasValue && x.EntityId == entityId)
            .OrderBy(ResolveOrderProperty(orderBy), descending)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return entities;
    }

    // I could use an expression tree here, but I don't really think it's worth it.
    // Expression trees are quite hard to read, understand and debug.
    // So if number of properties is small, I prefer to use old but gold switch-case.
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