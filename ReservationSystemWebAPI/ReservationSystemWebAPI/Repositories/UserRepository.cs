using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Repository responsible for CRUD operations on the Users table.
    /// Contains only data access logic; does not throw exceptions.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ReservationDbContext _dbContext;

        public UserRepository(ReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        /// <summary>
        /// Adds a new user and returns the number of affected rows.
        /// </summary>
        public async Task<int> AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing user by attaching the entity and marking as modified.
        /// Returns the number of affected rows.
        /// </summary>
        public async Task<int> UpdateAsync(User user)
        {
            // Attach user entity to context
            _dbContext.Users.Attach(user);

            // Tell EF this entity is modified
            // This is necessary to ensure that EF tracks changes to the entity
            _dbContext.Entry(user).State = EntityState.Modified;

            // Set original RowVersion for concurrency check
            if(user.RowVersion == null)
            {
                // If RowVersion is null, we cannot perform concurrency checks
                throw new InvalidOperationException("Manglende concurrency token (RowVersion) for update.");
            }

            _dbContext.Entry(user).OriginalValues["RowVersion"] = user.RowVersion;

            // Save changes will throw DbUpdateConcurrencyException if RowVersion does not match (concurrency conflict)
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the user by id.
        /// Returns:
        /// - Number of affected rows if deleted
        /// - 0 if user not found
        /// </summary>
        public async Task<int> DeleteAsync(int id, byte[] rowVersion)
        {
            if(rowVersion == null)
            {
                throw new InvalidOperationException("Manglende concurrency token (RowVersion) for delete.");
            }

            // Create a stub user with just Id and RowVersion
            // No need to fetch entire user entity from database
            // DELETE FROM Users WHERE Id = @id AND RowVersion = @rowVersion
            var userToDelete = new User
            {
                Id = id,
                RowVersion = rowVersion
            };

            // Attach User with RowVersion for concurrency check
            _dbContext.Entry(userToDelete).State = EntityState.Deleted;
            _dbContext.Entry(userToDelete).OriginalValues["RowVersion"] = rowVersion;


            return await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == id);
        }
    }
}