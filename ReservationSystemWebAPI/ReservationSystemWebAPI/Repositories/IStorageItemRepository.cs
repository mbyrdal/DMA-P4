using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// Defines repository operations for managing StorageItem entities.
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
        /// <returns>The <see cref="StorageItem"/> if found; otherwise, null.</returns>
        Task<StorageItem?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new storage item to the data source.
        /// </summary>
        /// <param name="newItem">The new <see cref="StorageItem"/> to add.</param>
        /// <returns>The number of affected rows.</returns>
        Task<int> AddAsync(StorageItem newItem);

        /// <summary>
        /// Updates an existing storage item.
        /// </summary>
        /// <param name="updateItem">The <see cref="StorageItem"/> with updated information.</param>
        /// <returns>The number of affected rows.</returns>
        Task<int> UpdateAsync(StorageItem updateItem);

        /// <summary>
        /// Deletes a storage item by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the storage item to remove.</param>
        /// <returns>The number of affected rows.</returns>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// Checks if a storage item exists by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the storage item.</param>
        /// <returns>True if the storage item exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(int id);
    }
}