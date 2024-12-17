// Repositories/UserRepository.cs

using System.Data;
using System.Data.SqlClient;
using Dapper;
using ProjectManagementLite.Models;

namespace ProjectManagementLite.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    
    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }
    
    private IDbConnection Connection => new SqlConnection(_connectionString);
    
    public async Task<User?> GetByUsernameAsync(string username)
    {
        var sql = "SELECT * FROM Users WHERE Username = @Username";
        using var db = Connection;
        return await db.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
    }
    
    public async Task<User> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Users WHERE Id = @Id";
        using var db = Connection;
        return await db.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }
    
    public async Task<int> CreateUserAsync(User user)
    {
        var sql = @"INSERT INTO Users (Username, PasswordHash, Email, CreatedAt)
                    VALUES (@Username, @PasswordHash, @Email, @CreatedAt);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
        using var db = Connection;
        return await db.QuerySingleAsync<int>(sql, user);
    }
}