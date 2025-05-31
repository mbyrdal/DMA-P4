using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;
using System.Threading.Tasks;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Repository implementation for user login data access.
    /// Provides methods to query user information related to authentication.
    /// </summary>
    public class LoginRepository : ILoginRepository
    {
        private readonly ReservationDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of <see cref="LoginRepository"/> with the specified database context.
        /// </summary>
        /// <param name="dbContext">The database context used for accessing user data.</param>
        public LoginRepository(ReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Asynchronously retrieves a user by their email address.
        /// Returns <c>null</c> if no user is found with the specified email.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A <see cref="Task{User}"/> representing the asynchronous operation,
        /// containing the user if found; otherwise, <c>null</c>.</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}