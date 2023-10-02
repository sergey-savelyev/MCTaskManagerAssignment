using System.Linq.Expressions;
using MCGAssignment.TodoList.Application.Exceptions;
using MCGAssignment.TodoList.Application.Repositories;
using MCGAssignment.TodoList.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MCGAssignment.TodoList.Infrastructure.MySQL.Repositories;

public class TasksRepository : ITasksRepository
{
    private ApplicationDbContext _context;

    public TasksRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Tasks.FindAsync(id, cancellationToken);

        if (entity is null)
        {
            throw new EntityNotFoundException(id);
        }

        return entity;
    }

    public async Task<TaskEntity> AddAsync(TaskEntity entity, CancellationToken cancellationToken)
    {
        await _context.Tasks.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<TaskEntity> UpdateAsync(TaskEntity entity, CancellationToken cancellationToken)
    {
        var entityToUpdate = await _context.Tasks.FindAsync(entity.Id, cancellationToken);

        if (entityToUpdate is null)
        {
            throw new EntityNotFoundException(entity.Id);
        }

        entityToUpdate.Summary = entity.Summary;
        entityToUpdate.Description = entity.Description;
        entityToUpdate.DueDate = entity.DueDate;
        entityToUpdate.Priority = entity.Priority;
        entityToUpdate.Status = entity.Status;

        await _context.SaveChangesAsync(cancellationToken);
        
        return entityToUpdate;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Tasks.FindAsync(id, cancellationToken);
        _context.Tasks.Remove(entity ?? throw new EntityNotFoundException(id));

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetSubTasksAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var subtaskEntities = await _context.Tasks.Where(x => x.RootTaskId == taskId).ToListAsync(cancellationToken);

        return subtaskEntities;
    }

    public async Task<(IEnumerable<TaskEntity> Entities, object ContinuationToken)> GetRootTaskBatchAsync(int take, object continuationToken, string sortBy, bool descending, CancellationToken cancellationToken)
    {
        if (int.TryParse(continuationToken?.ToString(), out var skip) is false)
        {
            skip = 0;
        }

        var entities = await _context.Tasks
            .Where(x => x.RootTaskId == null)
            .OrderBy(ResolveOrderProperty(sortBy), descending)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        int newContinuationToken = 0;
        if (entities.Count == take)
        {
            newContinuationToken = skip + take;
        }

        return (Entities: entities, ContinuationToken: newContinuationToken);
    }

    public async Task<(IEnumerable<TaskSearchEntity> Entities, object ContinuationToken)> SearchTasksAsync(string keyPhrase, int take, object continuationToken, CancellationToken cancellationToken)
    {
        if (int.TryParse(continuationToken?.ToString(), out var skip) is false)
        {
            skip = 0;
        }

        keyPhrase = keyPhrase.Trim().ToLower();
        var entities = 
            await _context.Tasks
                .Where(x => 
                    x.Summary.ToLower().Contains(keyPhrase) 
                    || (x.Description != null && x.Description.ToLower().Contains(keyPhrase)))
                .OrderBy(x => x.CreateDate)
                .Skip(skip)
                .Take(take)
                .Select(x => new TaskSearchEntity(x.Id, x.Summary, x.Description))
                .ToListAsync(cancellationToken);

        int newContinuationToken = 0;
        if (entities.Count == take)
        {
            newContinuationToken = skip + take;
        }

        return (Entities: entities, ContinuationToken: newContinuationToken);
    }

    public async Task<IEnumerable<Guid>> GetAllSubtaskIdsRecursivelyAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var allSubtaskIds = await _context.Database.SqlQueryRaw<Guid>(
            @$"with recursive cte (Id, RootTaskId) as (
                select     Id, 
                            RootTaskId 
                from       todolist.Tasks 
                where      RootTaskId = ""{taskId}""
                union all
                select     t.Id, 
                            t.RootTaskId 
                from       todolist.Tasks t 
                inner join cte
                        on t.RootTaskId = cte.Id 
            ) 
            select cte.Id from cte;"
        ).ToListAsync(cancellationToken);

        return allSubtaskIds;
    }

    public async Task UpdateTaskRootAsync(Guid taskId, Guid? newRootId, CancellationToken cancellationToken)
    {
        var entity = await _context.Tasks.FindAsync(taskId, cancellationToken);

        if (entity is null)
        {
            throw new EntityNotFoundException(taskId);
        }

        entity.RootTaskId = newRootId;
        await _context.SaveChangesAsync(cancellationToken);
    }

    
    // I could use an expression tree here, but I don't really think it's worth it.
    // Expression trees are quite hard to read, understand and debug.
    // So if number of properties is small, I prefer to use old but gold switch-case.
    private static Expression<Func<TaskEntity, object?>> ResolveOrderProperty(string propertyName) => propertyName switch
    {
        nameof(TaskEntity.Summary) => x => x.Summary,
        nameof(TaskEntity.CreateDate) => x => x.CreateDate,
        nameof(TaskEntity.DueDate) => x => x.DueDate,
        nameof(TaskEntity.Priority) => x => x.Priority,
        nameof(TaskEntity.Status) => x => x.Status,
        _ => throw new ArgumentException("Unsupported order property")
    };
}