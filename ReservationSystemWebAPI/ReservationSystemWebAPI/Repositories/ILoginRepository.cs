using ReservationSystemWebAPI.Models;
using System.Threading.Tasks;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Repository interface for handling user login related data operations.
    /// </summary>
    public interface ILoginRepository
    {
        /// <summary>
        /// Asynchronously retrieves a user by their email address.
        /// Returns <c>null</c> if no user is found with the specified email.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A <see cref="Task{User}"/> representing the asynchronous operation,
        /// containing the user if found; otherwise, <c>null</c>.</returns>
        Task<User?> GetUserByEmailAsync(string email);
    }
}