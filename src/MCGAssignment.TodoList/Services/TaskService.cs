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
        await _taskRepository.DeleteTaskAsync(taskId, cancellationToken);
    }

    public async Task<TaskViewFull> GetTaskAsync(string taskId, CancellationToken cancellationToken)
    {
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

        var documentToUpdate = new TaskEntity
        {
            Id = taskDetails.Id,
            Summary = taskDetails.Summary,
            Description = taskDetails.Description,
            DueDate = taskDetails.DueDate,
            Priority = taskDetails.Priority,
            Status = taskDetails.Status
        };

        var updated = await _taskRepository.UpdateTaskAsync(documentToUpdate, cancellationToken);

        return updated.Id;
    }

    public async Task UpdateTaskRootAsync(string taskId, string? newRootId, CancellationToken cancellationToken)
    {
        var entity = await _taskRepository.GetTaskAsync(taskId, cancellationToken).ConfigureAwait(false);
        var updated = entity with { RootTaskId = newRootId };

        await _taskRepository.UpdateTaskAsync(updated, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<TaskSearchView>> SearchTasksAsync(string keyPhrase, int take, int skip, CancellationToken cancellationToken)
    {
        var entities = await _taskRepository.SearchTasksAsync(keyPhrase, take, skip, cancellationToken);
        var searchResult = entities.Select(x => new TaskSearchView(x.Id, x.Summary, x.Description));

        return searchResult;
    }
}