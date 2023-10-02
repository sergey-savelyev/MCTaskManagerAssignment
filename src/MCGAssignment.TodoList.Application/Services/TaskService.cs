using System.Linq.Expressions;
using MCGAssignment.TodoList.Application.Exceptions;
using MCGAssignment.TodoList.Application.Extensions;
using MCGAssignment.TodoList.Application.DataTransferObjects;
using MCGAssignment.TodoList.Application.Repositories;
using MCGAssignment.TodoList.Core.Entities;
using MCGAssignment.TodoList.Core.Enums;

namespace MCGAssignment.TodoList.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITasksRepository _repository;
    private readonly ITaskActionLogService _logService;

    public TaskService(ITasksRepository repository, ITaskActionLogService logService)
    {
        _repository = repository;
        _logService = logService;
    }

    public async Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(taskId, cancellationToken);
        await _logService.LogTaskActionAsync(TaskAction.Delete, taskId, null, cancellationToken);
    }

    public async Task<TaskViewFull> GetTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var taskEntity = await _repository.GetByIdAsync(taskId, cancellationToken);
        var subtaskEntities = await _repository.GetSubTasksAsync(taskId, cancellationToken);

        var result = taskEntity.ToFullView(subtaskEntities.Select(x => new TaskViewBase
        {
            Id = x.Id,
            Summary = x.Summary,
            Priority = x.Priority,
            Status = x.Status
        }));

        return result;
    }

    public async Task<IEnumerable<TaskViewDetailed>> GetRootTaskBatchAsync(int take, int skip, string sortBy, bool descending, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetRootTaskBatchAsync(take, skip, sortBy, descending, cancellationToken);
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

        await _repository.AddAsync(newEntity, cancellationToken);
        await _logService.LogTaskActionAsync(TaskAction.Create, newEntity.Id, null, cancellationToken);

        return newEntity.Id;
    }

    public async Task UpdateTaskAsync(Guid taskId, UpsertTaskData updateData, CancellationToken cancellationToken)
    {        
        var entity = new TaskEntity
        {
            Id = taskId,
            Summary = updateData.Summary,
            Description = updateData.Description,
            DueDate = updateData.DueDate,
            Priority = updateData.Priority,
            Status = updateData.Status
        };

        await _repository.UpdateAsync(entity, cancellationToken);
        await _logService.LogTaskActionAsync(TaskAction.Update, taskId, entity, cancellationToken);
    }

    public async Task UpdateTaskRootAsync(Guid taskId, Guid? newRootId, CancellationToken cancellationToken)
    {
        if (taskId == newRootId)
        {
            throw new InvalidRootBindingException("Can't bind task to itself");
        }

        if (newRootId is not null)
        {
            var flatSubtaskIds = await _repository.GetAllSubtaskIdsRecursivelyAsync(taskId, cancellationToken);

            if (flatSubtaskIds.Contains(newRootId.Value))
            {
                throw new InvalidRootBindingException("Can't bind task to its subtask");
            }
        }
        
        await _repository.UpdateTaskRootAsync(taskId, newRootId, cancellationToken);
        await _logService.LogTaskActionAsync(TaskAction.RootChanged, taskId, new { RootId = newRootId }, cancellationToken);
    }

    public async Task<IEnumerable<TaskSearchView>> SearchTasksAsync(string keyPhrase, int take, int skip, CancellationToken cancellationToken)
    {
        var searchResultEntities = await _repository.SearchTasksAsync(keyPhrase, take, skip, cancellationToken);

        return searchResultEntities.Select(x => x.ToSearchView());
    }
}