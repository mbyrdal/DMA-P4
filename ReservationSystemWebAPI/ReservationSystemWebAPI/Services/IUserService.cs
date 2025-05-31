using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Services
{
    /// <summary>
    /// Interface defining asynchronous CRUD operations for user management.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves all users asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a collection of all users.</returns>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific user by their unique ID asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the user if found.</returns>
        Task<User> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new user asynchronously.
        /// </summary>
        /// <param name="user">The user object to add.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the ID of the newly added user.</returns>
        Task<int> AddAsync(User user);

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="user">The user object with updated information.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the number of affected records.</returns>
        Task<int> UpdateAsync(User user);

        /// <summary>
        /// Deletes a user by their unique ID asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <param name="rowVersion">Concurrency token as a Base64 string.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the number of affected records.</returns>
        Task<int> DeleteAsync(int id, string rowVersion);
    }
}