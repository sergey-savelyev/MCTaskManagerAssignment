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

    public async Task<(IEnumerable<LogEntity> Entities, object ContinuationToken)> GetLogBatchByEntityTypeAsync(string entityType, object continuationToken, int take, bool descending, CancellationToken cancellationToken)
    {
        if (int.TryParse(continuationToken?.ToString(), out var skip) is false)
        {
            skip = 0;
        }

        var entities = await _context.Logs
            .Where(x => x.EntityType == entityType)
            .OrderBy(x => x.TimestampMsec, descending)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        int newContinuationToken = 0;
        if (entities.Count == take)
        {
            newContinuationToken = skip + take;
        }

        return (entities, newContinuationToken);
    }

    public async Task<(IEnumerable<LogEntity> Entities, object ContinuationToken)> GetLogBatchByEntityAsync(Guid entityId, object continuationToken, int take, bool descending, CancellationToken cancellationToken)
    {
        if (int.TryParse(continuationToken?.ToString(), out var skip) is false)
        {
            skip = 0;
        }

        var entities = await _context.Logs
            .Where(x => x.EntityId.HasValue && x.EntityId == entityId)
            .OrderBy(x => x.TimestampMsec, descending)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        int newContinuationToken = 0;
        if (entities.Count == take)
        {
            newContinuationToken = skip + take;
        }

        return (entities, newContinuationToken);
    }
}