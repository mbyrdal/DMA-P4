using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReservationSystemWebAPI.Tests.Integration
{
    public class ReservationServiceIntegrationTests
    {
        private readonly IReservationService _reservationService;
        private readonly ReservationDbContext _context;

        public ReservationServiceIntegrationTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ReservationDbContext>(options =>
                options.UseSqlServer("Server=hildur.ucn.dk;Database=DMA-CSD-S235_10632110;User Id=DMA-CSD-S235_10632110;Password=Password1!;TrustServerCertificate=true"));

            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IReservationService, ReservationService>();

            var provider = services.BuildServiceProvider();
            _reservationService = provider.GetRequiredService<IReservationService>();
            _context = provider.GetRequiredService<ReservationDbContext>();
        }

        [Fact]
        public async Task CreateAsync_ValidDto_CreatesReservation()
        {
            // Arrange
            var dto = new ReservationDto
            {
                Email = $"integration_res_{Guid.NewGuid()}@wexo.dk",
                Status = "Afventer",
                Items = new List<ReservationItemDto>
                {
                    new ReservationItemDto
                    {
                        Equipment = "HDMI-kabel",
                        Quantity = 1
                    }
                }
            };

            // Act
            var reservation = await _reservationService.CreateAsync(dto);

            // Assert
            Assert.NotNull(reservation);
            Assert.Equal(dto.Email, reservation.Email);

            // Cleanup
            await _reservationService.DeleteAsync(reservation.Id);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsReservations()
        {
            // Arrange
            var dto = new ReservationDto
            {
                Email = $"integration_all_{Guid.NewGuid()}@wexo.dk",
                Status = "Afventer",
                Items = new List<ReservationItemDto>
                {
                    new ReservationItemDto
                    {
                        Equipment = "HDMI-kabel",
                        Quantity = 1
                    }
                }
            };

            var created = await _reservationService.CreateAsync(dto);

            // Act
            var all = await _reservationService.GetAllAsync();

            // Assert
            Assert.Contains(all, r => r.Id == created.Id);

            // Cleanup
            await _reservationService.DeleteAsync(created.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsCorrectReservation()
        {
            var dto = new ReservationDto
            {
                Email = $"integration_getbyid_{Guid.NewGuid()}@wexo.dk",
                Status = "Reserveret",
                Items = new List<ReservationItemDto>
        {
            new ReservationItemDto { Equipment = "HDMI-kabel", Quantity = 1 }
        }
            };

            var created = await _reservationService.CreateAsync(dto);
            var fetched = await _reservationService.GetByIdAsync(created.Id);

            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched.Id);

            await _reservationService.DeleteAsync(created.Id);
        }

        [Fact]
        public async Task GetByUserEmailAsync_ReturnsUserReservations()
        {
            string email = $"integration_userres_{Guid.NewGuid()}@wexo.dk";

            var dto = new ReservationDto
            {
                Email = email,
                Status = "Reserveret",
                Items = new List<ReservationItemDto>
        {
            new ReservationItemDto { Equipment = "HDMI-kabel", Quantity = 1 }
        }
            };

            var created = await _reservationService.CreateAsync(dto);
            var reservations = await _reservationService.GetByUserEmailAsync(email);

            Assert.Contains(reservations, r => r.Id == created.Id);

            await _reservationService.DeleteAsync(created.Id);
        }

        [Fact]
        public async Task UpdateStatusAsync_ValidInput_UpdatesStatus()
        {
            var dto = new ReservationDto
            {
                Email = $"integration_status_{Guid.NewGuid()}@wexo.dk",
                Status = "Reserveret",
                Items = new List<ReservationItemDto>
        {
            new ReservationItemDto { Equipment = "HDMI-kabel", Quantity = 1 }
        }
            };

            var created = await _reservationService.CreateAsync(dto);

            bool updated = await _reservationService.UpdateStatusAsync(created.Id, "Afhentet");

            var updatedRes = await _reservationService.GetByIdAsync(created.Id);

            Assert.True(updated);
            Assert.Equal("Afhentet", updatedRes.Status);

            await _reservationService.DeleteAsync(created.Id);
        }

        [Fact]
        public async Task MarkAsCollectedAsync_ValidId_MarksAsCollected()
        {
            var dto = new ReservationDto
            {
                Email = $"integration_collected_{Guid.NewGuid()}@wexo.dk",
                Status = "Reserveret",
                Items = new List<ReservationItemDto>
        {
            new ReservationItemDto { Equipment = "HDMI-kabel", Quantity = 1 }
        }
            };

            var created = await _reservationService.CreateAsync(dto);
            bool result = await _reservationService.MarkAsCollectedAsync(created.Id);

            var updated = await _reservationService.GetByIdAsync(created.Id);

            Assert.True(result);
            Assert.True(updated.IsCollected);
            Assert.Equal("Aktiv", updated.Status);

            await _reservationService.DeleteAsync(created.Id);
        }

        [Fact]
        public async Task ReturnItemsAsync_ValidReservation_ReturnsAllItems()
        {
            var dto = new ReservationDto
            {
                Email = $"integration_return_{Guid.NewGuid()}@wexo.dk",
                Status = "Aktiv",
                Items = new List<ReservationItemDto>
        {
            new ReservationItemDto { Equipment = "HDMI-kabel", Quantity = 1 }
        }
            };

            var created = await _reservationService.CreateAsync(dto);

            bool result = await _reservationService.ReturnItemsAsync(created.Id);
            var fetched = await _reservationService.GetByIdAsync(created.Id);

            Assert.True(result);
            Assert.All(fetched.Items, i => Assert.True(i.IsReturned));
            Assert.Equal("Inaktiv", fetched.Status);

            await _reservationService.DeleteAsync(created.Id);
        }

        [Fact]
        public async Task DeleteAsync_ValidId_DeletesReservation()
        {
            var dto = new ReservationDto
            {
                Email = $"integration_delete_{Guid.NewGuid()}@wexo.dk",
                Status = "Reserveret",
                Items = new List<ReservationItemDto>
        {
            new ReservationItemDto { Equipment = "HDMI-kabel", Quantity = 1 }
        }
            };

            var created = await _reservationService.CreateAsync(dto);
            bool deleted = await _reservationService.DeleteAsync(created.Id);

            Assert.True(deleted);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _reservationService.GetByIdAsync(created.Id));
        }

    }
}
