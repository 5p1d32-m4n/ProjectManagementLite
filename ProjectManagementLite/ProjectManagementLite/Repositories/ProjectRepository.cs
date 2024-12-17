using ProjectManagementLite.Models;
namespace ProjectManagementLite.Repositories;

// Repositories/ProjectRepository.cs
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

public class ProjectRepository : IProjectRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    
    public ProjectRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }
    
    private IDbConnection Connection => new SqlConnection(_connectionString);
    
    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId)
    {
        var sql = "SELECT * FROM Projects WHERE UserId = @UserId";
        using var db = Connection;
        return await db.QueryAsync<Project>(sql, new { UserId = userId });
    }
    
    public async Task<Project?> GetProjectByIdAsync(int projectId, int userId)
    {
        var sql = "SELECT * FROM Projects WHERE Id = @ProjectId AND UserId = @UserId";
        using var db = Connection;
        return await db.QueryFirstOrDefaultAsync<Project>(sql, new { ProjectId = projectId, UserId = userId });
    }
    
    public async Task<int> CreateProjectAsync(Project project)
    {
        var sql = @"INSERT INTO Projects (Name, Description, CreatedDate, UserId)
                    VALUES (@Name, @Description, @CreatedDate, @UserId);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
        using var db = Connection;
        return await db.QuerySingleAsync<int>(sql, project);
    }
    
    public async Task<bool> UpdateProjectAsync(Project project)
    {
        var sql = @"UPDATE Projects 
                    SET Name = @Name, Description = @Description 
                    WHERE Id = @Id AND UserId = @UserId";
        using var db = Connection;
        var affectedRows = await db.ExecuteAsync(sql, project);
        return affectedRows > 0;
    }
    
    public async Task<bool> DeleteProjectAsync(int projectId, int userId)
    {
        var sql = "DELETE FROM Projects WHERE Id = @ProjectId AND UserId = @UserId";
        using var db = Connection;
        var affectedRows = await db.ExecuteAsync(sql, new { ProjectId = projectId, UserId = userId });
        return affectedRows > 0;
    }
}