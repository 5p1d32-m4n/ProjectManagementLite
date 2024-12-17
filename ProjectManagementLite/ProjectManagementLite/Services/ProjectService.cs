using ProjectManagementLite.Models;
using ProjectManagementLite.Repositories;

namespace ProjectManagementLite.Services;

// Services/ProjectService.cs
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<Project>> GetProjectsAsync(int userId)
    {
        return await _projectRepository.GetProjectsByUserIdAsync(userId);
    }

    public async Task<Project> GetProjectByIdAsync(int projectId, int userId)
    {
        var project = await _projectRepository.GetProjectByIdAsync(projectId, userId);
        if (project == null)
            throw new Exception("Project not found.");
        return project;
    }

    public async Task<Project> CreateProjectAsync(ProjectCreateRequest request, int userId)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            UserId = userId
        };
        project.Id = await _projectRepository.CreateProjectAsync(project);
        return project;
    }

    public async Task<bool> UpdateProjectAsync(int projectId, ProjectUpdateRequest request, int userId)
    {
        var project = await _projectRepository.GetProjectByIdAsync(projectId, userId);
        if (project == null)
            throw new Exception("Project not found.");

        project.Name = request.Name;
        project.Description = request.Description;

        return await _projectRepository.UpdateProjectAsync(project);
    }

    public async Task<bool> DeleteProjectAsync(int projectId, int userId)
    {
        return await _projectRepository.DeleteProjectAsync(projectId, userId);
    }
}