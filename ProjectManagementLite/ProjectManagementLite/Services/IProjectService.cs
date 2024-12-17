namespace ProjectManagementLite.Services;
using ProjectManagementLite.Models;

// Services/IProjectService.cs
public interface IProjectService
{
    Task<IEnumerable<Project>> GetProjectsAsync(int userId);
    Task<Project> GetProjectByIdAsync(int projectId, int userId);
    Task<Project> CreateProjectAsync(ProjectCreateRequest request, int userId);
    Task<bool> UpdateProjectAsync(int projectId, ProjectUpdateRequest request, int userId);
    Task<bool> DeleteProjectAsync(int projectId, int userId);
}