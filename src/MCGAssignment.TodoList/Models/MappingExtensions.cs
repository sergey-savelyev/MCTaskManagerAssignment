using MCGAssignment.TodoList.DataTransferObjects;

namespace MCGAssignment.TodoList.Models;

public static class MappingExtensions
{
    public static TaskViewDetailed ToDetailedView(this TaskEntity document)
    {
        return new TaskViewDetailed
        {
            Id = document.Id,
            RootId = document.RootTaskId,
            Summary = document.Summary,
            Priority = document.Priority,
            Status = document.Status,
            CreateDate = document.CreateDate,
            DueDate = document.DueDate
        };
    }

    public static TaskViewFull ToFullView(this TaskEntity document, IEnumerable<TaskViewBase> subtasks)
    {
        return new TaskViewFull
        {
            Id = document.Id,
            RootId = document.RootTaskId,
            Summary = document.Summary,
            Priority = document.Priority,
            Status = document.Status,
            Description = document.Description,
            CreateDate = document.CreateDate,
            DueDate = document.DueDate,
            Subtasks = subtasks.ToList()
        };
    }
}