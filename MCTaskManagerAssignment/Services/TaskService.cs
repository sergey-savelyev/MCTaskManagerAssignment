using MCTaskManagerAssignment.DataTransferObjects;
using MCTaskManagerAssignment.Models;
using MCTaskManagerAssignment.Repositories;

namespace MCTaskManagerAssignment.Services;

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
        var childTaskDocuments = await _taskRepository.GetSubtasksAsync(taskId, cancellationToken);

        var task = taskDocument.ToFullView(childTaskDocuments.Select(x => new TaskViewBase
        {
            Id = x.Id,
            ParentId = x.ParentId,
            Summary = x.Summary,
            Priority = x.Priority,
            Status = x.Status
        }));

        return task;
    }

    public async Task<IEnumerable<TaskViewDetailed>> GetTaskBatchAsync(int take, int skip, string sortBy, bool descending, CancellationToken cancellationToken)
    {
        var taskDocuments = await _taskRepository.GetTaskBatchAsync(take, skip, sortBy, descending, cancellationToken);
        var taskDetails = taskDocuments.Select(x => x.ToDetailedView());

        return taskDetails;
    }

    public async Task<string> CreateOrUpdateTaskAsync(UpsertTaskData taskDetails, CancellationToken cancellationToken)
    {
        var document = new TaskDocument
        {
            Id = taskDetails.Id ?? Guid.NewGuid().ToString(),
            ParentId = taskDetails.ParentId,
            Summary = taskDetails.Summary,
            Description = taskDetails.Description,
            CreateDate = DateTime.UtcNow,
            DueDate = taskDetails.DueDate,
            Priority = taskDetails.Priority,
            Status = taskDetails.Status
        };

        await _taskRepository.UpsertTaskAsync(document, cancellationToken);

        return document.Id;
    }

    public async Task UpdateTaskParentAsync(string taskId, string newParentId, CancellationToken cancellationToken)
    {
        var document = await _taskRepository.GetTaskAsync(taskId, cancellationToken);
        document.ParentId = newParentId;

        await _taskRepository.UpsertTaskAsync(document, cancellationToken);
    }
}