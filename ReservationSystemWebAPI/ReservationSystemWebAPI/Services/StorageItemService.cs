using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;

namespace ReservationSystemWebAPI.Services
{
    /// <summary>
    /// Implements <see cref="IStorageItemService"/> and handles business logic and error handling for storage item operations.
    /// </summary>
    public class StorageItemService : IStorageItemService
    {
        private readonly IStorageItemRepository _storageItemAccessPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageItemService"/> class with the specified repository.
        /// </summary>
        /// <param name="storageItemAccessPoint">The repository access point for storage items.</param>
        public StorageItemService(IStorageItemRepository storageItemAccessPoint)
        {
            _storageItemAccessPoint = storageItemAccessPoint;
        }

        /// <summary>
        /// Retrieves all storage items from the database asynchronously.
        /// Throws <see cref="ArgumentNullException"/> if no data is retrieved.
        /// Throws <see cref="InvalidOperationException"/> if the storage is empty.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a collection of storage items.</returns>
        /// <exception cref="ArgumentNullException">Thrown when retrieved data is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no storage items exist.</exception>
        public async Task<IEnumerable<StorageItem>> GetAllItemsAsync()
        {
            var storageItems = await _storageItemAccessPoint.GetAllAsync();

            if (storageItems == null)
                throw new ArgumentNullException(nameof(storageItems), "Fejl ved henting af data om lagerstatus. Tjek venligst forbindelsen til databasen.");

            if (!storageItems.Any())
                throw new InvalidOperationException("Der er intet udstyr på lager. Tjek venligst lagerstatus.");

            return storageItems;
        }

        /// <summary>
        /// Retrieves a specific storage item by its ID asynchronously.
        /// Throws <see cref="KeyNotFoundException"/> if the item does not exist.
        /// </summary>
        /// <param name="id">The unique identifier of the storage item.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the storage item.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the storage item with the given ID is not found.</exception>
        public async Task<StorageItem> GetItemByIdAsync(int id)
        {
            var storageItem = await _storageItemAccessPoint.GetByIdAsync(id);
            if (storageItem == null)
            {
                throw new KeyNotFoundException($"Der opstod en fejl under hentning af udstyr med ID {id}.\nTjek venligst forbindelsen til databasen og evt. ID match.");
            }
            return storageItem;
        }

        /// <summary>
        /// Adds a new storage item asynchronously.
        /// Throws <see cref="ArgumentNullException"/> if the input DTO is null.
        /// Throws <see cref="ArgumentException"/> if the name property is null or whitespace.
        /// Throws <see cref="InvalidOperationException"/> if an error occurs during the add operation.
        /// </summary>
        /// <param name="storageItem">The data transfer object representing the new storage item.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="storageItem"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <see cref="StorageItemCreateDto.Navn"/> is null or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown if adding the storage item fails.</exception>
        public async Task AddItemAsync(StorageItemCreateDto storageItem)
        {
            if (storageItem == null)
                throw new ArgumentNullException(nameof(storageItem), "Udstyret er NULL/findes ikke. Tjek venligst input data.");

            if (string.IsNullOrWhiteSpace(storageItem.Navn))
                throw new ArgumentException("Navnet på udstyret er påkrævet. Tjek venligst efter whitespace, mellemrum osv.", nameof(storageItem.Navn));

            var storageItemDtoMapper = new StorageItem
            {
                Id = storageItem.Id,
                Navn = storageItem.Navn,
                Antal = storageItem.Antal,
                Reol = storageItem.Reol,
                Hylde = storageItem.Hylde,
                Kasse = storageItem.Kasse,
                RowVersion = new byte[8] // Initialized default, will be set by EF on insert
            };

            try
            {
                await _storageItemAccessPoint.AddAsync(storageItemDtoMapper);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Der opstod en fejl under tilføjelse af udstyr til lageret. Tjek venligst forbindelsen til databasen og prøv igen.", ex);
            }
        }

        /// <summary>
        /// Updates an existing storage item asynchronously.
        /// Throws <see cref="KeyNotFoundException"/> if the storage item does not exist.
        /// Throws <see cref="InvalidOperationException"/> if a concurrency conflict or other update error occurs.
        /// </summary>
        /// <param name="dto">The data transfer object containing updated storage item details.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the storage item does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown if concurrency conflict or other update failure occurs.</exception>
        public async Task UpdateItemAsync(StorageItemUpdateDto dto)
        {
            var itemExists = await _storageItemAccessPoint.ExistsAsync(dto.Id);
            if (!itemExists)
                throw new KeyNotFoundException($"Kan ikke opdatere udstyret med ID {dto.Id}. Udstyret findes ikke.");

            var storageItemDtoMapper = new StorageItem
            {
                Id = dto.Id,
                Navn = dto.Navn,
                Antal = dto.Antal,
                Reol = dto.Reol,
                Hylde = dto.Hylde,
                Kasse = dto.Kasse,
                RowVersion = Convert.FromBase64String(dto.RowVersion)
            };

            try
            {
                await _storageItemAccessPoint.UpdateAsync(storageItemDtoMapper);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Opdatering mislykkedes, fordi en anden bruger har ændret på lageret siden da. Refresh og prøv igen.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Der opstod en fejl under opdatering af udstyr i lageret. Tjek venligst forbindelsen til databasen og prøv igen.", ex);
            }
        }

        /// <summary>
        /// Deletes a specific storage item by its ID asynchronously.
        /// Throws <see cref="KeyNotFoundException"/> if the item does not exist.
        /// Throws <see cref="InvalidOperationException"/> if the deletion operation fails.
        /// </summary>
        /// <param name="id">The unique identifier of the storage item to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the storage item does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the deletion fails.</exception>
        public async Task DeleteItemAsync(int id)
        {
            var exists = await _storageItemAccessPoint.ExistsAsync(id);
            if (!exists)
                throw new KeyNotFoundException($"Kan ikke slette udstyret med ID {id}, da den ikke findes.");

            try
            {
                var affectedRows = await _storageItemAccessPoint.DeleteAsync(id);
                if (affectedRows == 0)
                    throw new InvalidOperationException($"Ingen udstyr blev slettet med ID {id}. Tjek venligst forbindelsen til databasen og prøv igen.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Der opstod en fejl under sletning af udstyr fra lageret. Tjek venligst forbindelsen til databasen og prøv igen.", ex);
            }
        }
    }
}