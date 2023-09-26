using MCGAssignment.TodoList.Exceptions;
using MCGAssignment.TodoList.DataTransferObjects;
using MCGAssignment.TodoList.Services;
using Microsoft.AspNetCore.Mvc;

namespace MCGAssignment.TodoList.Controllers;

[Route("api/tasks")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTaskAsync([FromRoute] string taskId, CancellationToken cancellationToken)
    {
        try
        {
            var task = await _taskService.GetTaskAsync(taskId, cancellationToken);

            return Ok(task);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("search/{phrase}")]
    public async Task<IActionResult> SearchTasksAsync([FromRoute] string phrase, [FromQuery] int take,
                                                      [FromQuery] int skip, CancellationToken cancellationToken)
    {
        var result = await _taskService.SearchTasksAsync(phrase, take, skip, cancellationToken);

        return Ok(result.ToList());
    }

    [HttpGet]
    public async Task<IActionResult> GetRootTaskBatchAsync([FromQuery] int skip, [FromQuery] int take,
                                                       CancellationToken cancellationToken,
                                                       [FromQuery] string sortBy = nameof(TaskViewFull.CreateDate),
                                                       [FromQuery] bool descendingSort = false)
    {
        var tasks = await _taskService.GetRootTaskBatchAsync(take, skip, sortBy, descendingSort, cancellationToken);
        
        return Ok(tasks.ToList());
    }

    [HttpPost]
    public async Task<IActionResult> UpsertTaskAsync([FromBody] UpsertTaskData taskData, CancellationToken cancellationToken)
    {
        var taskId = await _taskService.CreateOrUpdateTaskAsync(taskData, cancellationToken);

        return Ok(new UpsertTaskResponse(taskId));
    }

    [HttpPatch("{taskId}/root")]
    public async Task<IActionResult> ChangeTaskRootAsync([FromRoute] string taskId, [FromBody] TaskRootData newRoot, CancellationToken cancellationToken)
    {
        try
        {
            await _taskService.UpdateTaskRootAsync(taskId, newRoot.RootId, cancellationToken);

            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTaskAsync([FromRoute] string taskId, CancellationToken cancellationToken)
    {
        try
        {
            await _taskService.DeleteTaskAsync(taskId, cancellationToken);

            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}