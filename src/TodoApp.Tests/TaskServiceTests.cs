using Microsoft.EntityFrameworkCore;
using TodoApp.WPF.Core.Entities;
using TodoApp.WPF.Core.Enums;
using TodoApp.WPF.Core.Interfaces;
using TodoApp.WPF.Infrastructure.Data;
using TodoApp.WPF.Infrastructure.Repositories;
using TodoApp.WPF.Services;

namespace TodoApp.Tests;

public class TaskServiceTests
{
   private ITaskService GetTaskService()
   {
      var options = new DbContextOptionsBuilder<AppDbContext>()
         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
         .Options;

      var context = new AppDbContext(options);
      var taskRepository = new TaskRepository(context);
      return new TaskService(taskRepository);
   }

   [Fact]
   public async Task CreateTask_ValidTask_ReturnsCreatedTask()
   {
      // Arrange
      var service = GetTaskService();
      var task = new TodoTask
      {
         Title = "Test Task",
         Description = "Test Description",
         Priority = Priority.Medium,
         DueDate = DateTime.UtcNow.AddDays(1)
      };

      // Act
      var createdTask = await service.CreateTaskAsync(task);

      // Assert
      Assert.NotNull(createdTask);
      Assert.Equal("Test Task", createdTask.Title);
      Assert.True(createdTask.Id > 0);
   }

   [Fact]
   public async Task CreateTask_EmptyTitle_ThrowsArgumentException()
   {
      // Arrange
      var service = GetTaskService();
      var task = new TodoTask
      {
         Title = "",
         Description = "Test Description"
      };

      // Act & Assert
      await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTaskAsync(task));
   }

   [Fact]
   public async Task GetTaskById_ExistingId_ReturnsTask()
   {
      // Arrange
      var service = GetTaskService();
      var task = new TodoTask
      {
         Title = "Test Task",
         Description = "Test Description",
         Priority = Priority.Medium
      };
      var createdTask = await service.CreateTaskAsync(task);

      // Act
      var retrievedTask = await service.GetTaskByIdAsync(createdTask.Id);

      // Assert
      Assert.NotNull(retrievedTask);
      Assert.Equal(createdTask.Id, retrievedTask.Id);
      Assert.Equal("Test Task", retrievedTask.Title);
   }

   [Fact]
   public async Task ToggleTaskCompletion_TogglesCompletionStatus()
   {
      // Arrange
      var service = GetTaskService();
      var task = new TodoTask
      {
         Title = "Test Task",
         Priority = Priority.Medium
      };
      var createdTask = await service.CreateTaskAsync(task);

      // Act - Toggle to completed
      await service.ToggleTaskCompletionAsync(createdTask.Id);
      var taskAfterToggle1 = await service.GetTaskByIdAsync(createdTask.Id);

      // Assert - Should be completed
      Assert.True(taskAfterToggle1.IsCompleted);
      Assert.NotNull(taskAfterToggle1.CompletedAt);

      // Act - Toggle back to incomplete
      await service.ToggleTaskCompletionAsync(createdTask.Id);
      var taskAfterToggle2 = await service.GetTaskByIdAsync(createdTask.Id);

      // Assert - Should be incomplete again
      Assert.False(taskAfterToggle2.IsCompleted);
      Assert.Null(taskAfterToggle2.CompletedAt);
   }

   [Fact]
   public async Task DeleteTask_ExistingId_RemovesTask()
   {
      // Arrange
      var service = GetTaskService();
      var task = new TodoTask
      {
         Title = "Test Task",
         Priority = Priority.Medium
      };
      var createdTask = await service.CreateTaskAsync(task);

      // Act
      await service.DeleteTaskAsync(createdTask.Id);

      // Assert
      var deletedTask = await service.GetTaskByIdAsync(createdTask.Id);
      Assert.Null(deletedTask); // Task should be soft deleted
   }
}