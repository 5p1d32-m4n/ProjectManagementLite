// Controllers/ProjectsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProjectManagementLite.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    
    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    
    // Helper to get User ID from JWT
    private int GetUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : throw new Exception("User ID not found in token.");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        try
        {
            var userId = GetUserId();
            var projects = await _projectService.GetProjectsAsync(userId);
            return Ok(projects);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        try
        {
            var userId = GetUserId();
            var project = await _projectService.GetProjectByIdAsync(id, userId);
            return Ok(project);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectCreateRequest request)
    {
        try
        {
            var userId = GetUserId();
            var project = await _projectService.CreateProjectAsync(request, userId);
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectUpdateRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _projectService.UpdateProjectAsync(id, request, userId);
            if (result)
                return NoContent();
            else
                return NotFound(new { message = "Project not found or could not be updated." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            var userId = GetUserId();
            var result = await _projectService.DeleteProjectAsync(id, userId);
            if (result)
                return NoContent();
            else
                return NotFound(new { message = "Project not found or could not be deleted." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}