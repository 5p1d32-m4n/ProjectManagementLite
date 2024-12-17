using ProjectManagementLite.Models;
namespace ProjectManagementLite.Services;

// Services/IAuthService.cs
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest);
    Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
}