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
    /// <summary>
    /// Integration tests for <see cref="ReservationService"/> with a real database.
    /// Covers creation, retrieval, updating, return handling, and deletion of reservations.
    /// </summary>
    public class ReservationServiceIntegrationTests
    {
        private readonly IReservationService _reservationService;
        private readonly ReservationDbContext _context;

        /// <summary>
        /// Initializes dependencies using a real database connection and resolves services.
        /// </summary>
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

        /// <summary>
        /// Ensures a reservation can be created using valid input and then deleted successfully.
        /// </summary>
        [Fact]
        public async Task CreateAsync_ValidDto_CreatesReservation()
        {
            var dto = new ReservationCreateDto
            {
                Email = $"integration_res_{Guid.NewGuid()}@wexo.dk",
                Status = "Afventer",
                Items = new List<ReservationItemCreateDto>
                {
                    new ReservationItemCreateDto { Equipment = "HDMI-kabel", Quantity = 1 }
                }
            };

            var reservation = await _reservationService.CreateAsync(dto);

            Assert.NotNull(reservation);
            Assert.Equal(dto.Email, reservation.Email);

            await _reservationService.DeleteAsync(reservation.Id);
        }

        /// <summary>
        /// Ensures that created reservations appear in the list returned by <c>GetAllAsync</c>.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ReturnsReservations()
        {
            var dto = new ReservationCreateDto
            {
                Email = $"integration_all_{Guid.NewGuid()}@wexo.dk",
                Status = "Afventer",
                Items = new List<ReservationItemCreateDto>
                {
                    new ReservationItemCreateDto { Equipment = "HDMI-kabel", Quantity = 1 }
                }
            };

            var created = await _reservationService.CreateAsync(dto);
            var all = await _reservationService.GetAllAsync();

            Assert.Contains(all, r => r.Id == created.Id);

            await _reservationService.DeleteAsync(created.Id);
        }

        /// <summary>
        /// Verifies that a reservation can be retrieved by its ID.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsCorrectReservation()
        {
            var dto = new ReservationCreateDto
            {
                Email = $"integration_getbyid_{Guid.NewGuid()}@wexo.dk",
                Status = "Reserveret",
                Items = new List<ReservationItemCreateDto>
                {
                    new ReservationItemCreateDto { Equipment = "HDMI-kabel", Quantity = 1 }
                }
            };

            var created = await _reservationService.CreateAsync(dto);
            var fetched = await _reservationService.GetByIdAsync(created.Id);

            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched.Id);

            await _reservationService.DeleteAsync(created.Id);
        }

        /// <summary>
        /// Ensures that a reservation can be fetched by user email.
        /// </summary>
        [Fact]
        public async Task GetByUserEmailAsync_ReturnsUserReservations()
        {
            string email = $"integration_userres_{Guid.NewGuid()}@wexo.dk";

            var dto = new ReservationCreateDto
            {
                Email = email,
                Status = "Reserveret",
                Items = new List<ReservationItemCreateDto>
                {
                    new ReservationItemCreateDto { Equipment = "HDMI-kabel", Quantity = 1 }
                }
            };

            var created = await _reservationService.CreateAsync(dto);
            var reservations = await _reservationService.GetByUserEmailAsync(email);

            Assert.Contains(reservations, r => r.Id == created.Id);

            await _reservationService.DeleteAsync(created.Id);
        }

        /// <summary>
        /// Ensures that a reservation's status can be updated and saved correctly.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ValidInput_UpdatesStatus()
        {
            var dto = new ReservationCreateDto
            {
                Email = $"integration_status_{Guid.NewGuid()}@wexo.dk",
                Status = "Reserveret",
                Items = new List<ReservationItemCreateDto>
                {
                    new ReservationItemCreateDto { Equipment = "HDMI-kabel", Quantity = 1 }
                }
            };

            var created = await _reservationService.CreateAsync(dto);
            var original = await _reservationService.GetByIdAsync(created.Id);

            var updateDto = new ReservationUpdateDto
            {
                Status = "Afhentet",
                RowVersion = Convert.ToBase64String(original.RowVersion ?? Array.Empty<byte>())
            };

            var updated = await _reservationService.UpdateAsync(created.Id, updateDto);
            var updatedRes = await _reservationService.GetByIdAsync(created.Id);

            Assert.True(updated);
            Assert.Equal("Afhentet", updatedRes.Status);

            await _reservationService.DeleteAsync(created.Id);
        }

        /// <summary>
        /// Ensures that all reservation items can be marked as returned and status is updated accordingly.
        /// </summary>
        [Fact]
        public async Task ReturnItemsAsync_ValidReservation_ReturnsAllItems()
        {
            var dto = new ReservationCreateDto
            {
                Email = $"integration_return_{Guid.NewGuid()}@wexo.dk",
                Status = "Aktiv",
                Items = new List<ReservationItemCreateDto>
                {
                    new ReservationItemCreateDto { Equipment = "HDMI-kabel", Quantity = 1 }
                }
            };

            var created = await _reservationService.CreateAsync(dto);
            var result = await _reservationService.ReturnItemsAsync(created.Id);
            var fetched = await _reservationService.GetByIdAsync(created.Id);

            Assert.True(result);
            Assert.All(fetched.Items, i => Assert.True(i.IsReturned));
            Assert.Equal("Inaktiv", fetched.Status);

            await _reservationService.DeleteAsync(created.Id);
        }

        /// <summary>
        /// Ensures that a reservation can be deleted and is no longer retrievable.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ValidId_DeletesReservation()
        {
            var dto = new ReservationCreateDto
            {
                Email = $"integration_delete_{Guid.NewGuid()}@wexo.dk",
                Status = "Reserveret",
                Items = new List<ReservationItemCreateDto>
                {
                    new ReservationItemCreateDto { Equipment = "HDMI-kabel", Quantity = 1 }
                }
            };

            var created = await _reservationService.CreateAsync(dto);
            bool deleted = await _reservationService.DeleteAsync(created.Id);

            Assert.True(deleted);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _reservationService.GetByIdAsync(created.Id));
        }
    }
}
