using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Handles data access operations for <see cref="StorageItem"/> entities.
    /// Implements <see cref="IStorageItemRepository"/> contract.
    /// Optimistic concurrency control is implemented on update operations using a <c>RowVersion</c> concurrency token.
    /// </summary>
    public class StorageItemRepository : IStorageItemRepository
    {
        private readonly ReservationDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of <see cref="StorageItemRepository"/>.
        /// </summary>
        /// <param name="dbContext">The EF Core database context.</param>
        public StorageItemRepository(ReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves all storage items from the database.
        /// </summary>
        /// <returns>A list of all <see cref="StorageItem"/> entities.</returns>
        public async Task<IEnumerable<StorageItem>> GetAllAsync()
        {
            return await _dbContext.WEXO_DEPOT.ToListAsync();
        }

        /// <summary>
        /// Finds a storage item by its ID.
        /// </summary>
        /// <param name="id">The ID of the storage item.</param>
        /// <returns>The <see cref="StorageItem"/> with the specified ID, or <c>null</c> if not found.</returns>
        public async Task<StorageItem?> GetByIdAsync(int id)
        {
            return await _dbContext.WEXO_DEPOT.FindAsync(id);
        }

        /// <summary>
        /// Adds a new storage item to the database.
        /// </summary>
        /// <param name="newItem">The new <see cref="StorageItem"/> to add.</param>
        /// <returns>The number of affected rows (should be 1 if successful).</returns>
        public async Task<int> AddAsync(StorageItem newItem)
        {
            await _dbContext.WEXO_DEPOT.AddAsync(newItem);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing storage item with optimistic concurrency control.
        /// Uses the <c>RowVersion</c> property as a concurrency token to detect conflicts.
        /// </summary>
        /// <param name="updateItem">The storage item entity with updated values including the original <c>RowVersion</c>.</param>
        /// <returns>The number of affected rows (should be 1 if successful).</returns>
        /// <exception cref="DbUpdateConcurrencyException">Thrown if a concurrency conflict is detected.</exception>
        public async Task<int> UpdateAsync(StorageItem updateItem)
        {
            // Attach the entity so EF Core can track changes
            _dbContext.Attach(updateItem);

            // Set original RowVersion for concurrency check to detect conflicts
            _dbContext.Entry(updateItem).Property(si => si.RowVersion).OriginalValue = updateItem.RowVersion;

            // Mark entity as modified so EF Core issues update
            _dbContext.Entry(updateItem).State = EntityState.Modified;

            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Rethrow to let upper layers handle concurrency conflicts gracefully
                throw;
            }
        }

        /// <summary>
        /// Removes a storage item by its ID.
        /// </summary>
        /// <param name="id">The ID of the storage item to remove.</param>
        /// <returns>The number of affected rows (0 if the item was not found).</returns>
        public async Task<int> DeleteAsync(int id)
        {
            var item = await _dbContext.WEXO_DEPOT.FindAsync(id);
            if (item == null) return 0;

            _dbContext.WEXO_DEPOT.Remove(item);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if a storage item with the specified ID exists in the database.
        /// </summary>
        /// <param name="id">The ID to check for existence.</param>
        /// <returns><c>true</c> if an item with the ID exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.WEXO_DEPOT.AnyAsync(e => e.Id == id);
        }
    }
}