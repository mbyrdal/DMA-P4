using Xunit;
using Moq;
using System.Threading.Tasks;
using ReservationSystemWebAPI.Services;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Tests.Services
{
    /// <summary>
    /// Unit tests for the <see cref="LoginService"/> class.
    /// Verifies authentication logic using mocked dependencies.
    /// </summary>
    public class LoginServiceTests
    {
        private readonly Mock<ILoginRepository> _mockRepo;
        private readonly LoginService _service;

        /// <summary>
        /// Initializes the test suite with a mocked <see cref="ILoginRepository"/>.
        /// </summary>
        public LoginServiceTests()
        {
            _mockRepo = new Mock<ILoginRepository>();
            _service = new LoginService(_mockRepo.Object);
        }

        /// <summary>
        /// Ensures a valid email and password returns the correct user.
        /// </summary>
        [Fact]
        public async Task AuthenticateUserAsync_ValidCredentials_ReturnsUser()
        {
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

        /// <summary>
        /// Ensures an exception is thrown if the user does not exist.
        /// </summary>
        [Fact]
        public async Task AuthenticateUserAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var email = "ukendt@example.com";
            var password = "any";

            _mockRepo.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.AuthenticateUserAsync(email, password));
        }

        /// <summary>
        /// Ensures an exception is thrown if the password is incorrect.
        /// </summary>
        [Fact]
        public async Task AuthenticateUserAsync_WrongPassword_ThrowsUnauthorizedAccessException()
        {
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
