using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Repository interface for managing user entities.
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
        /// <returns>The user if found; otherwise, null.</returns>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new user asynchronously.
        /// </summary>
        /// <param name="user">The user entity to add.</param>
        /// <returns>The number of affected records.</returns>
        Task<int> AddAsync(User user);

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="user">The user entity with updated data.</param>
        /// <returns>The number of affected records.</returns>
        Task<int> UpdateAsync(User user);

        /// <summary>
        /// Deletes a user by their ID asynchronously.
        /// </summary>
        /// <param name="id">The user ID to delete.</param>
        /// <returns>The number of affected records.</returns>
        Task<int> DeleteAsync(int id, byte[] rowVersion);

        /// <summary>
        /// Checks if a user exists by their ID asynchronously.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(int id);
    }
}