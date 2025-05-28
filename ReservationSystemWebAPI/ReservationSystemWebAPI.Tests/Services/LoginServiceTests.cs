using Xunit;
using Moq;
using System.Threading.Tasks;
using ReservationSystemWebAPI.Services;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.Models;
using System.Collections.Generic;

namespace ReservationSystemWebAPI.Tests.Services
{
    public class LoginServiceTests
    {
        private readonly Mock<ILoginRepository> _mockRepo;
        private readonly LoginService _service;

        public LoginServiceTests()
        {
            _mockRepo = new Mock<ILoginRepository>();
            _service = new LoginService(_mockRepo.Object);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ValidCredentials_ReturnsUser()
        {
            // Test: Brugeren findes og adgangskoden matcher → returner bruger

            // Arrange
            var email = "test@example.com";
            var password = "1234";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Id = 1,
                Email = email,
                Password = hashedPassword
            };

            _mockRepo.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync(user);

            // Act
            var result = await _service.AuthenticateUserAsync(email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task AuthenticateUserAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Test: Brugeren findes ikke → kast KeyNotFoundException

            // Arrange
            var email = "ukendt@example.com";
            var password = "any";

            _mockRepo.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.AuthenticateUserAsync(email, password));
        }

        [Fact]
        public async Task AuthenticateUserAsync_WrongPassword_ThrowsUnauthorizedAccessException()
        {
            // Test: Brugeren findes, men adgangskoden matcher ikke → kast UnauthorizedAccessException

            // Arrange
            var email = "test@example.com";
            var correctPassword = "correct";
            var wrongPassword = "wrong";

            var user = new User
            {
                Id = 1,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(correctPassword)
            };

            _mockRepo.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.AuthenticateUserAsync(email, wrongPassword));
        }
    }
}
