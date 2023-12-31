using MCGAssignment.TodoList.Application.DataTransferObjects;
using MCGAssignment.TodoList.Core.Entities;

namespace MCGAssignment.TodoList.Application.Extensions;

public static class MappingExtensions
{
    public static TaskViewBase ToBaseView(this TaskEntity entity) => new TaskViewBase
    {
        Id = entity.Id,
        Summary = entity.Summary,
        Priority = entity.Priority,
        Status = entity.Status
    };

    public static TaskViewDetailed ToDetailedView(this TaskEntity entity)=> new TaskViewDetailed
    {
        Id = entity.Id,
        RootId = entity.RootTaskId,
        Summary = entity.Summary,
        Priority = entity.Priority,
        Status = entity.Status,
        CreateDate = entity.CreateDate,
        DueDate = entity.DueDate
    };

    public static TaskViewFull ToFullView(this TaskEntity entity, IEnumerable<TaskViewBase> subtasks) => new TaskViewFull
    {
        Id = entity.Id,
        RootId = entity.RootTaskId,
        RootTask = entity.RootTask?.ToBaseView(),
        Summary = entity.Summary,
        Priority = entity.Priority,
        Status = entity.Status,
        Description = entity.Description,
        CreateDate = entity.CreateDate,
        DueDate = entity.DueDate,
        Subtasks = subtasks.ToList()
    };

    public static LogEntryView ToView(this LogEntity entity) =>
        new LogEntryView(entity.Id, entity.Action, entity.TimestampMsec, entity.EntityId, entity.EntityType, entity.Payload);

    public static TaskSearchView ToSearchView(this TaskSearchEntity entity) => new TaskSearchView(entity.Id, entity.Summary, entity.Description);
}