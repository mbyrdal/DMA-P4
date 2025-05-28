using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.Services;
using System;

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
        public async Task GetAllAsync_UsersExist_ReturnsUserList()
        {
            // Verificerer at en liste med brugere returneres korrekt, når brugere findes i databasen

            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Email = "a@b.com" },
                new User { Id = 2, Email = "c@d.com" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<User>)result).Count);
        }

        [Fact]
        public async Task GetAllAsync_RepositoryReturnsNull_ThrowsArgumentNullException()
        {
            // Forventer ArgumentNullException hvis repository returnerer null

            // Arrange
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync((IEnumerable<User>)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetAllAsync());
        }

        [Fact]
        public async Task GetAllAsync_EmptyList_ThrowsInvalidOperationException()
        {
            // Forventer InvalidOperationException hvis listen af brugere er tom

            // Arrange
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetAllAsync());
        }

        [Fact]
        public async Task AddAsync_ValidUser_ReturnsNewUserId()
        {
            // Bekræfter at ID returneres korrekt når bruger oprettes med gyldig e-mail

            // Arrange
            var user = new User { Email = "test@wexo.dk" };
            _mockRepo.Setup(r => r.AddAsync(user)).ReturnsAsync(42);

            // Act
            var result = await _service.AddAsync(user);

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public async Task AddAsync_InvalidUserEmail_ThrowsArgumentException()
        {
            // Forventer ArgumentException ved tom eller ugyldig e-mail

            // Arrange
            var user = new User { Email = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(user));
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsUser()
        {
            // Bekræfter at korrekt bruger returneres, når ID findes

            // Arrange
            int id = 1;
            var user = new User { Id = id, Email = "test@wexo.dk" };
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Forventer KeyNotFoundException hvis bruger ikke findes med givent ID

            // Arrange
            int id = 99;
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
        }

        [Fact]
        public async Task UpdateAsync_UserExists_ReturnsUserId()
        {
            // Verificerer at opdatering returnerer korrekt ID, når bruger eksisterer

            // Arrange
            var user = new User { Id = 2, Email = "opdateret@wexo.dk" };
            _mockRepo.Setup(r => r.ExistsAsync(user.Id)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.UpdateAsync(user)).ReturnsAsync(user.Id);

            // Act
            var result = await _service.UpdateAsync(user);

            // Assert
            Assert.Equal(user.Id, result);
        }

        [Fact]
        public async Task UpdateAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Forventer KeyNotFoundException hvis bruger ikke findes ved opdatering

            // Arrange
            var user = new User { Id = 99, Email = "ukendt@wexo.dk" };
            _mockRepo.Setup(r => r.ExistsAsync(user.Id)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(user));
        }

        [Fact]
        public async Task UpdateAsync_RepositoryFails_ThrowsInvalidOperationException()
        {
            // Forventer InvalidOperationException hvis repository kaster en exception under opdatering

            // Arrange
            var user = new User { Id = 3, Email = "fejl@wexo.dk" };
            _mockRepo.Setup(r => r.ExistsAsync(user.Id)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.UpdateAsync(user)).ThrowsAsync(new Exception("DB-fejl"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(user));
        }

        [Fact]
        public async Task DeleteAsync_UserExists_ReturnsUserId()
        {
            // Bekræfter at sletning returnerer brugerens ID, når bruger findes

            // Arrange
            int id = 4;
            _mockRepo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.DeleteAsync(id)).ReturnsAsync(id);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            Assert.Equal(id, result);
        }

        [Fact]
        public async Task DeleteAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Forventer KeyNotFoundException hvis bruger ikke findes ved sletning

            // Arrange
            int id = 123;
            _mockRepo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(id));
        }

        [Fact]
        public async Task DeleteAsync_RepositoryFails_ThrowsInvalidOperationException()
        {
            // Forventer InvalidOperationException hvis repository kaster en exception under sletning

            // Arrange
            int id = 5;
            _mockRepo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.DeleteAsync(id)).ThrowsAsync(new Exception("DB-fejl"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(id));
        }
    }
}
