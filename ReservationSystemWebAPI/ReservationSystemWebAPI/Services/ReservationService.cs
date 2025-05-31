using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReservationSystemWebAPI.Services
{
    /// <summary>
    /// Provides business logic and operations for managing reservations.
    /// </summary>
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationService"/> class.
        /// </summary>
        /// <param name="repository">The repository used to access reservation data.</param>
        public ReservationService(IReservationRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves all reservations asynchronously.
        /// Throws <see cref="InvalidOperationException"/> if no reservations exist.
        /// </summary>
        /// <returns>A collection of all <see cref="Reservation"/> entities.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no reservations are found.</exception>
        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            var reservations = await _repository.GetAllAsync();
            if (reservations == null || !reservations.Any())
                throw new InvalidOperationException("Ingen reservationer fundet.");
            return reservations;
        }

        /// <summary>
        /// Retrieves a reservation by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The ID of the reservation.</param>
        /// <returns>The <see cref="Reservation"/> if found; otherwise, null.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the reservation is not found.</exception>
        public async Task<Reservation?> GetByIdAsync(int id)
        {
            var reservation = await _repository.GetByIdAsync(id);
            if (reservation == null)
                throw new KeyNotFoundException($"Reservation med ID {id} blev ikke fundet.");
            return reservation;
        }

        /// <summary>
        /// Retrieves all reservations for a specified user email asynchronously.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>A collection of <see cref="Reservation"/> entities for the user.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no reservations are found for the user.</exception>
        public async Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email)
        {
            var reservations = await _repository.GetByUserEmailAsync(email);
            if (reservations == null || !reservations.Any())
                throw new InvalidOperationException("Ingen reservationer fundet for den angivne bruger.");
            return reservations;
        }

        /// <summary>
        /// Creates a new reservation asynchronously from the provided data transfer object.
        /// </summary>
        /// <param name="dto">The <see cref="ReservationCreateDto"/> containing reservation details.</param>
        /// <returns>The newly created <see cref="Reservation"/> entity.</returns>
        /// <exception cref="ArgumentException">Thrown if the reservation could not be created.</exception>
        public async Task<Reservation> CreateAsync(ReservationCreateDto dto)
        {
            var reservation = new Reservation
            {
                Email = dto.Email,
                Status = dto.Status,
                CreatedAt = DateTime.Now,
                IsCollected = dto.IsCollected,
                Items = dto.Items.Select(i => new ReservationItems
                {
                    Equipment = i.Equipment,
                    Quantity = i.Quantity,
                    IsReturned = false
                }).ToList()
            };

            var createdReservation = await _repository.CreateAsync(reservation);

            if (createdReservation == null)
                throw new ArgumentException("Reservation kunne ikke oprettes. Tjek venligst at alle data er korrekte og prøv igen.");

            return createdReservation;
        }

        /// <summary>
        /// Updates an existing reservation asynchronously.
        /// </summary>
        /// <param name="id">The ID of the reservation to update.</param>
        /// <param name="dto">The <see cref="ReservationUpdateDto"/> containing updated reservation data.</param>
        /// <returns>True if the update succeeded; otherwise, false.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the reservation does not exist.</exception>
        /// <exception cref="DbUpdateConcurrencyException">Thrown if a concurrency conflict occurs during update.</exception>
        public async Task<bool> UpdateAsync(int id, ReservationUpdateDto dto)
        {
            var existingReservation = await _repository.GetByIdAsync(id);
            if (existingReservation == null)
                throw new KeyNotFoundException($"Reservation med ID {id} blev ikke fundet!");

            var dtoRowVersion = Convert.FromBase64String(dto.RowVersion);
            if (!existingReservation.RowVersion.SequenceEqual(dtoRowVersion))
                throw new DbUpdateConcurrencyException("Der opstod en konflikt ved opdatering af reservation. Tjek venligst at data er korrekte og prøv igen.");

            if (dto.Email != null) existingReservation.Email = dto.Email;
            if (dto.Status != null) existingReservation.Status = dto.Status;
            if (dto.IsCollected.HasValue) existingReservation.IsCollected = dto.IsCollected.Value;

            foreach (var itemDto in dto.Items)
            {
                var existingItem = existingReservation.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                if (existingItem == null) continue;

                var itemRowVersion = Convert.FromBase64String(itemDto.RowVersion);
                if (!existingItem.RowVersion.SequenceEqual(itemRowVersion))
                    throw new DbUpdateConcurrencyException($"Der opstod en konflikt ved opdatering af reservation item med ID {itemDto.Id}. Tjek venligst at data er korrekte og prøv igen.");

                existingItem.Equipment = itemDto.Equipment;
                existingItem.Quantity = itemDto.Quantity;
                existingItem.IsReturned = itemDto.IsReturned;
            }

            var affectedRows = await _repository.UpdateAsync(id, existingReservation);

            if (affectedRows > 0) return true;
            if (affectedRows == -1) throw new DbUpdateConcurrencyException("Der opstod en konflikt ved opdatering af reservation. Tjek venligst at data er korrekte og prøv igen.");

            return false;
        }

        /// <summary>
        /// Deletes a reservation by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the reservation to delete.</param>
        /// <returns>True if the deletion was successful.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the reservation was not found.</exception>
        public async Task<bool> DeleteAsync(int id)
        {
            var affected = await _repository.DeleteAsync(id);
            if (affected == 0)
                throw new KeyNotFoundException($"Reservation med ID {id} blev ikke fundet.");
            return true;
        }

        /// <summary>
        /// Marks all items in a reservation as returned and updates inventory accordingly asynchronously.
        /// </summary>
        /// <param name="reservationId">The reservation ID.</param>
        /// <returns>True if the items were successfully marked as returned.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the reservation or items were not found.</exception>
        /// <exception cref="DbUpdateConcurrencyException">Thrown if a concurrency conflict occurs.</exception>
        /// <exception cref="InvalidOperationException">Thrown if an unexpected result is encountered.</exception>
        public async Task<bool> ReturnItemsAsync(int reservationId)
        {
            var affected = await _repository.ReturnItemsAsync(reservationId);

            if (affected > 0) return true;
            if (affected == 0) throw new KeyNotFoundException($"Reservation med ID {reservationId} blev ikke fundet eller har intet tilknyttet udstyr.");
            if (affected < 0) throw new DbUpdateConcurrencyException("Der opstod en konflikt ved returnering af items. Tjek venligst at data er korrekte og prøv igen.");

            throw new InvalidOperationException("Uventet resultat ved returnering af items.");
        }

        /// <summary>
        /// Creates a history record for a reservation asynchronously.
        /// </summary>
        /// <param name="reservationId">The reservation ID.</param>
        /// <returns>True if the history record was successfully created.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the reservation was not found.</exception>
        public async Task<bool> CreateHistoryAsync(int reservationId)
        {
            var affected = await _repository.CreateHistoryAsync(reservationId);
            if (affected == 0)
                throw new KeyNotFoundException($"Reservation med ID {reservationId} blev ikke fundet.");
            return true;
        }
    }
}