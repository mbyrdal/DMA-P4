using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Repository for handling user login data access.
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
        /// Retrieves a user by their email address asynchronously.
        /// Returns null if no user is found with the specified email.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A Task containing the user if found; otherwise, null.</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // Query the Users DbSet for the first user matching the provided email
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}