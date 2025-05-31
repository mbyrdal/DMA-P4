using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Repository responsible for CRUD operations on the Users table.
    /// Handles optimistic concurrency via <c>RowVersion</c> concurrency tokens.
    /// Throws exceptions on concurrency token absence or concurrency conflicts.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ReservationDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of <see cref="UserRepository"/>.
        /// </summary>
        /// <param name="dbContext">The EF Core database context.</param>
        public UserRepository(ReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves all users asynchronously.
        /// </summary>
        /// <returns>A collection of all <see cref="User"/> entities.</returns>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        /// <summary>
        /// Retrieves a user by ID asynchronously.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The matching <see cref="User"/> if found; otherwise, <c>null</c>.</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        /// <summary>
        /// Adds a new user asynchronously.
        /// </summary>
        /// <param name="user">The <see cref="User"/> entity to add.</param>
        /// <returns>The number of affected rows, typically 1 if successful.</returns>
        public async Task<int> AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing user asynchronously using optimistic concurrency control.
        /// Requires a valid <c>RowVersion</c> concurrency token.
        /// </summary>
        /// <param name="user">The <see cref="User"/> entity with updated data and original concurrency token.</param>
        /// <returns>The number of affected rows, typically 1 if successful.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <c>RowVersion</c> is <c>null</c>.</exception>
        /// <exception cref="DbUpdateConcurrencyException">Thrown if a concurrency conflict occurs.</exception>
        public async Task<int> UpdateAsync(User user)
        {
            // Attach the user entity to the context to track changes
            _dbContext.Users.Attach(user);

            // Mark entity as modified
            _dbContext.Entry(user).State = EntityState.Modified;

            // Verify concurrency token presence
            if (user.RowVersion == null)
                throw new InvalidOperationException("Missing concurrency token (RowVersion) for update.");

            // Set original RowVersion for concurrency check
            _dbContext.Entry(user).OriginalValues["RowVersion"] = user.RowVersion;

            // Save changes; may throw DbUpdateConcurrencyException if concurrency conflict detected
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a user by ID asynchronously using optimistic concurrency control.
        /// Requires the original <c>RowVersion</c> concurrency token.
        /// </summary>
        /// <param name="id">The user ID to delete.</param>
        /// <param name="rowVersion">The original concurrency token.</param>
        /// <returns>The number of affected rows if deleted, or 0 if user not found or token mismatch.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <c>RowVersion</c> is <c>null</c>.</exception>
        public async Task<int> DeleteAsync(int id, byte[] rowVersion)
        {
            if (rowVersion == null)
                throw new InvalidOperationException("Missing concurrency token (RowVersion) for delete.");

            // Retrieve the user entity from the database to ensure correct tracking and concurrency resolution
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return 0;

            // Compare the provided RowVersion with the one in the database
            if (!user.RowVersion.SequenceEqual(rowVersion))
                return 0;

            // Mark the entity for deletion
            _dbContext.Users.Remove(user);

            // Attempt to save changes and return the number of affected rows
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Checks asynchronously whether a user with the specified ID exists.
        /// </summary>
        /// <param name="id">The user ID to check.</param>
        /// <returns><c>true</c> if a user with the given ID exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Users.AnyAsync(u => u.Id == id);
        }
    }
}