using ReservationSystemWebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Repository interface for managing reservation data access operations.
/// Provides methods to create, retrieve, update, delete, and track reservation histories.
/// </summary>
public interface IReservationRepository
{
    /// <summary>
    /// Retrieves all reservations asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The result contains all reservations.</returns>
    Task<IEnumerable<Reservation>> GetAllAsync();

    /// <summary>
    /// Retrieves a reservation by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the reservation.</param>
    /// <returns>A task representing the asynchronous operation. The result contains the reservation if found; otherwise, null.</returns>
    Task<Reservation?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves all reservations associated with a specific user email asynchronously.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>A task representing the asynchronous operation. The result contains reservations for the specified user.</returns>
    Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email);

    /// <summary>
    /// Creates a new reservation asynchronously.
    /// </summary>
    /// <param name="reservation">The reservation entity to create.</param>
    /// <returns>A task representing the asynchronous operation. The result contains the created reservation.</returns>
    Task<Reservation> CreateAsync(Reservation reservation);

    /// <summary>
    /// Updates an existing reservation identified by its ID asynchronously.
    /// Returns:
    /// -1 if concurrency conflict,
    /// 0 if reservation not found,
    /// otherwise number of updated records.
    /// </summary>
    /// <param name="id">The unique identifier of the reservation to update.</param>
    /// <param name="reservation">The updated reservation entity.</param>
    /// <returns>A task representing the asynchronous operation. The result contains status or affected rows.</returns>
    Task<int> UpdateAsync(int id, Reservation reservation);

    /// <summary>
    /// Deletes a reservation by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the reservation to delete.</param>
    /// <returns>A task representing the asynchronous operation. The result contains the number of affected rows.</returns>
    Task<int> DeleteAsync(int id);

    /// <summary>
    /// Marks all items in a reservation as returned asynchronously and updates inventory.
    /// Returns:
    /// -1 if concurrency conflict,
    /// 0 if reservation or items not found,
    /// otherwise number of updated records.
    /// </summary>
    /// <param name="reservationId">The unique identifier of the reservation.</param>
    /// <returns>A task representing the asynchronous operation. The result contains status or affected rows.</returns>
    Task<int> ReturnItemsAsync(int reservationId);

    /// <summary>
    /// Creates a history record for a reservation asynchronously.
    /// </summary>
    /// <param name="reservationId">The unique identifier of the reservation to archive.</param>
    /// <returns>A task representing the asynchronous operation. The result contains the number of affected rows.</returns>
    Task<int> CreateHistoryAsync(int reservationId);
}