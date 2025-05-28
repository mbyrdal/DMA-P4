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
            // Verificerer at der returneres en Reservation, når en gyldig DTO leveres

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

            var result = await _service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Email, result.Email);
        }

        [Fact]
        public async Task CreateAsync_InvalidDto_ThrowsArgumentException()
        {
            // Forventer ArgumentException ved ugyldig DTO uden email og items

            var invalidDto = new ReservationCreateDto();

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(invalidDto));
        }

        [Fact]
        public async Task CreateAsync_RepoReturnsNull_ReturnsNull()
        {
            // Verificerer at der returneres null, hvis repository returnerer null

            var dto = new ReservationCreateDto
            {
                Email = "test@wexo.dk",
                Items = new List<ReservationItemCreateDto>
                {
                    new ReservationItemCreateDto { Equipment = "Kabel", Quantity = 1 }
                }
            };

            _mockRepo.Setup(r => r.CreateAsync(dto)).ReturnsAsync((Reservation)null);

            var result = await _service.CreateAsync(dto);

            Assert.Null(result);
        }

        [Fact]
        public async Task MarkAsCollectedAsync_ValidId_ReturnsTrue()
        {
            // Verificerer at metoden returnerer true ved gyldigt ID

            int reservationId = 1;
            _mockRepo.Setup(r => r.MarkAsCollectedAsync(reservationId)).ReturnsAsync(true);

            var result = await _service.MarkAsCollectedAsync(reservationId);

            Assert.True(result);
        }

        [Fact]
        public async Task MarkAsCollectedAsync_RepoFails_ReturnsFalse()
        {
            // Verificerer at metoden returnerer false hvis repository fejler

            int reservationId = 999;
            _mockRepo.Setup(r => r.MarkAsCollectedAsync(reservationId)).ReturnsAsync(false);

            var result = await _service.MarkAsCollectedAsync(reservationId);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateStatusAsync_ValidInput_ReturnsTrue()
        {
            // Verificerer at status opdateres korrekt når input er gyldigt

            int id = 1;
            string status = "Afhentet";

            _mockRepo.Setup(r => r.UpdateStatusAsync(id, status)).ReturnsAsync(true);

            var result = await _service.UpdateStatusAsync(id, status);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateStatusAsync_EmptyStatus_ThrowsArgumentException()
        {
            // Forventer ArgumentException hvis status er tom eller kun whitespace

            int id = 1;
            string status = "   ";

            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateStatusAsync(id, status));
        }

        [Fact]
        public async Task UpdateStatusAsync_RepoFails_ReturnsFalse()
        {
            // Verificerer at false returneres hvis repository ikke kan opdatere status

            int id = 2;
            string status = "Ikke afhentet";

            _mockRepo.Setup(r => r.UpdateStatusAsync(id, status)).ReturnsAsync(false);

            var result = await _service.UpdateStatusAsync(id, status);

            Assert.False(result);
        }

        [Fact]
        public async Task ReturnItemsAsync_ValidId_ReturnsTrue()
        {
            // Verificerer at returnering lykkes ved gyldigt ID

            int reservationId = 1;
            _mockRepo.Setup(r => r.ReturnItemsAsync(reservationId)).ReturnsAsync(true);

            var result = await _service.ReturnItemsAsync(reservationId);

            Assert.True(result);
        }

        [Fact]
        public async Task ReturnItemsAsync_InvalidId_ReturnsFalse()
        {
            // Verificerer at returnering fejler ved ugyldigt ID

            int reservationId = 999;
            _mockRepo.Setup(r => r.ReturnItemsAsync(reservationId)).ReturnsAsync(false);

            var result = await _service.ReturnItemsAsync(reservationId);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsTrue()
        {
            // Verificerer at en reservation slettes korrekt ved gyldigt ID

            int reservationId = 1;
            _mockRepo.Setup(r => r.DeleteAsync(reservationId)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(reservationId);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_InvalidId_ReturnsFalse()
        {
            // Verificerer at false returneres når reservationen ikke findes

            int reservationId = 404;
            _mockRepo.Setup(r => r.DeleteAsync(reservationId)).ReturnsAsync(false);

            var result = await _service.DeleteAsync(reservationId);

            Assert.False(result);
        }
    }
}
