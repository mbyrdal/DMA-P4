using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ReservationSystemWebAPI.Services;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ReservationSystemWebAPI.Tests.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _mockRepo;
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _mockRepo = new Mock<IReservationRepository>();
            _service = new ReservationService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidDto_ReturnsReservation()
        {
            // Arrange
            var dto = new ReservationCreateDto
            {
                Email = "test@wexo.dk",
                Items = new List<ReservationItemCreateDto>
                {
                    new ReservationItemCreateDto { Equipment = "HDMI-kabel", Quantity = 2 }
                }
            };

            var expectedReservation = new Reservation
            {
                Id = 1,
                Email = dto.Email,
                Items = new List<ReservationItems>()
            };

            _mockRepo.Setup(r => r.CreateAsync(dto)).ReturnsAsync(expectedReservation);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Email, result.Email);
        }

        [Fact]
        public async Task CreateAsync_InvalidDto_ThrowsArgumentException()
        {
            // Arrange
            var invalidDto = new ReservationCreateDto();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(invalidDto));
        }

        [Fact]
        public async Task CreateAsync_RepoReturnsNull_ReturnsNull()
        {
            // Arrange
            var dto = new ReservationCreateDto
            {
                Email = "test@wexo.dk",
                Items = new List<ReservationItemCreateDto>
                {
                    new ReservationItemCreateDto { Equipment = "Kabel", Quantity = 1 }
                }
            };

            _mockRepo.Setup(r => r.CreateAsync(dto)).ReturnsAsync((Reservation)null);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ReturnItemsAsync_Success_ReturnsTrue()
        {
            // Arrange
            int reservationId = 1;
            _mockRepo.Setup(r => r.ReturnItemsAsync(reservationId)).ReturnsAsync(1);

            // Act
            var result = await _service.ReturnItemsAsync(reservationId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ReturnItemsAsync_NotFound_ThrowsException()
        {
            // Arrange
            int reservationId = 999;
            _mockRepo.Setup(r => r.ReturnItemsAsync(reservationId)).ReturnsAsync(0);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.ReturnItemsAsync(reservationId)
            );
        }

        [Fact]
        public async Task ReturnItemsAsync_ConcurrencyConflict_ThrowsException()
        {
            // Arrange
            int reservationId = 1;
            _mockRepo.Setup(r => r.ReturnItemsAsync(reservationId)).ReturnsAsync(-1);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
                _service.ReturnItemsAsync(reservationId)
            );
        }

        [Fact]
        public async Task DeleteAsync_Success_ReturnsTrue()
        {
            // Arrange
            int reservationId = 1;
            _mockRepo.Setup(r => r.DeleteAsync(reservationId)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(reservationId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_NotFound_ThrowsException()
        {
            // Arrange
            int reservationId = 404;
            _mockRepo.Setup(r => r.DeleteAsync(reservationId)).ReturnsAsync(0);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteAsync(reservationId)
            );
        }

        [Fact]
        public async Task UpdateAsync_Success_ReturnsTrue()
        {
            // Arrange
            var dto = new ReservationUpdateDto { RowVersion = new byte[8] };
            _mockRepo.Setup(r => r.UpdateAsync(1, dto)).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(1, dto);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAsync_NotFound_ThrowsException()
        {
            // Arrange
            var dto = new ReservationUpdateDto { RowVersion = new byte[8] };
            _mockRepo.Setup(r => r.UpdateAsync(1, dto)).ReturnsAsync(0);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateAsync(1, dto));
        }

        [Fact]
        public async Task UpdateAsync_ConcurrencyConflict_ThrowsException()
        {
            // Arrange
            var dto = new ReservationUpdateDto { RowVersion = new byte[8] };
            _mockRepo.Setup(r => r.UpdateAsync(1, dto)).ReturnsAsync(-1);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
                _service.UpdateAsync(1, dto));
        }
    }
}
