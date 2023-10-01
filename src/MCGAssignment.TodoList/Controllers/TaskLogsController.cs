using MCGAssignment.TodoList.Lib.DataTransferObjects;
using MCGAssignment.TodoList.Services;
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
        [FromQuery] int take = 20, [FromQuery] string orderby = nameof(LogEntryView.TimestampMsec),
        bool descending = true)
    {
        if (!Guid.TryParse(taskId, out var taskIdGuid))
        {
            return BadRequest("Invalid task id");
        }

        var logs = await _taskActionLogService.GetTaskActionLogBatchByTaskAsync(taskIdGuid, skip, take, orderby, descending, cancellationToken);

        return Ok(logs);
    }

    [HttpGet("tasks/logs")]
    public async Task<IActionResult> GetAllLogs(
        CancellationToken cancellationToken, [FromQuery] int skip = 0,
        [FromQuery] int take = 20, [FromQuery] string orderby = nameof(LogEntryView.TimestampMsec),
        bool descending = true)
    {
        var logs = await _taskActionLogService.GetTaskActionLogBatchAsync(skip, take, orderby, descending, cancellationToken);

        return Ok(logs);
    }
}