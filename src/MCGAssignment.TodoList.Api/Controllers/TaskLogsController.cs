using MCGAssignment.TodoList.Application.DataTransferObjects;
using MCGAssignment.TodoList.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MCGAssignment.TodoList.Controllers;

[Route("api")]
[ApiController]
public class TaskLogsController : ControllerBase
{
    private readonly ITaskActionLogService _taskActionLogService;

    public TaskLogsController(ITaskActionLogService taskActionLogService)
    {
        _taskActionLogService = taskActionLogService;
    }

    [HttpGet("tasks/{taskId}/logs")]
    public async Task<IActionResult> GetTaskLogs(
        [FromRoute] string taskId, CancellationToken cancellationToken, [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        bool descending = true)
    {
        if (!Guid.TryParse(taskId, out var taskIdGuid))
        {
            return BadRequest("Invalid task id");
        }

        var logs = await _taskActionLogService.GetTaskActionLogBatchByTaskAsync(taskIdGuid, skip, take, descending, cancellationToken);

        return Ok(logs);
    }

    [HttpGet("tasks/logs")]
    public async Task<IActionResult> GetAllLogs(
        CancellationToken cancellationToken, [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        bool descending = true)
    {
        var logs = await _taskActionLogService.GetTaskActionLogBatchAsync(skip, take, descending, cancellationToken);

        return Ok(logs);
    }
}