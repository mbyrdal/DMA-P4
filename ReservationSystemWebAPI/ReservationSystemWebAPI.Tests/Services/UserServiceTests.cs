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
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _service = new UserService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsUsers()
        {
            var expected = new List<User> { new User { Id = 1, Email = "test@wexo.dk" } };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ThrowsInvalidOperationException()
        {
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetAllAsync());
        }

        [Fact]
        public async Task GetByIdAsync_UserExists_ReturnsUser()
        {
            var user = new User { Id = 1, Email = "test@wexo.dk" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("test@wexo.dk", result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(1));
        }

        [Fact]
        public async Task AddAsync_ValidUser_ReturnsAffectedRows()
        {
            var newUser = new User { Email = "new@wexo.dk" };
            _mockRepo.Setup(r => r.AddAsync(newUser)).ReturnsAsync(1);

            var result = await _service.AddAsync(newUser);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task AddAsync_InvalidEmail_ThrowsArgumentException()
        {
            var newUser = new User { Email = " " };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(newUser));
        }

        [Fact]
        public async Task UpdateAsync_UserExists_ReturnsAffectedRows()
        {
            var user = new User { Id = 1, Email = "update@wexo.dk", RowVersion = new byte[8] };
            _mockRepo.Setup(r => r.ExistsAsync(user.Id)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.UpdateAsync(user)).ReturnsAsync(1);

            var result = await _service.UpdateAsync(user);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UpdateAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            var user = new User { Id = 999, Email = "missing@wexo.dk", RowVersion = new byte[8] };
            _mockRepo.Setup(r => r.ExistsAsync(user.Id)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(user));
        }

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

        [Fact]
        public async Task DeleteAsync_InvalidRowVersion_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DeleteAsync(1, "NotBase64"));
        }

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

        [Fact]
        public async Task DeleteAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteAsync(999, Convert.ToBase64String(new byte[8])));
        }
    }
}