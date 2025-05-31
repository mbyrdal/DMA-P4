using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Provides data access operations for <see cref="StorageItem"/> entities.
    /// Implements the <see cref="IStorageItemRepository"/> interface.
    /// Supports optimistic concurrency using the <c>RowVersion</c> property.
    /// </summary>
    public class StorageItemRepository : IStorageItemRepository
    {
        private readonly ReservationDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageItemRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The EF Core database context.</param>
        public StorageItemRepository(ReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves all storage items from the database asynchronously.
        /// </summary>
        /// <returns>A collection of all <see cref="StorageItem"/> entities.</returns>
        public async Task<IEnumerable<StorageItem>> GetAllAsync()
        {
            return await _dbContext.WEXO_DEPOT.ToListAsync();
        }

        /// <summary>
        /// Retrieves a storage item by its unique ID asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the storage item.</param>
        /// <returns>The matching <see cref="StorageItem"/> if found; otherwise, <c>null</c>.</returns>
        public async Task<StorageItem?> GetByIdAsync(int id)
        {
            return await _dbContext.WEXO_DEPOT.FindAsync(id);
        }

        /// <summary>
        /// Adds a new storage item to the database asynchronously.
        /// </summary>
        /// <param name="newItem">The <see cref="StorageItem"/> to add.</param>
        /// <returns>The number of affected rows, usually 1 if the operation is successful.</returns>
        public async Task<int> AddAsync(StorageItem newItem)
        {
            await _dbContext.WEXO_DEPOT.AddAsync(newItem);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing storage item with optimistic concurrency control.
        /// Uses the <c>RowVersion</c> concurrency token to detect update conflicts.
        /// </summary>
        /// <param name="updateItem">The <see cref="StorageItem"/> containing updated values and the original concurrency token.</param>
        /// <returns>The number of affected rows, usually 1 if the update succeeds.</returns>
        /// <exception cref="DbUpdateConcurrencyException">Thrown if a concurrency conflict is detected.</exception>
        public async Task<int> UpdateAsync(StorageItem updateItem)
        {
            // Attach entity so EF Core can track changes
            _dbContext.Attach(updateItem);

            // Set original RowVersion to detect concurrency conflicts
            _dbContext.Entry(updateItem).Property(si => si.RowVersion).OriginalValue = updateItem.RowVersion;

            // Mark the entire entity as modified
            _dbContext.Entry(updateItem).State = EntityState.Modified;

            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Propagate concurrency conflict to caller for handling
                throw;
            }
        }

        /// <summary>
        /// Deletes a storage item by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The ID of the storage item to delete.</param>
        /// <returns>The number of affected rows, or 0 if the item was not found.</returns>
        public async Task<int> DeleteAsync(int id)
        {
            var item = await _dbContext.WEXO_DEPOT.FindAsync(id);
            if (item == null)
                return 0;

            _dbContext.WEXO_DEPOT.Remove(item);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Checks asynchronously whether a storage item with the specified ID exists.
        /// </summary>
        /// <param name="id">The storage item ID to check.</param>
        /// <returns><c>true</c> if an item with the ID exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.WEXO_DEPOT.AnyAsync(e => e.Id == id);
        }
    }
}