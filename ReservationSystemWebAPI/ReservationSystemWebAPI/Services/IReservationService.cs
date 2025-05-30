using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.DTOs;

namespace ReservationSystemWebAPI.Services
{
    /// <summary>
    /// Service interface for managing reservations and related operations asynchronously.
    /// </summary>
    public interface IReservationService
    {
        /// <summary>
        /// Retrieves all reservations asynchronously.
        /// </summary>
        /// <returns>A collection of all reservations.</returns>
        Task<IEnumerable<Reservation>> GetAllAsync();

        /// <summary>
        /// Retrieves a reservation by its unique ID asynchronously.
        /// </summary>
        /// <param name="id">The reservation ID.</param>
        /// <returns>The reservation if found; otherwise, null.</returns>
        Task<Reservation?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves reservations for a specified user email asynchronously.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <returns>A collection of reservations for the user.</returns>
        Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email);

        /// <summary>
        /// Creates a new reservation asynchronously.
        /// </summary>
        /// <param name="dto">Data transfer object containing reservation details.</param>
        /// <returns>The created reservation entity.</returns>
        Task<Reservation> CreateAsync(ReservationCreateDto dto);

        /// <summary>
        /// Updates an existing reservation asynchronously.
        /// </summary>
        /// <param name="id">The ID of the reservation to update.</param>
        /// <param name="dto">Data transfer object containing update details.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        Task<bool> UpdateAsync(int id, ReservationUpdateDto dto);

        /// <summary>
        /// Deletes a reservation by ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the reservation to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Marks all items in a reservation as returned and updates inventory accordingly asynchronously.
        /// </summary>
        /// <param name="reservationId">The reservation ID.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        Task<bool> ReturnItemsAsync(int reservationId);

        /// <summary>
        /// Creates a history record for the reservation asynchronously.
        /// </summary>
        /// <param name="reservationId">The reservation ID.</param>
        /// <returns>True if the history record was created successfully; otherwise, false.</returns>
        Task<bool> CreateHistoryAsync(int reservationId);
    }
}