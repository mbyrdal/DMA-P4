using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Services
{
    /// <summary>
    /// Defines authentication-related operations for users.
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Authenticates a user by verifying email and password.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's plaintext password.</param>
        /// <returns>The authenticated <see cref="User"/> if credentials are valid.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no user is found with the specified email.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if the password is incorrect.</exception>
        Task<User> AuthenticateUserAsync(string email, string password);
    }
}