using MCGAssignment.TodoList.Api.ViewModels;
using MCGAssignment.TodoList.Application.DataTransferObjects;
using MCGAssignment.TodoList.Application.Exceptions;
using MCGAssignment.TodoList.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MCGAssignment.TodoList.Api.Controllers;

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
    public async Task<IActionResult> GetTaskAsync([FromRoute] Guid taskId, CancellationToken cancellationToken)
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

        return Ok(new TaskSearchResponse { Entities = result.Entities.ToList(), ContinuationToken = result.ContinuationToken });
    }

    [HttpGet]
    public async Task<IActionResult> GetRootTaskBatchAsync([FromQuery] int skip, [FromQuery] int take,
                                                       CancellationToken cancellationToken,
                                                       [FromQuery] string sortBy = nameof(TaskViewFull.CreateDate),
                                                       [FromQuery] bool descendingSort = false)
    {
        var result = await _taskService.GetRootTaskBatchAsync(take, skip, sortBy, descendingSort, cancellationToken);
        
        return Ok(new TaskResponse { Entities = result.Entities.ToList(), ContinuationToken = result.ContinuationToken });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaskAsync([FromBody] UpsertTaskDto taskData, CancellationToken cancellationToken)
    {
        var taskId = await _taskService.CreateTaskAsync(taskData, cancellationToken);

        return Ok(new CreeateTaskResponse(taskId));
    }


    [HttpPatch("{taskId}")]
    public async Task<IActionResult> UpdateTaskAsync([FromRoute] Guid taskId, [FromBody] UpsertTaskDto taskData, CancellationToken cancellationToken)
    {
        try
        {
            await _taskService.UpdateTaskAsync(taskId, taskData, cancellationToken);

            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPatch("{taskId}/root")]
    public async Task<IActionResult> ChangeTaskRootAsync([FromRoute] Guid taskId, [FromBody] TaskRootData newRoot, CancellationToken cancellationToken)
    {
        try
        {
            await _taskService.UpdateTaskRootAsync(taskId, newRoot.RootId, cancellationToken);

            return NoContent();
        }
        catch (InvalidRootBindingException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTaskAsync([FromRoute] Guid taskId, CancellationToken cancellationToken)
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