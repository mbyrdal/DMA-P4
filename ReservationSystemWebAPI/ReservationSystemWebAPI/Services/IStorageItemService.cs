using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Services
{
    /// <summary>
    /// Service interface for managing storage items asynchronously, including CRUD operations.
    /// </summary>
    public interface IStorageItemService
    {
        /// <summary>
        /// Retrieves all storage items asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.
        /// The task result contains a collection of all storage items.</returns>
        Task<IEnumerable<StorageItem>> GetAllItemsAsync();

        /// <summary>
        /// Retrieves a specific storage item by its unique ID asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the storage item.</param>
        /// <returns>A task representing the asynchronous operation.
        /// The task result contains the storage item if found.</returns>
        Task<StorageItem> GetItemByIdAsync(int id);

        /// <summary>
        /// Adds a new storage item asynchronously.
        /// </summary>
        /// <param name="newItem">The data transfer object containing details of the new storage item.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddItemAsync(StorageItemCreateDto newItem);

        /// <summary>
        /// Updates an existing storage item asynchronously.
        /// </summary>
        /// <param name="updatedItem">The data transfer object containing updated storage item details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateItemAsync(StorageItemUpdateDto updatedItem);

        /// <summary>
        /// Deletes a storage item by its unique ID asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the storage item to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteItemAsync(int id);
    }
}