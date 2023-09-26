using System.Linq.Expressions;
using MCGAssignment.TodoList.Exceptions;
using MCGAssignment.TodoList.Models;
using Microsoft.EntityFrameworkCore;

namespace MCGAssignment.TodoList.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TodoListContext _context;

    public TaskRepository(TodoListContext context)
    {
        _context = context;
    }

    public async Task DeleteTaskAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await _context.Task.FindAsync(id, cancellationToken).ConfigureAwait(false);
        _context.Task.Remove(entity ?? throw new EntityNotFoundException(id));

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetSubtasksAsync(string parentId, CancellationToken cancellationToken)
    {
        var tasks = 
            await _context.Task
                .Where(x => x.RootTaskId == parentId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

        return tasks ?? new ();
    }

    public async Task<TaskEntity> GetTaskAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await _context.Task.FindAsync(id, cancellationToken).ConfigureAwait(false);

        if (entity is null)
        {
            throw new EntityNotFoundException(id);
        }

        return entity;
    }

    public async Task<IEnumerable<TaskEntity>> GetRootTaskBatchAsync(int take, int skip, string sortBy, bool descending, CancellationToken cancellationToken)
    {
        var query = _context.Task.Where(x => x.RootTaskId == null);

        var orderPropertyExpression = ResolveOrderProperty(sortBy);
        var orderedQuery = descending
            ? query.OrderByDescending(orderPropertyExpression)
            : query.OrderBy(orderPropertyExpression);

        query = orderedQuery.Skip(skip).Take(take);
        var entities = await query.ToListAsync(cancellationToken).ConfigureAwait(false);

        return entities;
    }

    public async Task<IEnumerable<TaskEntity>> SearchTasksAsync(string keyPhrase, int take, int skip, CancellationToken cancellationToken)
    {
        keyPhrase = keyPhrase.Trim().ToLower();
        var entities = 
            await _context.Task.Where(x => x.Summary.ToLower().Contains(keyPhrase)).Skip(skip).Take(take).ToListAsync(cancellationToken).ConfigureAwait(false);

        return entities;
    }

    public async Task<TaskEntity> UpdateTaskAsync(TaskEntity task, CancellationToken cancellationToken)
    {
        _context.Task.Update(task);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return task;
    }

    public async Task<TaskEntity> CreateTaskAsync(TaskEntity task, CancellationToken cancellationToken)
    {
        _context.Task.Add(task);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return task;
    }

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