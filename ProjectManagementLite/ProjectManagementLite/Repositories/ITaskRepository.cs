using ProjectManagementLite.Models;
namespace ProjectManagementLite.Repositories;
// Repositories/ITaskRepository.cs
public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(int projectId);
    Task<TaskItem?> GetTaskByIdAsync(int taskId, int projectId);
    Task<int> CreateTaskAsync(TaskItem task);
    Task<bool> UpdateTaskAsync(TaskItem task);
    Task<bool> DeleteTaskAsync(int taskId, int projectId);
}