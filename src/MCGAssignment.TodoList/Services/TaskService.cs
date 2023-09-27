using MCGAssignment.TodoList.DataTransferObjects;
using MCGAssignment.TodoList.Exceptions;
using MCGAssignment.TodoList.Models;
using MCGAssignment.TodoList.Repositories;

namespace MCGAssignment.TodoList.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task DeleteTaskAsync(string taskId, CancellationToken cancellationToken)
    {
        ThrowIfNotGuidOrNull(taskId);

        await _taskRepository.DeleteTaskAsync(taskId, cancellationToken);
    }

    public async Task<TaskViewFull> GetTaskAsync(string taskId, CancellationToken cancellationToken)
    {
        ThrowIfNotGuidOrNull(taskId);

        var taskDocument = await _taskRepository.GetTaskAsync(taskId, cancellationToken);

        if (taskDocument is null)
        {
            throw new EntityNotFoundException(taskId);
        }

        var childTaskDocuments = await _taskRepository.GetSubtasksAsync(taskId, cancellationToken);

        var task = taskDocument.ToFullView(childTaskDocuments.Select(x => new TaskViewBase
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
        var taskDocuments = await _taskRepository.GetRootTaskBatchAsync(take, skip, sortBy, descending, cancellationToken);
        var taskDetails = taskDocuments.Select(x => x.ToDetailedView());

        return taskDetails;
    }

    public async Task<string> CreateOrUpdateTaskAsync(UpsertTaskData taskDetails, CancellationToken cancellationToken)
    {
        if (taskDetails.Id is null)
        {
            var entityToCreate = new TaskEntity
            {
                Id = Guid.NewGuid().ToString(),
                Summary = taskDetails.Summary,
                Description = taskDetails.Description,
                CreateDate = DateTime.UtcNow,
                DueDate = taskDetails.DueDate,
                Priority = taskDetails.Priority,
                Status = taskDetails.Status
            };

            var created = await _taskRepository.CreateTaskAsync(entityToCreate, cancellationToken);

            return created.Id;
        }

        ThrowIfNotGuidOrNull(taskDetails.Id);
        ThrowIfNotGuidOrNull(taskDetails.RootId);

        var updatedEntity = new TaskEntity
        {
            Id = taskDetails.Id,
            RootTaskId = taskDetails.RootId,
            Summary = taskDetails.Summary,
            Description = taskDetails.Description,
            DueDate = taskDetails.DueDate,
            Priority = taskDetails.Priority,
            Status = taskDetails.Status
        };

        var updated = await _taskRepository.UpdateTaskAsync(updatedEntity, cancellationToken);

        return updated.Id;
    }

    public Task UpdateTaskRootAsync(string taskId, string? newRootId, CancellationToken cancellationToken)
    {
        ThrowIfNotGuidOrNull(taskId);
        ThrowIfNotGuidOrNull(newRootId);
        
        return _taskRepository.UpdateTaskRootAsync(taskId, newRootId, cancellationToken);
    }

    public async Task<IEnumerable<TaskSearchView>> SearchTasksAsync(string keyPhrase, int take, int skip, CancellationToken cancellationToken)
    {
        var entities = await _taskRepository.SearchTasksAsync(keyPhrase, take, skip, cancellationToken);
        var searchResult = entities.Select(x => new TaskSearchView(x.Id, x.Summary, x.Description));

        return searchResult;
    }

    private static void ThrowIfNotGuidOrNull(string? id)
    {
        if (id is not null && !Guid.TryParse(id, out _))
        {
            throw new ArgumentException("Id must be Guid or null");
        }
    }
}