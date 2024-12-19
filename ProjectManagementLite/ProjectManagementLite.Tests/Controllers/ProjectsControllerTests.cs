// ProjectManagementLite.Tests/Controllers/ProjectsControllerTests.cs
using Xunit;
using Moq;
using ProjectManagementLite.Controllers;
using ProjectManagementLite.Services;
using ProjectManagementLite.DTOs.Project;
using ProjectManagementLite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;

namespace ProjectManagementLite.Tests.Controllers
{
    public class ProjectsControllerTests
    {
        private readonly Mock<IProjectService> _projectServiceMock;
        private readonly ProjectsController _controller;

        public ProjectsControllerTests()
        {
            _projectServiceMock = new Mock<IProjectService>();
            _controller = new ProjectsController(_projectServiceMock.Object);

            // Setup user claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("id", "1"),
                new Claim(ClaimTypes.Name, "john_doe")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetProjects_ShouldReturnOkResult_WithListOfProjects()
        {
            // Arrange
            int userId = 1;
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Project 1", Description = "Description 1", UserId = userId },
                new Project { Id = 2, Name = "Project 2", Description = "Description 2", UserId = userId }
            };

            _projectServiceMock.Setup(service => service.GetProjectsAsync(userId))
                .ReturnsAsync(projects);

            // Act
            var result = await _controller.GetProjects();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProjects = Assert.IsAssignableFrom<IEnumerable<Project>>(okResult.Value);
            Assert.Equal(2, ((List<Project>)returnProjects).Count);
            _projectServiceMock.Verify(service => service.GetProjectsAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetProject_ShouldReturnOkResult_WithProject()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            var project = new Project { Id = projectId, Name = "Project 1", Description = "Description 1", UserId = userId };

            _projectServiceMock.Setup(service => service.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync(project);

            // Act
            var result = await _controller.GetProject(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProject = Assert.IsType<Project>(okResult.Value);
            Assert.Equal(projectId, returnProject.Id);
            _projectServiceMock.Verify(service => service.GetProjectByIdAsync(projectId, userId), Times.Once);
        }

        [Fact]
        public async Task GetProject_ShouldReturnNotFound_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;

            _projectServiceMock.Setup(service => service.GetProjectByIdAsync(projectId, userId))
                .ThrowsAsync(new Exception("Project not found."));

            // Act
            var result = await _controller.GetProject(projectId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<Dictionary<string, string>>(notFoundResult.Value);
            Assert.Equal("Project not found.", message["message"]);
            _projectServiceMock.Verify(service => service.GetProjectByIdAsync(projectId, userId), Times.Once);
        }

        [Fact]
        public async Task CreateProject_ShouldReturnCreatedAtAction_WithCreatedProject()
        {
            // Arrange
            int userId = 1;
            var createRequest = new ProjectCreateRequest
            {
                Name = "New Project",
                Description = "New Project Description"
            };

            var createdProject = new Project
            {
                Id = 1,
                Name = createRequest.Name,
                Description = createRequest.Description,
                UserId = userId
            };

            _projectServiceMock.Setup(service => service.CreateProjectAsync(createRequest, userId))
                .ReturnsAsync(createdProject);

            // Act
            var result = await _controller.CreateProject(createRequest);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnProject = Assert.IsType<Project>(createdAtActionResult.Value);
            Assert.Equal(createdProject.Id, returnProject.Id);
            _projectServiceMock.Verify(service => service.CreateProjectAsync(createRequest, userId), Times.Once);
        }

        [Fact]
        public async Task CreateProject_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            int userId = 1;
            var createRequest = new ProjectCreateRequest
            {
                Name = "New Project",
                Description = "New Project Description"
            };

            _projectServiceMock.Setup(service => service.CreateProjectAsync(createRequest, userId))
                .ThrowsAsync(new Exception("Invalid project data."));

            // Act
            var result = await _controller.CreateProject(createRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var message = Assert.IsType<Dictionary<string, string>>(badRequestResult.Value);
            Assert.Equal("Invalid project data.", message["message"]);
            _projectServiceMock.Verify(service => service.CreateProjectAsync(createRequest, userId), Times.Once);
        }

        [Fact]
        public async Task UpdateProject_ShouldReturnNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            var updateRequest = new ProjectUpdateRequest
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };

            _projectServiceMock.Setup(service => service.UpdateProjectAsync(projectId, updateRequest, userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProject(projectId, updateRequest);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _projectServiceMock.Verify(service => service.UpdateProjectAsync(projectId, updateRequest, userId), Times.Once);
        }

        [Fact]
        public async Task UpdateProject_ShouldReturnNotFound_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            var updateRequest = new ProjectUpdateRequest
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };

            _projectServiceMock.Setup(service => service.UpdateProjectAsync(projectId, updateRequest, userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateProject(projectId, updateRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<Dictionary<string, string>>(notFoundResult.Value);
            Assert.Equal("Project not found or could not be updated.", message["message"]);
            _projectServiceMock.Verify(service => service.UpdateProjectAsync(projectId, updateRequest, userId), Times.Once);
        }

        [Fact]
        public async Task UpdateProject_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            var updateRequest = new ProjectUpdateRequest
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };

            _project_service_mock.Setup(service => service.UpdateProjectAsync(projectId, updateRequest, userId))
                .ThrowsAsync(new Exception("Invalid update data."));

            // Act
            var result = await _controller.UpdateProject(projectId, updateRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var message = Assert.IsType<Dictionary<string, string>>(badRequestResult.Value);
            Assert.Equal("Invalid update data.", message["message"]);
            _projectServiceMock.Verify(service => service.UpdateProjectAsync(projectId, updateRequest, userId), Times.Once);
        }

        [Fact]
        public async Task DeleteProject_ShouldReturnNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;

            _projectServiceMock.Setup(service => service.DeleteProjectAsync(projectId, userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteProject(projectId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _projectServiceMock.Verify(service => service.DeleteProjectAsync(projectId, userId), Times.Once);
        }

        [Fact]
        public async Task DeleteProject_ShouldReturnNotFound_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;

            _projectServiceMock.Setup(service => service.DeleteProjectAsync(projectId, userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteProject(projectId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<Dictionary<string, string>>(notFoundResult.Value);
            Assert.Equal("Project not found or could not be deleted.", message["message"]);
            _projectServiceMock.Verify(service => service.DeleteProjectAsync(projectId, userId), Times.Once);
        }

        [Fact]
        public async Task DeleteProject_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;

            _projectServiceMock.Setup(service => service.DeleteProjectAsync(projectId, userId))
                .ThrowsAsync(new Exception("Deletion failed."));

            // Act
            var result = await _controller.DeleteProject(projectId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var message = Assert.IsType<Dictionary<string, string>>(badRequestResult.Value);
            Assert.Equal("Deletion failed.", message["message"]);
            _project_service_mock.Verify(service => service.DeleteProjectAsync(projectId, userId), Times.Once);
        }
    }
}