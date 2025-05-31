using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Repository interface for managing <see cref="User"/> entities.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves all users asynchronously.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Retrieves a user by their unique ID asynchronously.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user if found; otherwise, <c>null</c>.</returns>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new user asynchronously.
        /// </summary>
        /// <param name="user">The user entity to add.</param>
        /// <returns>The number of affected records, typically 1 if successful.</returns>
        Task<int> AddAsync(User user);

        /// <summary>
        /// Updates an existing user asynchronously with optimistic concurrency control.
        /// Requires the original <c>RowVersion</c> concurrency token.
        /// </summary>
        /// <param name="user">The user entity with updated data including the original concurrency token.</param>
        /// <returns>The number of affected records, typically 1 if successful.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the concurrency token (<c>RowVersion</c>) is missing.</exception>
        /// <exception cref="DbUpdateConcurrencyException">Thrown if a concurrency conflict is detected.</exception>
        Task<int> UpdateAsync(User user);

        /// <summary>
        /// Deletes a user by their ID asynchronously using optimistic concurrency control.
        /// Requires the original <c>RowVersion</c> concurrency token.
        /// </summary>
        /// <param name="id">The user ID to delete.</param>
        /// <param name="rowVersion">The original concurrency token for the user.</param>
        /// <returns>The number of affected records, or 0 if the user was not found.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the concurrency token (<c>RowVersion</c>) is missing.</exception>
        /// <exception cref="DbUpdateConcurrencyException">Thrown if a concurrency conflict is detected.</exception>
        Task<int> DeleteAsync(int id, byte[] rowVersion);

        /// <summary>
        /// Checks if a user exists by their ID asynchronously.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns><c>true</c> if the user exists; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync(int id);
    }
}