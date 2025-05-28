using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Repository for handling user login data access.
    /// </summary>
    public class LoginRepository : ILoginRepository
    {
        private readonly ReservationDbContext _dbContext;

        public LoginRepository(ReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // Fetch user by email from the database
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
