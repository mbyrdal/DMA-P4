using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;

namespace ReservationSystemWebAPI.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repository;

        /// <summary>
        /// Constructor: Inject the reservation repository.
        /// </summary>
        public ReservationService(IReservationRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves all reservations asynchronously.
        /// Throws <see cref="InvalidOperationException"/> if none are found.
        /// </summary>
        /// <returns>A collection of all reservations.</returns>
        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            var reservations = await _repository.GetAllAsync();
            if (reservations == null || !reservations.Any())
                throw new InvalidOperationException("Ingen reservationer fundet.");
            return reservations;
        }

        /// <summary>
        /// Retrieves a reservation by its ID asynchronously.
        /// Throws <see cref="KeyNotFoundException"/> if not found.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>The reservation entity if found.</returns>
        public async Task<Reservation?> GetByIdAsync(int id)
        {
            var reservation = await _repository.GetByIdAsync(id);
            if (reservation == null)
                throw new KeyNotFoundException($"Reservation med ID {id} blev ikke fundet.");
            return reservation;
        }

        /// <summary>
        /// Retrieves all reservations for a specific user by email asynchronously.
        /// Throws <see cref="InvalidOperationException"/> if none are found.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <returns>A collection of reservations for the user.</returns>
        public async Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email)
        {
            var reservations = await _repository.GetByUserEmailAsync(email);
            if (reservations == null || !reservations.Any())
                throw new InvalidOperationException("Ingen reservationer fundet for den angivne bruger.");
            return reservations;
        }

        /// <summary>
        /// Creates a new reservation asynchronously from the specified DTO.
        /// Throws <see cref="ArgumentException"/> if data is invalid.
        /// </summary>
        /// <param name="dto">Reservation creation data transfer object.</param>
        /// <returns>The created reservation entity.</returns>
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
                    IsReturned = false // New items are not returned at creation
                }).ToList()
            };

            var createdReservation = await _repository.CreateAsync(reservation);
            if (createdReservation == null)
            {
                throw new ArgumentException("Reservation kunne ikke oprettes. Tjek venligst at alle data er korrekte og prøv igen.");
            }

            return createdReservation;
        }

        /// <summary>
        /// Updates an existing reservation asynchronously.
        /// Throws <see cref="KeyNotFoundException"/> if reservation not found.
        /// Throws <see cref="DbUpdateConcurrencyException"/> if concurrency conflict is detected.
        /// Returns true if update was successful.
        /// </summary>
        /// <param name="id">The reservation ID to update.</param>
        /// <param name="dto">Reservation update data transfer object.</param>
        /// <returns>True if update succeeded; otherwise false.</returns>
        public async Task<bool> UpdateAsync(int id, ReservationUpdateDto dto)
        {
            // Get the existing reservation with tracking for concurrency
            var existingReservation = await _repository.GetByIdAsync(id);
            if(existingReservation == null)
            {
                throw new KeyNotFoundException($"Reservation med ID {id} blev ikke fundet!");
            }

            // Check Reservation concurrency
            var dtoRowVersion = Convert.FromBase64String(dto.RowVersion);
            if(!existingReservation.RowVersion.SequenceEqual(dtoRowVersion))
            {
                throw new DbUpdateConcurrencyException("Der opstod en konflikt ved opdatering af reservation. Tjek venligst at data er korrekte og prøv igen.");
            }

            // Apply updates from DTO to the tracked Reservation
            if(dto.Email != null) existingReservation.Email = dto.Email;
            if(dto.Status != null) existingReservation.Status = dto.Status;
            if(dto.IsCollected.HasValue) existingReservation.IsCollected = dto.IsCollected.Value;

            // Update items
            foreach(var itemDto in dto.Items)
            {
                var existingItem = existingReservation.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                if (existingItem != null) continue;

                // Convert item RowVersion from Base64 string to byte array
                var itemRowVersion = Convert.FromBase64String(itemDto.RowVersion);
                if(!existingItem.RowVersion.SequenceEqual(itemRowVersion))
                {
                    throw new DbUpdateConcurrencyException($"Der opstod en konflikt ved opdatering af reservation item med ID {itemDto.Id}. Tjek venligst at data er korrekte og prøv igen.");
                }

                // Update item properties
                existingItem.Equipment = itemDto.Equipment;
                existingItem.Quantity = itemDto.Quantity;
                existingItem.IsReturned = itemDto.IsReturned; // Update return status
            }

            // Save changes through repository
            var affectedRows = await _repository.UpdateAsync(id, existingReservation);

            if (affectedRows > 0) return true;
            if(affectedRows == -1) throw new DbUpdateConcurrencyException("Der opstod en konflikt ved opdatering af reservation. Tjek venligst at data er korrekte og prøv igen.");

            return false;
        }

        /// <summary>
        /// Deletes a reservation by ID asynchronously.
        /// Throws <see cref="KeyNotFoundException"/> if reservation not found.
        /// Returns true if deletion was successful.
        /// </summary>
        /// <param name="id">The reservation ID to delete.</param>
        /// <returns>True if deletion succeeded.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            var affected = await _repository.DeleteAsync(id);
            if (affected == 0)
                throw new KeyNotFoundException($"Reservation med ID {id} blev ikke fundet.");
            return true;
        }

        /// <summary>
        /// Marks all items in a reservation as returned and updates inventory asynchronously.
        /// Throws <see cref="KeyNotFoundException"/> if reservation or items not found.
        /// Throws <see cref="DbUpdateConcurrencyException"/> if concurrency conflict detected.
        /// Returns true if operation was successful.
        /// </summary>
        /// <param name="reservationId">The reservation ID.</param>
        /// <returns>True if the items were successfully returned.</returns>
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
        /// Throws <see cref="KeyNotFoundException"/> if reservation not found.
        /// Returns true if history record was created successfully.
        /// </summary>
        /// <param name="reservationId">The reservation ID.</param>
        /// <returns>True if the history record was created.</returns>
        public async Task<bool> CreateHistoryAsync(int reservationId)
        {
            var affected = await _repository.CreateHistoryAsync(reservationId);
            if (affected == 0)
                throw new KeyNotFoundException($"Reservation med ID {reservationId} blev ikke fundet.");
            return true;
        }
    }
}