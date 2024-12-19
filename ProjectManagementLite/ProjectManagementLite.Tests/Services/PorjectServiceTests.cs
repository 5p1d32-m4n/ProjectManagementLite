// ProjectManagementLite.Tests/Services/ProjectServiceTests.cs
using Xunit;
using Moq;
using ProjectManagementLite;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ProjectManagementLite.Tests.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectService = new ProjectService(_projectRepositoryMock.Object);
        }

        [Fact]
        public async Task GetProjectsAsync_ShouldReturnProjects_ForGivenUser()
        {
            // Arrange
            int userId = 1;
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Project 1", Description = "Description 1", UserId = userId },
                new Project { Id = 2, Name = "Project 2", Description = "Description 2", UserId = userId }
            };

            _projectRepositoryMock.Setup(repo => repo.GetProjectsByUserIdAsync(userId))
                .ReturnsAsync(projects);

            // Act
            var result = await _projectService.GetProjectsAsync(userId);

            // Assert
            Assert.Equal(2, ((List<Project>)result).Count);
            _projectRepositoryMock.Verify(repo => repo.GetProjectsByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetProjectByIdAsync_ShouldReturnProject_WhenProjectExists()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            var project = new Project { Id = projectId, Name = "Project 1", Description = "Description 1", UserId = userId };

            _projectRepositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync(project);

            // Act
            var result = await _projectService.GetProjectByIdAsync(projectId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectId, result.Id);
            _projectRepositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
        }

        [Fact]
        public async Task GetProjectByIdAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;

            _projectRepositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync((Project)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _projectService.GetProjectByIdAsync(projectId, userId));
            Assert.Equal("Project not found.", exception.Message);
            _projectRepositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
        }

        [Fact]
        public async Task CreateProjectAsync_ShouldCreateAndReturnProject()
        {
            // Arrange
            int userId = 1;
            var createRequest = new ProjectCreateRequest
            {
                Name = "New Project",
                Description = "New Project Description"
            };

            var project = new Project
            {
                Id = 1,
                Name = createRequest.Name,
                Description = createRequest.Description,
                UserId = userId
            };

            _projectRepositoryMock.Setup(repo => repo.CreateProjectAsync(It.IsAny<Project>()))
                .ReturnsAsync(project.Id);

            // Act
            var result = await _projectService.CreateProjectAsync(createRequest, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(project.Id, result.Id);
            Assert.Equal(createRequest.Name, result.Name);
            _projectRepositoryMock.Verify(repo => repo.CreateProjectAsync(It.Is<Project>(p => p.Name == createRequest.Name && p.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task UpdateProjectAsync_ShouldUpdateProject_WhenProjectExists()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            var updateRequest = new ProjectUpdateRequest
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };

            var existingProject = new Project
            {
                Id = projectId,
                Name = "Old Project",
                Description = "Old Description",
                UserId = userId
            };

            _projectRepositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync(existingProject);

            _projectRepositoryMock.Setup(repo => repo.UpdateProjectAsync(existingProject))
                .ReturnsAsync(true);

            // Act
            var result = await _projectService.UpdateProjectAsync(projectId, updateRequest, userId);

            // Assert
            Assert.True(result);
            _projectRepositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _projectRepositoryMock.Verify(repo => repo.UpdateProjectAsync(It.Is<Project>(p => p.Name == updateRequest.Name && p.Description == updateRequest.Description)), Times.Once);
        }

        [Fact]
        public async Task UpdateProjectAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            var updateRequest = new ProjectUpdateRequest
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };

            _projectRepositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync((Project)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _projectService.UpdateProjectAsync(projectId, updateRequest, userId));
            Assert.Equal("Project not found.", exception.Message);
            _projectRepositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _projectRepositoryMock.Verify(repo => repo.UpdateProjectAsync(It.IsAny<Project>()), Times.Never);
        }

        [Fact]
        public async Task DeleteProjectAsync_ShouldDeleteProject_WhenProjectExists()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;

            _projectRepositoryMock.Setup(repo => repo.DeleteProjectAsync(projectId, userId))
                .ReturnsAsync(true);

            // Act
            var result = await _projectService.DeleteProjectAsync(projectId, userId);

            // Assert
            Assert.True(result);
            _projectRepositoryMock.Verify(repo => repo.DeleteProjectAsync(projectId, userId), Times.Once);
        }

        [Fact]
        public async Task DeleteProjectAsync_ShouldReturnFalse_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;

            _projectRepositoryMock.Setup(repo => repo.DeleteProjectAsync(projectId, userId))
                .ReturnsAsync(false);

            // Act
            var result = await _projectService.DeleteProjectAsync(projectId, userId);

            // Assert
            Assert.False(result);
            _projectRepositoryMock.Verify(repo => repo.DeleteProjectAsync(projectId, userId), Times.Once);
        }
    }
}