using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ReservationSystemWebAPI.Tests.Integration
{
    /// <summary>
    /// Integration tests for <see cref="UserService"/> using real database connections.
    /// Covers full lifecycle operations: Create, Read, Update, and Delete.
    /// </summary>
    public class UserServiceIntegrationTests
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes the test with actual database connection and dependency injection.
        /// </summary>
        public UserServiceIntegrationTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ReservationDbContext>(options =>
                options.UseSqlServer("Server=hildur.ucn.dk;Database=DMA-CSD-S235_10632110;User Id=DMA-CSD-S235_10632110;Password=Password1!;TrustServerCertificate=true"));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            var provider = services.BuildServiceProvider();
            _userService = provider.GetRequiredService<IUserService>();
        }

        /// <summary>
        /// Verifies that a new user can be added, retrieved, and deleted correctly.
        /// </summary>
        [Fact]
        public async Task AddAsync_ValidUser_WorksCorrectly()
        {
            var email = $"integration_add_{Guid.NewGuid()}@wexo.dk";
            var user = new User
            {
                Name = "Integration Add",
                Email = email,
                Password = "test1234",
                Role = "Employee"
            };

            var affectedRows = await _userService.AddAsync(user);
            Assert.Equal(1, affectedRows);

            var createdUser = (await _userService.GetAllAsync()).FirstOrDefault(u => u.Email == email);
            Assert.NotNull(createdUser);

            var fresh = await _userService.GetByIdAsync(createdUser.Id);
            await _userService.DeleteAsync(fresh.Id, fresh.RowVersionAsString());
        }

        /// <summary>
        /// Ensures a user can be retrieved by a valid ID.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsUser()
        {
            var email = $"integration_get_{Guid.NewGuid()}@wexo.dk";
            var user = new User
            {
                Name = "Integration Get",
                Email = email,
                Password = "test1234",
                Role = "Employee"
            };

            await _userService.AddAsync(user);
            var createdUser = (await _userService.GetAllAsync()).FirstOrDefault(u => u.Email == email);
            Assert.NotNull(createdUser);

            var fetched = await _userService.GetByIdAsync(createdUser.Id);
            Assert.NotNull(fetched);
            Assert.Equal(email, fetched.Email);

            await _userService.DeleteAsync(fetched.Id, fetched.RowVersionAsString());
        }

        /// <summary>
        /// Verifies that an existing user's properties can be updated successfully.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ExistingUser_UpdatesCorrectly()
        {
            var email = $"integration_update_{Guid.NewGuid()}@wexo.dk";
            var user = new User
            {
                Name = "Integration Update",
                Email = email,
                Password = "1234",
                Role = "Employee"
            };

            await _userService.AddAsync(user);
            var createdUser = (await _userService.GetAllAsync()).FirstOrDefault(u => u.Email == email);
            Assert.NotNull(createdUser);

            var fresh = await _userService.GetByIdAsync(createdUser.Id);
            fresh.Role = "Admin";
            fresh.Password = "updated1234";

            var affectedRows = await _userService.UpdateAsync(fresh);
            Assert.Equal(1, affectedRows);

            var result = await _userService.GetByIdAsync(createdUser.Id);
            Assert.Equal("Admin", result.Role);
            Assert.Equal(email, result.Email);
            Assert.NotEqual("1234", result.Password);

            await _userService.DeleteAsync(result.Id, result.RowVersionAsString());
        }

        /// <summary>
        /// Tests whether a user can be deleted successfully and ensures they no longer exist.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ExistingUser_DeletesCorrectly()
        {
            var email = $"integration_delete_{Guid.NewGuid()}@wexo.dk";
            var user = new User
            {
                Name = "Integration Delete",
                Email = email,
                Password = "test1234",
                Role = "Employee"
            };

            await _userService.AddAsync(user);

            // Find user by email
            var fromList = (await _userService.GetAllAsync()).FirstOrDefault(u => u.Email == email);
            Assert.NotNull(fromList);

            // Get latest version from DB
            var fresh = await _userService.GetByIdAsync(fromList.Id);
            Assert.NotNull(fresh);
            Assert.NotNull(fresh.RowVersion);

            // Delete with correct RowVersion
            var affectedRows = await _userService.DeleteAsync(fresh.Id, fresh.RowVersionAsString());

            Assert.Equal(1, affectedRows);

            // Ensure user no longer exists
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.GetByIdAsync(fresh.Id));
        }

        /// <summary>
        /// Ensures that GetAllAsync returns the list of users, including newly created ones.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ReturnsUsers()
        {
            var email = $"integration_all_{Guid.NewGuid()}@wexo.dk";
            var user = new User
            {
                Name = "Integration All",
                Email = email,
                Password = "test1234",
                Role = "Employee"
            };

            await _userService.AddAsync(user);

            var users = await _userService.GetAllAsync();
            var match = users.FirstOrDefault(u => u.Email == email);

            Assert.NotNull(match);
            Assert.Equal(email, match.Email);

            var fresh = await _userService.GetByIdAsync(match.Id);
            await _userService.DeleteAsync(fresh.Id, fresh.RowVersionAsString());
        }
    }

    /// <summary>
    /// Extension method for converting RowVersion to Base64 string.
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Converts a user's RowVersion byte array to a Base64 string.
        /// </summary>
        /// <param name="user">The user whose RowVersion is to be converted.</param>
        /// <returns>Base64-encoded string representing the RowVersion.</returns>
        public static string RowVersionAsString(this User user)
        {
            return Convert.ToBase64String(user.RowVersion ?? Array.Empty<byte>());
        }
    }
}
