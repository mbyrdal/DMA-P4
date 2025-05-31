using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservationSystemWebAPI.Services;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.Models;
using System;
using Microsoft.EntityFrameworkCore;

namespace ReservationSystemWebAPI.Tests.Services
{
    /// <summary>
    /// Unit tests for the <see cref="UserService"/> class using mocked dependencies.
    /// </summary>
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserServiceTests"/> class.
        /// </summary>
        public UserServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _service = new UserService(_mockRepo.Object);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetAllAsync"/> returns a list of users.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ReturnsUsers()
        {
            var expected = new List<User> { new User { Id = 1, Email = "test@wexo.dk" } };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetAllAsync"/> throws an exception when the list is empty.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WhenEmpty_ThrowsInvalidOperationException()
        {
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetAllAsync());
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetByIdAsync"/> returns a user when found.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_UserExists_ReturnsUser()
        {
            var user = new User { Id = 1, Email = "test@wexo.dk" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("test@wexo.dk", result.Email);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetByIdAsync"/> throws when the user is not found.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(1));
        }

        /// <summary>
        /// Verifies that <see cref="UserService.AddAsync"/> returns affected rows when the user is valid.
        /// </summary>
        [Fact]
        public async Task AddAsync_ValidUser_ReturnsAffectedRows()
        {
            var newUser = new User { Email = "new@wexo.dk" };
            _mockRepo.Setup(r => r.AddAsync(newUser)).ReturnsAsync(1);

            var result = await _service.AddAsync(newUser);

            Assert.Equal(1, result);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.AddAsync"/> throws for invalid email input.
        /// </summary>
        [Fact]
        public async Task AddAsync_InvalidEmail_ThrowsArgumentException()
        {
            var newUser = new User { Email = " " };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(newUser));
        }

        /// <summary>
        /// Verifies that <see cref="UserService.UpdateAsync"/> returns affected rows for a valid user.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_UserExists_ReturnsAffectedRows()
        {
            var user = new User { Id = 1, Email = "update@wexo.dk", RowVersion = new byte[8] };
            _mockRepo.Setup(r => r.ExistsAsync(user.Id)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.UpdateAsync(user)).ReturnsAsync(1);

            var result = await _service.UpdateAsync(user);

            Assert.Equal(1, result);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.UpdateAsync"/> throws when user does not exist.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            var user = new User { Id = 999, Email = "missing@wexo.dk", RowVersion = new byte[8] };
            _mockRepo.Setup(r => r.ExistsAsync(user.Id)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(user));
        }

        /// <summary>
        /// Verifies that <see cref="UserService.DeleteAsync"/> returns affected rows for valid input.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ValidInput_ReturnsAffectedRows()
        {
            int id = 1;
            string rowVersion = Convert.ToBase64String(new byte[8]);

            _mockRepo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.DeleteAsync(id, It.IsAny<byte[]>())).ReturnsAsync(1);

            var result = await _service.DeleteAsync(id, rowVersion);

            Assert.Equal(1, result);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.DeleteAsync"/> throws for invalid Base64 RowVersion.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_InvalidRowVersion_ThrowsArgumentException()
        {
            int id = 1;
            string invalidRowVersion = "NotBase64";

            _mockRepo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DeleteAsync(id, invalidRowVersion));
        }

        /// <summary>
        /// Verifies that <see cref="UserService.DeleteAsync"/> throws when a concurrency conflict occurs.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ConcurrencyConflict_ThrowsInvalidOperationException()
        {
            int id = 1;
            string rowVersion = Convert.ToBase64String(new byte[8]);

            _mockRepo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.DeleteAsync(id, It.IsAny<byte[]>()))
                     .ThrowsAsync(new DbUpdateConcurrencyException());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.DeleteAsync(id, rowVersion));
        }

        /// <summary>
        /// Verifies that <see cref="UserService.DeleteAsync"/> throws when the user does not exist.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteAsync(999, Convert.ToBase64String(new byte[8])));
        }
    }
}
