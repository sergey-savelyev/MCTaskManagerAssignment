using MCTaskManagerAssignment.DataTransferObjects;
using MCTaskManagerAssignment.Models;
using MCTaskManagerAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace MCTaskManagerAssignment.Controllers;

[Route("tasks")]
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
        var task = await _taskService.GetTaskAsync(taskId, cancellationToken);
        
        return Ok(task);
    }

    [HttpGet]
    public async Task<IActionResult> GetTaskBatchAsync([FromQuery] int skip, [FromQuery] int take, CancellationToken cancellationToken, [FromQuery] string sortBy = TaskSortFields.CreateDate, [FromQuery] bool descendingSort = false)
    {
        var tasks = await _taskService.GetTaskBatchAsync(take, skip, sortBy, descendingSort, cancellationToken);
        
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> UpsertTaskAsync([FromBody] UpsertTaskData taskData, CancellationToken cancellationToken)
    {
        var taskId = await _taskService.CreateOrUpdateTaskAsync(taskData, cancellationToken);

        return Ok(new UpsertTaskResponse(taskId));
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTaskAsync([FromRoute] string taskId, CancellationToken cancellationToken)
    {
        await _taskService.DeleteTaskAsync(taskId, cancellationToken);

        return NoContent();
    }
}