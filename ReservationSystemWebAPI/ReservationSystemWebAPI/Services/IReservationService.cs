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
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a collection of all reservations.</returns>
        Task<IEnumerable<Reservation>> GetAllAsync();

        /// <summary>
        /// Retrieves a reservation by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the reservation.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the reservation if found; otherwise, null.</returns>
        Task<Reservation?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all reservations associated with a specific user's email asynchronously.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a collection of reservations for the specified user.</returns>
        Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email);

        /// <summary>
        /// Creates a new reservation asynchronously.
        /// </summary>
        /// <param name="dto">The data transfer object containing details for the new reservation.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the newly created reservation entity.</returns>
        Task<Reservation> CreateAsync(ReservationCreateDto dto);

        /// <summary>
        /// Updates an existing reservation asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the reservation to update.</param>
        /// <param name="dto">The data transfer object containing updated reservation details.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result is true if the update was successful; otherwise, false.</returns>
        Task<bool> UpdateAsync(int id, ReservationUpdateDto dto);

        /// <summary>
        /// Deletes a reservation by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the reservation to delete.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result is true if the deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Marks all items in a reservation as returned and updates inventory asynchronously.
        /// </summary>
        /// <param name="reservationId">The identifier of the reservation whose items should be returned.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result is true if the operation was successful; otherwise, false.</returns>
        Task<bool> ReturnItemsAsync(int reservationId);

        /// <summary>
        /// Creates a history record for a specific reservation asynchronously.
        /// </summary>
        /// <param name="reservationId">The identifier of the reservation to record history for.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result is true if the history record was created successfully; otherwise, false.</returns>
        Task<bool> CreateHistoryAsync(int reservationId);
    }
}