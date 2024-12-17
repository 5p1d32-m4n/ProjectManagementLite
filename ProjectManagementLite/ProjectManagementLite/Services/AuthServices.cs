using ProjectManagementLite.Repositories;

namespace ProjectManagementLite.Services;

// Services/AuthService.cs
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using ProjectManagementLite.Models;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest registerRequest)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByUsernameAsync(registerRequest.Username);
        if (existingUser != null)
        {
            throw new Exception("User already exists.");
        }

        // Hash the password
        string passwordHash = BCrypt.HashPassword(registerRequest.Password);

        // Create user
        var user = new User
        {
            UserName = registerRequest.Username,
            PasswordHash = passwordHash,
            Email = registerRequest.Email
        };

        user.Id = await _userRepository.CreateUserAsync(user);

        // Generate JWT
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            Username = user.UserName
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userRepository.GetByUsernameAsync(loginRequest.Username);
        if (user == null || !BCrypt.Verify(loginRequest.Password, user.PasswordHash))
        {
            throw new Exception("Invalid username or password.");
        }

        // Generate JWT
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            Username = user.UserName
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings.GetValue<string>("SecretKey");
        var issuer = jwtSettings.GetValue<string>("Issuer");
        var audience = jwtSettings.GetValue<string>("Audience");
        var expiryInMinutes = jwtSettings.GetValue<int>("ExpiryInMinutes");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim("id", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}