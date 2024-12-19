// ProjectManagementLite.Tests/Services/AuthServiceTests.cs
using Xunit;
using Moq;
using ProjectManagementLite;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ProjectManagementLite.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();

            // Setup JWT settings in configuration mock
            var inMemorySettings = new Dictionary<string, string> {
                {"JwtSettings:SecretKey", "YourSuperSecretKey12345"},
                {"JwtSettings:Issuer", "ProjectManagementLite"},
                {"JwtSettings:Audience", "ProjectManagementLiteUsers"},
                {"JwtSettings:ExpiryInMinutes", "60"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _configurationMock.Setup(c => c.GetSection("JwtSettings")).Returns(configuration.GetSection("JwtSettings"));

            _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenUserDoesNotExist()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Username = "john_doe",
                Password = "SecureP@ssw0rd",
                Email = "john@example.com"
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(registerRequest.Username))
                .ReturnsAsync((User)null);

            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(1); // Simulate user ID

            // Act
            var result = await _authService.RegisterAsync(registerRequest);

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.Token));
            Assert.Equal(registerRequest.Username, result.Username);
            _userRepositoryMock.Verify(repo => repo.GetByUsernameAsync(registerRequest.Username), Times.Once);
            _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.Is<User>(u => u.Username == registerRequest.Username)), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenUserAlreadyExists()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Username = "john_doe",
                Password = "SecureP@ssw0rd",
                Email = "john@example.com"
            };

            var existingUser = new User
            {
                Id = 1,
                Username = "john_doe",
                PasswordHash = "hashedpassword",
                Email = "john@example.com"
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(registerRequest.Username))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(registerRequest));
            Assert.Equal("User already exists.", exception.Message);
            _userRepositoryMock.Verify(repo => repo.GetByUsernameAsync(registerRequest.Username), Times.Once);
            _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "john_doe",
                Password = "SecureP@ssw0rd"
            };

            var user = new User
            {
                Id = 1,
                Username = "john_doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginRequest.Password),
                Email = "john@example.com"
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(loginRequest.Username))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.Token));
            Assert.Equal(user.Username, result.Username);
            _userRepositoryMock.Verify(repo => repo.GetByUsernameAsync(loginRequest.Username), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "nonexistent_user",
                Password = "SecureP@ssw0rd"
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(loginRequest.Username))
                .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(loginRequest));
            Assert.Equal("Invalid username or password.", exception.Message);
            _userRepositoryMock.Verify(repo => repo.GetByUsernameAsync(loginRequest.Username), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenPasswordIsInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "john_doe",
                Password = "WrongPassword"
            };

            var user = new User
            {
                Id = 1,
                Username = "john_doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SecureP@ssw0rd"),
                Email = "john@example.com"
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(loginRequest.Username))
                .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(loginRequest));
            Assert.Equal("Invalid username or password.", exception.Message);
            _userRepositoryMock.Verify(repo => repo.GetByUsernameAsync(loginRequest.Username), Times.Once);
        }
    }
}