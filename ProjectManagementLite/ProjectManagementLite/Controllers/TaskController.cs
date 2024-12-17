// Controllers/TasksController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProjectManagementLite.Services;

[ApiController]
[Route("api/projects/{projectId}/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    
    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }
    
    // Helper to get User ID from JWT
    private int GetUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : throw new Exception("User ID not found in token.");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTasks(int projectId)
    {
        try
        {
            var userId = GetUserId();
            var tasks = await _taskService.GetTasksAsync(projectId, userId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTask(int projectId, int taskId)
    {
        try
        {
            var userId = GetUserId();
            var task = await _taskService.GetTaskByIdAsync(taskId, projectId, userId);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTask(int projectId, [FromBody] TaskCreateRequest request)
    {
        try
        {
            var userId = GetUserId();
            var task = await _taskService.CreateTaskAsync(projectId, request, userId);
            return CreatedAtAction(nameof(GetTask), new { projectId = projectId, taskId = task.Id }, task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateTask(int projectId, int taskId, [FromBody] TaskUpdateRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _taskService.UpdateTaskAsync(taskId, request, projectId, userId);
            if (result)
                return NoContent();
            else
                return NotFound(new { message = "Task not found or could not be updated." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(int projectId, int taskId)
    {
        try
        {
            var userId = GetUserId();
            var result = await _taskService.DeleteTaskAsync(taskId, projectId, userId);
            if (result)
                return NoContent();
            else
                return NotFound(new { message = "Task not found or could not be deleted." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}