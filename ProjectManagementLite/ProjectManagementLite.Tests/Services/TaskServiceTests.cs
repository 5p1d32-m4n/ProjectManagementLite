// ProjectManagementLite.Tests/Services/TaskServiceTests.cs
using Xunit;
using Moq;
using ProjectManagementLite.Services;
using ProjectManagementLite.Repositories;
using ProjectManagementLite.Models;
using ProjectManagementLite.DTOs.TaskItem;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ProjectManagementLite.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _taskService = new TaskService(_taskRepositoryMock.Object, _projectRepositoryMock.Object);
        }

        [Fact]
        public async Task GetTasksAsync_ShouldReturnTasks_ForGivenProjectAndUser()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;

            var project = new Project { Id = projectId, Name = "Project 1", UserId = userId };
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, ProjectId = projectId, Title = "Task 1", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) },
                new TaskItem { Id = 2, ProjectId = projectId, Title = "Task 2", Status = "Completed", DueDate = DateTime.UtcNow.AddDays(2) }
            };

            _projectRepositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync(project);

            _taskRepositoryMock.Setup(repo => repo.GetTasksByProjectIdAsync(projectId))
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetTasksAsync(projectId, userId);

            // Assert
            Assert.Equal(2, ((List<TaskItem>)result).Count);
            _projectRepositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _taskRepositoryMock.Verify(repo => repo.GetTasksByProjectIdAsync(projectId), Times.Once);
        }

        [Fact]
        public async Task GetTasksAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync((Project)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _taskService.GetTasksAsync(projectId, userId));
            Assert.Equal("Project not found.", exception.Message);
            _projectRepositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _taskRepositoryMock.Verify(repo => repo.GetTasksByProjectIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            int taskId = 1;

            var project = new Project { Id = projectId, Name = "Project 1", UserId = userId };
            var task = new TaskItem { Id = taskId, ProjectId = projectId, Title = "Task 1", Status = "Pending", DueDate = DateTime.UtcNow.AddDays(1) };

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync(project);

            _taskRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId, projectId))
                .ReturnsAsync(task);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId, projectId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
            _project_repositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.GetTaskByIdAsync(taskId, projectId), Times.Once);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            int taskId = 1;

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync((Project)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _taskService.GetTaskByIdAsync(taskId, projectId, userId));
            Assert.Equal("Project not found.", exception.Message);
            _project_repositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.GetTaskByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldThrowException_WhenTaskDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            int taskId = 1;

            var project = new Project { Id = projectId, Name = "Project 1", UserId = userId };

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync(project);

            _task_repositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId, projectId))
                .ReturnsAsync((TaskItem)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _taskService.GetTaskByIdAsync(taskId, projectId, userId));
            Assert.Equal("Task not found.", exception.Message);
            _project_repositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.GetTaskByIdAsync(taskId, projectId), Times.Once);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldCreateAndReturnTask_WhenProjectExists()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            var createRequest = new TaskCreateRequest
            {
                Title = "New Task",
                Description = "Task Description",
                Status = "Pending",
                DueDate = DateTime.UtcNow.AddDays(3)
            };

            var project = new Project { Id = projectId, Name = "Project 1", UserId = userId };
            var taskId = 1;

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync(project);

            _task_repositoryMock.Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(taskId);

            // Act
            var result = await _task_service.CreateTaskAsync(projectId, createRequest, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
            Assert.Equal(createRequest.Title, result.Title);
            _project_repositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.CreateTaskAsync(It.Is<TaskItem>(t => t.Title == createRequest.Title && t.ProjectId == projectId)), Times.Once);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            var createRequest = new TaskCreateRequest
            {
                Title = "New Task",
                Description = "Task Description",
                Status = "Pending",
                DueDate = DateTime.UtcNow.AddDays(3)
            };

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync((Project)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _taskService.CreateTaskAsync(projectId, createRequest, userId));
            Assert.Equal("Project not found.", exception.Message);
            _project_repositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldUpdateTask_WhenTaskExists()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            int taskId = 1;
            var updateRequest = new TaskUpdateRequest
            {
                Title = "Updated Task",
                Description = "Updated Description",
                Status = "In Progress",
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            var project = new Project { Id = projectId, Name = "Project 1", UserId = userId };
            var existingTask = new TaskItem
            {
                Id = taskId,
                ProjectId = projectId,
                Title = "Old Task",
                Description = "Old Description",
                Status = "Pending",
                DueDate = DateTime.UtcNow.AddDays(2)
            };

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync(project);

            _task_repositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId, projectId))
                .ReturnsAsync(existingTask);

            _task_repositoryMock.Setup(repo => repo.UpdateTaskAsync(existingTask))
                .ReturnsAsync(true);

            // Act
            var result = await _task_service.UpdateTaskAsync(taskId, updateRequest, projectId, userId);

            // Assert
            Assert.True(result);
            _project_repositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.GetTaskByIdAsync(taskId, projectId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.UpdateTaskAsync(It.Is<TaskItem>(t => t.Title == updateRequest.Title && t.Status == updateRequest.Status)), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            int taskId = 1;
            var updateRequest = new TaskUpdateRequest
            {
                Title = "Updated Task",
                Description = "Updated Description",
                Status = "In Progress",
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync((Project)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _task_service.UpdateTaskAsync(taskId, updateRequest, projectId, userId));
            Assert.Equal("Project not found.", exception.Message);
            _project_repositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.GetTaskByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _task_repositoryMock.Verify(repo => repo.UpdateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldThrowException_WhenTaskDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            int taskId = 1;
            var updateRequest = new TaskUpdateRequest
            {
                Title = "Updated Task",
                Description = "Updated Description",
                Status = "In Progress",
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            var project = new Project { Id = projectId, Name = "Project 1", UserId = userId };

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync(project);

            _task_repositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId, projectId))
                .ReturnsAsync((TaskItem)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _task_service.UpdateTaskAsync(taskId, updateRequest, projectId, userId));
            Assert.Equal("Task not found.", exception.Message);
            _project_repositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.GetTaskByIdAsync(taskId, projectId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.UpdateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldDeleteTask_WhenTaskExists()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            int taskId = 1;

            var project = new Project { Id = projectId, Name = "Project 1", UserId = userId };

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync(project);

            _task_repositoryMock.Setup(repo => repo.DeleteTaskAsync(taskId, projectId))
                .ReturnsAsync(true);

            // Act
            var result = await _task_service.DeleteTaskAsync(taskId, projectId, userId);

            // Assert
            Assert.True(result);
            _project_repositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.DeleteTaskAsync(taskId, projectId), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            int taskId = 1;

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync((Project)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _task_service.DeleteTaskAsync(taskId, projectId, userId));
            Assert.Equal("Project not found.", exception.Message);
            _project_repositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.DeleteTaskAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int projectId = 1;
            int taskId = 1;

            var project = new Project { Id = projectId, Name = "Project 1", UserId = userId };

            _project_repositoryMock.Setup(repo => repo.GetProjectByIdAsync(projectId, userId))
                .ReturnsAsync(project);

            _task_repositoryMock.Setup(repo => repo.DeleteTaskAsync(taskId, projectId))
                .ReturnsAsync(false);

            // Act
            var result = await _task_service.DeleteTaskAsync(taskId, projectId, userId);

            // Assert
            Assert.False(result);
            _project_repositoryMock.Verify(repo => repo.GetProjectByIdAsync(projectId, userId), Times.Once);
            _task_repositoryMock.Verify(repo => repo.DeleteTaskAsync(taskId, projectId), Times.Once);
        }
    }
}