using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ReservationSystemWebAPI.Services;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Tests.Services
{
    public class ReservationServiceTests
    {
        // Mock af repository som vi kan kontrollere og forudsige
        private readonly Mock<IReservationRepository> _mockRepo;

        // Den service vi tester (med mocket repository som afhængighed)
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _mockRepo = new Mock<IReservationRepository>();
            _service = new ReservationService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidDto_ReturnsReservation()
        {
            // TEST: Når vi giver en gyldig DTO, skal CreateAsync returnere en Reservation

            // Arrange – lav en gyldig DTO med Email og mindst én item
            var dto = new ReservationDto
            {
                Email = "test@wexo.dk",
                Items = new List<ReservationItemDto>
                {
                    new ReservationItemDto { Equipment = "HDMI-kabel", Quantity = 2 }
                }
            };

            // Forventet resultat (repository skal returnere dette)
            var expectedReservation = new Reservation
            {
                Id = 1,
                Email = dto.Email,
                Items = new List<ReservationItems>()
            };

            // Mock: Repositoryens CreateAsync returnerer den forventede reservation
            _mockRepo
                .Setup(r => r.CreateAsync(dto))
                .ReturnsAsync(expectedReservation);

            // Act – kald service-metoden
            var result = await _service.CreateAsync(dto);

            // Assert – vi forventer et ikke-null resultat med samme Email
            Assert.NotNull(result);
            Assert.Equal(dto.Email, result.Email);
        }

        [Fact]
        public async Task CreateAsync_InvalidDto_ThrowsArgumentException()
        {
            // TEST: Når vi sender en ugyldig DTO (uden Email og Items), skal metoden kaste ArgumentException

            // Arrange – DTO uden Email og Items
            var invalidDto = new ReservationDto();

            // Act & Assert – vi forventer en ArgumentException
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(invalidDto));
        }

        [Fact]
        public async Task CreateAsync_RepoReturnsNull_ReturnsNull()
        {
            // TEST: Når repository returnerer null, skal service også returnere null

            // Arrange – en gyldig DTO
            var dto = new ReservationDto
            {
                Email = "test@wexo.dk",
                Items = new List<ReservationItemDto>
                {
                    new ReservationItemDto { Equipment = "Kabel", Quantity = 1 }
                }
            };

            // Mock: Repository returnerer null
            _mockRepo.Setup(r => r.CreateAsync(dto)).ReturnsAsync((Reservation)null);

            // Act – kald CreateAsync
            var result = await _service.CreateAsync(dto);

            // Assert – resultatet skal være null
            Assert.Null(result);
        }

        [Fact]
        public async Task MarkAsCollectedAsync_ValidId_ReturnsTrue()
        {
            // TEST: Når repository returnerer true for en gyldig ID, skal service gøre det samme

            // Arrange – vi simulerer at reservationen med ID 1 findes og kan markeres som afhentet
            int reservationId = 1;
            _mockRepo.Setup(r => r.MarkAsCollectedAsync(reservationId)).ReturnsAsync(true);

            // Act – kald service-metoden
            var result = await _service.MarkAsCollectedAsync(reservationId);

            // Assert – resultatet skal være true
            Assert.True(result);
        }

        [Fact]
        public async Task MarkAsCollectedAsync_RepoFails_ReturnsFalse()
        {
            // TEST: Hvis repository fejler (returnerer false), skal service også returnere false

            // Arrange – simulerer fx ugyldigt ID eller teknisk fejl
            int reservationId = 999;
            _mockRepo.Setup(r => r.MarkAsCollectedAsync(reservationId)).ReturnsAsync(false);

            // Act – kald service
            var result = await _service.MarkAsCollectedAsync(reservationId);

            // Assert – vi forventer false som svar
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateStatusAsync_ValidInput_ReturnsTrue()
        {
            // Test: Når status er gyldig og repository returnerer true

            // Arrange
            int id = 1;
            string status = "Afhentet";

            _mockRepo.Setup(r => r.UpdateStatusAsync(id, status)).ReturnsAsync(true);

            // Act
            var result = await _service.UpdateStatusAsync(id, status);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateStatusAsync_EmptyStatus_ThrowsArgumentException()
        {
            // Test: Når status er tom, skal ArgumentException kastes

            // Arrange
            int id = 1;
            string status = "   "; // whitespace er ugyldigt

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateStatusAsync(id, status));
        }

        [Fact]
        public async Task UpdateStatusAsync_RepoFails_ReturnsFalse()
        {
            // Test: Når repository returnerer false, skal service også gøre det

            // Arrange
            int id = 2;
            string status = "Ikke afhentet";

            _mockRepo.Setup(r => r.UpdateStatusAsync(id, status)).ReturnsAsync(false);

            // Act
            var result = await _service.UpdateStatusAsync(id, status);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ReturnItemsAsync_ValidId_ReturnsTrue()
        {
            // Test: Når reservation findes og returnering lykkes, skal metoden returnere true

            // Arrange
            int reservationId = 1;
            _mockRepo.Setup(r => r.ReturnItemsAsync(reservationId)).ReturnsAsync(true);

            // Act
            var result = await _service.ReturnItemsAsync(reservationId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ReturnItemsAsync_InvalidId_ReturnsFalse()
        {
            // Test: Når reservation ikke findes eller repo fejler, skal returneringen give false

            // Arrange
            int reservationId = 999;
            _mockRepo.Setup(r => r.ReturnItemsAsync(reservationId)).ReturnsAsync(false);

            // Act
            var result = await _service.ReturnItemsAsync(reservationId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsTrue()
        {
            // Test: Når en reservation slettes med gyldigt ID, returneres true

            // Arrange
            int reservationId = 1;
            _mockRepo.Setup(r => r.DeleteAsync(reservationId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(reservationId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_InvalidId_ReturnsFalse()
        {
            // Test: Hvis reservationen ikke findes, returneres false

            // Arrange
            int reservationId = 404;
            _mockRepo.Setup(r => r.DeleteAsync(reservationId)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(reservationId);

            // Assert
            Assert.False(result);
        }

    }
}
