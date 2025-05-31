using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ReservationSystemWebAPI.Services;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ReservationSystemWebAPI.Tests.Services
{
    /// <summary>
    /// Unit tests for the <see cref="ReservationService"/> class using mocked dependencies.
    /// </summary>
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _mockRepo;
        private readonly ReservationService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationServiceTests"/> class with mocked repository.
        /// </summary>
        public ReservationServiceTests()
        {
            _mockRepo = new Mock<IReservationRepository>();
            _service = new ReservationService(_mockRepo.Object);
        }

        /// <summary>
        /// Ensures a valid reservation DTO is correctly processed and returned.
        /// </summary>
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

            _mockRepo
                .Setup(r => r.CreateAsync(It.IsAny<Reservation>()))
                .ReturnsAsync(expectedReservation);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Email, result.Email);
        }

        /// <summary>
        /// Ensures an invalid DTO triggers an exception during reservation creation.
        /// </summary>
        [Fact]
        public async Task CreateAsync_InvalidDto_ThrowsInvalidOperationException()
        {
            var invalidDto = new ReservationCreateDto();

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(invalidDto));
        }

        /// <summary>
        /// Ensures null result from repository during reservation creation throws an exception.
        /// </summary>
        [Fact]
        public async Task CreateAsync_RepoReturnsNull_ThrowsException()
        {
            var dto = new ReservationCreateDto
            {
                Email = "test@wexo.dk",
                Items = new List<ReservationItemCreateDto>
                {
                    new ReservationItemCreateDto { Equipment = "Kabel", Quantity = 1 }
                }
            };

            _mockRepo
                .Setup(r => r.CreateAsync(It.IsAny<Reservation>()))
                .ReturnsAsync((Reservation)null!);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
        }

        /// <summary>
        /// Ensures successful item return updates the reservation and returns true.
        /// </summary>
        [Fact]
        public async Task ReturnItemsAsync_Success_ReturnsTrue()
        {
            int reservationId = 1;
            _mockRepo.Setup(r => r.ReturnItemsAsync(reservationId)).ReturnsAsync(1);

            var result = await _service.ReturnItemsAsync(reservationId);

            Assert.True(result);
        }

        /// <summary>
        /// Ensures item return fails when reservation is not found.
        /// </summary>
        [Fact]
        public async Task ReturnItemsAsync_NotFound_ThrowsException()
        {
            int reservationId = 999;
            _mockRepo.Setup(r => r.ReturnItemsAsync(reservationId)).ReturnsAsync(0);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.ReturnItemsAsync(reservationId)
            );
        }

        /// <summary>
        /// Ensures concurrency conflict during item return throws a concurrency exception.
        /// </summary>
        [Fact]
        public async Task ReturnItemsAsync_ConcurrencyConflict_ThrowsException()
        {
            int reservationId = 1;
            _mockRepo.Setup(r => r.ReturnItemsAsync(reservationId)).ReturnsAsync(-1);

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
                _service.ReturnItemsAsync(reservationId)
            );
        }

        /// <summary>
        /// Ensures a valid reservation ID results in a successful delete.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_Success_ReturnsTrue()
        {
            int reservationId = 1;
            _mockRepo.Setup(r => r.DeleteAsync(reservationId)).ReturnsAsync(1);

            var result = await _service.DeleteAsync(reservationId);

            Assert.True(result);
        }

        /// <summary>
        /// Ensures deleting a non-existent reservation throws an exception.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_NotFound_ThrowsException()
        {
            int reservationId = 404;
            _mockRepo.Setup(r => r.DeleteAsync(reservationId)).ReturnsAsync(0);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteAsync(reservationId)
            );
        }

        /// <summary>
        /// Ensures a valid update changes the reservation as expected.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_Success_ReturnsTrue()
        {
            var dto = new ReservationUpdateDto
            {
                Email = "test@wexo.dk",
                RowVersion = Convert.ToBase64String(new byte[8]),
                Items = new List<ReservationItemUpdateDto>
                {
                    new ReservationItemUpdateDto
                    {
                        Id = 1,
                        Equipment = "HDMI-kabel",
                        Quantity = 1,
                        IsReturned = false,
                        RowVersion = Convert.ToBase64String(new byte[8])
                    }
                }
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Reservation
            {
                Id = 1,
                Email = "test@wexo.dk",
                Status = "Aktiv",
                CreatedAt = DateTime.Now,
                IsCollected = false,
                RowVersion = new byte[8],
                Items = new List<ReservationItems>
                {
                    new ReservationItems
                    {
                        Id = 1,
                        Equipment = "HDMI-kabel",
                        Quantity = 1,
                        IsReturned = false,
                        RowVersion = new byte[8]
                    }
                }
            });

            _mockRepo.Setup(r => r.UpdateAsync(1, It.IsAny<Reservation>())).ReturnsAsync(1);

            var result = await _service.UpdateAsync(1, dto);

            Assert.True(result);
        }

        /// <summary>
        /// Ensures update fails when the reservation is not found.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_NotFound_ThrowsException()
        {
            var dto = new ReservationUpdateDto { RowVersion = Convert.ToBase64String(new byte[8]) };
            _mockRepo.Setup(r => r.UpdateAsync(1, It.IsAny<Reservation>())).ReturnsAsync(0);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateAsync(1, dto));
        }

        /// <summary>
        /// Ensures update fails with a concurrency exception when version conflict occurs.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ConcurrencyConflict_ThrowsException()
        {
            var dto = new ReservationUpdateDto
            {
                Email = "test@wexo.dk",
                RowVersion = Convert.ToBase64String(new byte[8]),
                Items = new List<ReservationItemUpdateDto>
                {
                    new ReservationItemUpdateDto
                    {
                        Id = 1,
                        Equipment = "HDMI-kabel",
                        Quantity = 1,
                        IsReturned = false,
                        RowVersion = Convert.ToBase64String(new byte[8])
                    }
                }
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Reservation
            {
                Id = 1,
                Email = "test@wexo.dk",
                Status = "Aktiv",
                CreatedAt = DateTime.Now,
                IsCollected = false,
                RowVersion = new byte[8],
                Items = new List<ReservationItems>
                {
                    new ReservationItems
                    {
                        Id = 1,
                        Equipment = "HDMI-kabel",
                        Quantity = 1,
                        IsReturned = false,
                        RowVersion = new byte[8]
                    }
                }
            });

            _mockRepo.Setup(r => r.UpdateAsync(1, It.IsAny<Reservation>())).ReturnsAsync(-1);

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
                _service.UpdateAsync(1, dto));
        }
    }
}
