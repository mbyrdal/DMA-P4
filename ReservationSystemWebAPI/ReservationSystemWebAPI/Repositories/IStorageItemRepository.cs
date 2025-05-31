using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Defines repository operations for managing <see cref="StorageItem"/> entities.
    /// </summary>
    public interface IStorageItemRepository
    {
        /// <summary>
        /// Retrieves all storage items.
        /// </summary>
        /// <returns>A list of all <see cref="StorageItem"/> objects.</returns>
        Task<IEnumerable<StorageItem>> GetAllAsync();

        /// <summary>
        /// Retrieves a storage item by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the storage item.</param>
        /// <returns>The <see cref="StorageItem"/> if found; otherwise, <c>null</c>.</returns>
        Task<StorageItem?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new storage item to the data source.
        /// </summary>
        /// <param name="newItem">The new <see cref="StorageItem"/> to add.</param>
        /// <returns>The number of affected rows, typically 1 if successful.</returns>
        Task<int> AddAsync(StorageItem newItem);

        /// <summary>
        /// Updates an existing storage item with optimistic concurrency control.
        /// The <c>RowVersion</c> property is used as a concurrency token.
        /// </summary>
        /// <param name="updateItem">The <see cref="StorageItem"/> with updated information including the original concurrency token.</param>
        /// <returns>The number of affected rows, typically 1 if successful.</returns>
        /// <exception cref="DbUpdateConcurrencyException">Thrown if a concurrency conflict occurs.</exception>
        Task<int> UpdateAsync(StorageItem updateItem);

        /// <summary>
        /// Deletes a storage item by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the storage item to remove.</param>
        /// <returns>The number of affected rows, or 0 if the item was not found.</returns>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// Checks if a storage item exists by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the storage item.</param>
        /// <returns><c>true</c> if the storage item exists; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync(int id);
    }
}