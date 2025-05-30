using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReservationSystemWebAPI.Tests.Integration
{
    public class UserServiceIntegrationTests
    {
        private readonly IUserService _userService;
        private readonly ReservationDbContext _context;

        public UserServiceIntegrationTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ReservationDbContext>(options =>
                options.UseSqlServer("Server=hildur.ucn.dk;Database=DMA-CSD-S235_10632110;User Id=DMA-CSD-S235_10632110;Password=Password1!;TrustServerCertificate=true"));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            var provider = services.BuildServiceProvider();
            _userService = provider.GetRequiredService<IUserService>();
            _context = provider.GetRequiredService<ReservationDbContext>();
        }

        [Fact]
        public async Task AddAsync_ValidUser_WorksCorrectly()
        {
            var user = new User
            {
                Name = "Integration Add",
                Email = $"integration_add_{Guid.NewGuid()}@wexo.dk",
                Password = "test1234",
                Role = "Employee"
            };

            int userId = await _userService.AddAsync(user);
            var result = await _userService.GetByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);

            // Cleanup: Delete requires rowVersion now
            await _userService.DeleteAsync(userId, result.RowVersionAsString());
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsUser()
        {
            var user = new User
            {
                Name = "Integration Get",
                Email = $"integration_get_{Guid.NewGuid()}@wexo.dk",
                Password = "test1234",
                Role = "Employee"
            };

            int id = await _userService.AddAsync(user);
            var result = await _userService.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);

            // Cleanup
            await _userService.DeleteAsync(id, result.RowVersionAsString());
        }

        [Fact]
        public async Task UpdateAsync_ExistingUser_UpdatesCorrectly()
        {
            // Arrange – create user
            var user = new User
            {
                Name = "Integration Update",
                Email = $"integration_update_{Guid.NewGuid()}@wexo.dk",
                Password = "1234",
                Role = "Employee"
            };

            int id = await _userService.AddAsync(user);

            // Retrieve existing user with concurrency token
            var existing = await _userService.GetByIdAsync(id);

            // Modify properties
            existing.Role = "Admin";
            existing.Password = "updated1234";

            // Act – update (pass the concurrency token)
            int updatedId = await _userService.UpdateAsync(existing);

            // Assert
            var result = await _userService.GetByIdAsync(id);
            Assert.Equal("Admin", result.Role);

            // Cleanup - delete with concurrency token
            await _userService.DeleteAsync(id, result.RowVersionAsString());
        }

        [Fact]
        public async Task DeleteAsync_ExistingUser_DeletesCorrectly()
        {
            var user = new User
            {
                Name = "Integration Delete",
                Email = $"integration_delete_{Guid.NewGuid()}@wexo.dk",
                Password = "test1234",
                Role = "Employee"
            };

            int id = await _userService.AddAsync(user);

            var userFromDb = await _userService.GetByIdAsync(id);

            int deletedId = await _userService.DeleteAsync(id, userFromDb.RowVersionAsString());
            Assert.Equal(id, deletedId);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.GetByIdAsync(id));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsUsers()
        {
            var user = new User
            {
                Email = $"integration_all_{Guid.NewGuid()}@wexo.dk",
                Password = "test1234",
                Role = "Employee",
                Name = "Test All"
            };

            int id = await _userService.AddAsync(user);

            var users = await _userService.GetAllAsync();
            Assert.Contains(users, u => u.Id == id);

            // Get the user with concurrency token for delete
            var userFromDb = await _userService.GetByIdAsync(id);
            await _userService.DeleteAsync(id, userFromDb.RowVersionAsString());
        }
    }

    // Extension method to get Base64 string of concurrency token
    public static class UserExtensions
    {
        public static string RowVersionAsString(this User user)
        {
            return Convert.ToBase64String(user.RowVersion ?? Array.Empty<byte>());
        }
    }
}