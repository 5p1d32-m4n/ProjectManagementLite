using ProjectManagementLite.Models;
using ProjectManagementLite.Repositories;

namespace ProjectManagementLite.Services;

// Services/TaskService.cs
public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<TaskItem>> GetTasksAsync(int projectId, int userId)
    {
        // Verify project ownership
        var project = await _projectRepository.GetProjectByIdAsync(projectId, userId);
        if (project == null)
            throw new Exception("Project not found.");

        return await _taskRepository.GetTasksByProjectIdAsync(projectId);
    }

    public async Task<TaskItem> GetTaskByIdAsync(int taskId, int projectId, int userId)
    {
        // Verify project ownership
        var project = await _projectRepository.GetProjectByIdAsync(projectId, userId);
        if (project == null)
            throw new Exception("Project not found.");

        var task = await _taskRepository.GetTaskByIdAsync(taskId, projectId);
        if (task == null)
            throw new Exception("Task not found.");

        return task;
    }

    public async Task<TaskItem> CreateTaskAsync(int projectId, TaskCreateRequest request, int userId)
    {
        // Verify project ownership
        var project = await _projectRepository.GetProjectByIdAsync(projectId, userId);
        if (project == null)
            throw new Exception("Project not found.");

        var task = new TaskItem
        {
            ProjectId = projectId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            DueDate = request.DueDate
        };

        task.Id = await _taskRepository.CreateTaskAsync(task);
        return task;
    }

    public async Task<bool> UpdateTaskAsync(int taskId, TaskUpdateRequest request, int projectId, int userId)
    {
        // Verify project ownership
        var project = await _projectRepository.GetProjectByIdAsync(projectId, userId);
        if (project == null)
            throw new Exception("Project not found.");

        var task = await _taskRepository.GetTaskByIdAsync(taskId, projectId);
        if (task == null)
            throw new Exception("Task not found.");

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.DueDate = request.DueDate;

        return await _taskRepository.UpdateTaskAsync(task);
    }

    public async Task<bool> DeleteTaskAsync(int taskId, int projectId, int userId)
    {
        // Verify project ownership
        var project = await _projectRepository.GetProjectByIdAsync(projectId, userId);
        if (project == null)
            throw new Exception("Project not found.");

        return await _taskRepository.DeleteTaskAsync(taskId, projectId);
    }
}