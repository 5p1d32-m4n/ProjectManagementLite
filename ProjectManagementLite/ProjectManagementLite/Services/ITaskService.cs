using ProjectManagementLite.Models;

namespace ProjectManagementLite.Services;

// Services/ITaskService.cs
public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetTasksAsync(int projectId, int userId);
    Task<TaskItem> GetTaskByIdAsync(int taskId, int projectId, int userId);
    Task<TaskItem> CreateTaskAsync(int projectId, TaskCreateRequest request, int userId);
    Task<bool> UpdateTaskAsync(int taskId, TaskUpdateRequest request, int projectId, int userId);
    Task<bool> DeleteTaskAsync(int taskId, int projectId, int userId);
}