using ProjectManagementLite.Models;

namespace ProjectManagementLite.Repositories;
// Repositories/TaskRepository.cs
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

public class TaskRepository : ITaskRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    
    public TaskRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }
    
    private IDbConnection Connection => new SqlConnection(_connectionString);
    
    public async Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(int projectId)
    {
        var sql = "SELECT * FROM Tasks WHERE ProjectId = @ProjectId";
        using var db = Connection;
        return await db.QueryAsync<TaskItem>(sql, new { ProjectId = projectId });
    }

    Task<TaskItem?> ITaskRepository.GetTaskByIdAsync(int taskId, int projectId)
    {
        return GetTaskByIdAsync(taskId, projectId);
    }

    Task<int> ITaskRepository.CreateTaskAsync(TaskItem task)
    {
        return CreateTaskAsync(task);
    }

    Task<bool> ITaskRepository.UpdateTaskAsync(TaskItem task)
    {
        return UpdateTaskAsync(task);
    }

    Task<IEnumerable<TaskItem>> ITaskRepository.GetTasksByProjectIdAsync(int projectId)
    {
        return GetTasksByProjectIdAsync(projectId);
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int taskId, int projectId)
    {
        var sql = "SELECT * FROM Tasks WHERE Id = @TaskId AND ProjectId = @ProjectId";
        using var db = Connection;
        return await db.QueryFirstOrDefaultAsync<TaskItem>(sql, new { TaskId = taskId, ProjectId = projectId });
    }
    
    public async Task<int> CreateTaskAsync(TaskItem task)
    {
        var sql = @"INSERT INTO Tasks (ProjectId, Title, Description, Status, DueDate)
                    VALUES (@ProjectId, @Title, @Description, @Status, @DueDate);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
        using var db = Connection;
        return await db.QuerySingleAsync<int>(sql, task);
    }
    
    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        var sql = @"UPDATE Tasks 
                    SET Title = @Title, Description = @Description, Status = @Status, DueDate = @DueDate 
                    WHERE Id = @Id AND ProjectId = @ProjectId";
        using var db = Connection;
        var affectedRows = await db.ExecuteAsync(sql, task);
        return affectedRows > 0;
    }
    
    public async Task<bool> DeleteTaskAsync(int taskId, int projectId)
    {
        var sql = "DELETE FROM Tasks WHERE Id = @TaskId AND ProjectId = @ProjectId";
        using var db = Connection;
        var affectedRows = await db.ExecuteAsync(sql, new { TaskId = taskId, ProjectId = projectId });
        return affectedRows > 0;
    }
}