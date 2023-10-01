using System.Linq.Expressions;
using MCGAssignment.TodoList.Exceptions;
using MCGAssignment.TodoList.Extensions;
using MCGAssignment.TodoList.Lib.DataTransferObjects;
using MCGAssignment.TodoList.Lib.Enums;
using MCGAssignment.TodoList.Models;
using MCGAssignment.TodoList.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCGAssignment.TodoList.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;
    private readonly ITaskActionLogService _logService;

    public TaskService(ApplicationDbContext context, ITaskActionLogService logService)
    {
        _context = context;
        _logService = logService;
    }

    public async Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var entity = await _context.Tasks.FindAsync(taskId, cancellationToken);
        _context.Tasks.Remove(entity ?? throw new EntityNotFoundException(taskId));

        await _context.SaveChangesAsync(cancellationToken);
        await _logService.LogTaskActionAsync(TaskAction.Delete, taskId, null, cancellationToken);
    }

    public async Task<TaskViewFull> GetTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var taskDocument = await _context.Tasks.FindAsync(taskId, cancellationToken);

        if (taskDocument is null)
        {
            throw new EntityNotFoundException(taskId);
        }

        var subtaskDocuments = _context.Tasks.Where(x => x.RootTaskId == taskId);

        var task = taskDocument.ToFullView(subtaskDocuments.Select(x => new TaskViewBase
        {
            Id = x.Id,
            Summary = x.Summary,
            Priority = x.Priority,
            Status = x.Status
        }));

        return task;
    }

    public async Task<IEnumerable<TaskViewDetailed>> GetRootTaskBatchAsync(int take, int skip, string sortBy, bool descending, CancellationToken cancellationToken)
    {
        var query = _context.Tasks.Where(x => x.RootTaskId == null);

        var orderPropertyExpression = ResolveOrderProperty(sortBy);
        var orderedQuery = descending
            ? query.OrderByDescending(orderPropertyExpression)
            : query.OrderBy(orderPropertyExpression);

        query = orderedQuery.Skip(skip).Take(take);
        var entities = await query.ToListAsync(cancellationToken);

        var taskDetails = entities.Select(x => x.ToDetailedView());

        return taskDetails;
    }

    public async Task<Guid> CreateTaskAsync(UpsertTaskData taskDetails, CancellationToken cancellationToken)
    {
         var newEntity = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Summary = taskDetails.Summary,
            Description = taskDetails.Description,
            CreateDate = DateTime.UtcNow,
            DueDate = taskDetails.DueDate,
            Priority = taskDetails.Priority,
            Status = taskDetails.Status
        };

        await _context.Tasks.AddAsync(newEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _logService.LogTaskActionAsync(TaskAction.Create, newEntity.Id, null, cancellationToken);

        return newEntity.Id;
    }

    public async Task UpdateTaskAsync(Guid taskId, UpsertTaskData updateData, CancellationToken cancellationToken)
    {        
        var entity = await _context.Tasks.FindAsync(taskId);

        if (entity is null)
        {
            throw new EntityNotFoundException(taskId);
        }

        entity.Summary = updateData.Summary;
        entity.Description = updateData.Description;
        entity.DueDate = updateData.DueDate;
        entity.Priority = updateData.Priority;
        entity.Status = updateData.Status;

        await _context.SaveChangesAsync(cancellationToken);
        await _logService.LogTaskActionAsync(TaskAction.Update, taskId, entity, cancellationToken);
    }

    public async Task UpdateTaskRootAsync(Guid taskId, Guid? newRootId, CancellationToken cancellationToken)
    {
        if (taskId == newRootId)
        {
            throw new InvalidRootBindingException("Can't bind task to itself");
        }

        var entity = await _context.Tasks.FindAsync(taskId);

        if (entity is null)
        {
            throw new EntityNotFoundException(taskId);
        }

        if (newRootId is not null)
        {
            var flatSubtaskIds = await GetAllSubtaskIdsRecursivelyAsync(taskId, cancellationToken);

            if (flatSubtaskIds.Contains(newRootId.Value))
            {
                throw new InvalidRootBindingException("Can't bind task to its subtask");
            }
        }
        
        entity.RootTaskId = newRootId;
        await _context.SaveChangesAsync(cancellationToken);

        await _logService.LogTaskActionAsync(TaskAction.RootChanged, taskId, new { RootId = newRootId }, cancellationToken);
    }

    public async Task<IEnumerable<TaskSearchView>> SearchTasksAsync(string keyPhrase, int take, int skip, CancellationToken cancellationToken)
    {
        keyPhrase = keyPhrase.Trim().ToLower();
        var entities = 
            await _context.Tasks
                .Where(x => 
                    x.Summary.ToLower().Contains(keyPhrase) 
                    || (x.Description != null && x.Description.ToLower().Contains(keyPhrase)))
                .OrderBy(x => x.CreateDate)
                .Skip(skip)
                .Take(take)
                .Select(x => new TaskSearchView(x.Id, x.Summary, x.Description))
                .ToListAsync(cancellationToken);

        return entities;
    }

    private async Task<IEnumerable<Guid>> GetAllSubtaskIdsRecursivelyAsync(Guid taskId, CancellationToken cancellationToken)
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