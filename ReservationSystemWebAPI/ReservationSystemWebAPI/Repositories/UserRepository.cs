using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Repository responsible for performing data operations on the Users table.
    /// Encapsulates Entity Framework logic and abstracts database access.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ReservationDbContext _dbContext;

        public UserRepository(ReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves all users from the database asynchronously.
        /// </summary>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        /// <summary>
        /// Finds a single user by ID asynchronously.
        /// </summary>
        public async Task<User> GetByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        /// <summary>
        /// Adds a new user to the database and saves changes asynchronously.
        /// </summary>
        /// <param name="user">User object to add.</param>
        /// <returns>Number of state entries written to the database.</returns>
        public async Task<int> AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            int rows = await _dbContext.SaveChangesAsync();

            if (rows == 0)
                throw new InvalidOperationException("No changes were saved");

            return user.Id;
        }

        /// <summary>
        /// Updates an existing user in the database by attaching the modified entity
        /// and setting its state to Modified.
        /// </summary>
        /// <param name="user">The updated user object.</param>
        /// <returns>Number of state entries written to the database.</returns>
        /// <remarks>
        /// Entity Framework Core uses a change tracker. Here we attach the entity and tell EF it's modified,
        /// which causes all properties to be marked for update — even if only some were changed.
        /// </remarks>
        public async Task<int> UpdateAsync(User user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a user by ID from the database asynchronously.
        /// </summary>
        /// <param name="id">ID of the user to delete.</param>
        /// <returns>Number of state entries written to the database, or 0 if user was not found.</returns>
        public async Task<int> DeleteAsync(int id)
        {
            User existingUser = await _dbContext.Users.FindAsync(id);

            if (existingUser == null)
                return 0;

            _dbContext.Users.Remove(existingUser);
            int rows = await _dbContext.SaveChangesAsync();

            if (rows == 0)
                throw new InvalidOperationException("No user was deleted from the database.");

            return id;
        }

        /// <summary>
        /// Checks whether a user with the given ID exists in the database.
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == id);
        }
    }
}
