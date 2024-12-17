using ProjectManagementLite.Models;
namespace ProjectManagementLite.Repositories;

// Repositories/IProjectRepository.cs
public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId);
    Task<Project?> GetProjectByIdAsync(int projectId, int userId);
    Task<int> CreateProjectAsync(Project project);
    Task<bool> UpdateProjectAsync(Project project);
    Task<bool> DeleteProjectAsync(int projectId, int userId);
}