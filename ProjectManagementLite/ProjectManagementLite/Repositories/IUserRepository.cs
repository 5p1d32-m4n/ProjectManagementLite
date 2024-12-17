// Repositories/IUserRepository.cs

using ProjectManagementLite.Models;

namespace ProjectManagementLite.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> GetByIdAsync(int id);
    Task<int> CreateUserAsync(User user);
}