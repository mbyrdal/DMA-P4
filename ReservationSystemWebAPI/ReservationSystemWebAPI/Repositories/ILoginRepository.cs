using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Repository interface for handling user login related data operations.
    /// </summary>
    public interface ILoginRepository
    {
        /// <summary>
        /// Retrieves a user by their email address asynchronously.
        /// Returns null if no user is found with the specified email.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A Task containing the user if found; otherwise, null.</returns>
        Task<User?> GetUserByEmailAsync(string email);
    }
}