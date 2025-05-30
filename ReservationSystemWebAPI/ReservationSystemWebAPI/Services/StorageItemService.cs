using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;

namespace ReservationSystemWebAPI.Services
{
    /// <summary>
    /// This class' main purpose is to handle error handling and business logic applied to the system. <br />
    /// A contract (interface) IStorageItemService is used to define the methods that this class implements. <br />
    /// </summary>
    public class StorageItemService : IStorageItemService
    {
        private readonly IStorageItemRepository _storageItemAccessPoint;

        public StorageItemService(IStorageItemRepository storageItemAccessPoint)
        {
            _storageItemAccessPoint = storageItemAccessPoint;
        }

        /// <summary>
        /// Henter alt lagerudstyr fra databasen asynkront.
        /// </summary>
        public async Task<IEnumerable<StorageItem>> GetAllItemsAsync()
        {
            var storageItems = await _storageItemAccessPoint.GetAllAsync();

            if(storageItems == null)
            {
                throw new ArgumentNullException(nameof(storageItems), "Fejl ved henting af data om lagerstatus. Tjek venligst forbindelsen til databasen.");
            }

            if (!storageItems.Any())
            {
                throw new InvalidOperationException("Der er intet udstyr på lager. Tjek venligst lagerstatus.");
            }

            return storageItems;
        }

        /// <summary>
        /// Hentet et specifikt udstyr fra lageret baseret på dets ID asynkront.<br/>
        /// </summary>
        /// <param name="id">Identifier (ID) på udstyr i databasen.</param>
        public async Task<StorageItem> GetItemByIdAsync(int id)
        {
            var storageItem = await _storageItemAccessPoint.GetByIdAsync(id);
            if (storageItem == null)
            {
                throw new KeyNotFoundException($"Der opstod en fejl under hentning af udstyr med ID {id}.\n" +
                                               $"Tjek venligst forbindelsen til databasen og evt. ID match.");
            }
            return storageItem;
        }

        /// <summary>
        /// Tilføjer nyt udstyr til lageret asynkront.
        /// </summary>
        /// <param name="storageItem">Udstyr (StorageItem) objekt, som skal tilføjes.</param>
        public async Task AddItemAsync(StorageItem storageItem)
        {

            if(storageItem == null)
            {
                throw new ArgumentNullException(nameof(storageItem), "Udstyret er NULL/findes ikke. Tjek venligst input data.");
            }

            if (string.IsNullOrWhiteSpace(storageItem.Navn))
            {
                throw new ArgumentException("Navnet på udstyret er påkrævet. ", nameof(storageItem.Navn));
            }

            try
            {
                await _storageItemAccessPoint.AddAsync(storageItem);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Der opstod en fejl under tilføjelse af udstyr til lageret. " +
                                                    "Tjek venligst forbindelsen til databasen og prøv igen.", ex);
            }
        }

        /// <summary>
        /// Opdaterer eksisterende ustyr i lageret asynkront.
        /// </summary>
        /// <param name="storageItem">Eksisterende udstyr (StorageItem) objekt, som skal opdateres.</param>
        public async Task UpdateItemAsync(StorageItem storageItem)
        {
            var itemExists = await _storageItemAccessPoint.ExistsAsync(storageItem.Id);

            if(!itemExists)
            {
                throw new KeyNotFoundException($"Kan ikke opdatere udstyret med ID {storageItem.Id}. Udstyret {storageItem.Navn} findes ikke.");
            }

            try
            {
                await _storageItemAccessPoint.UpdateAsync(storageItem);
            }

            // The RowVersion concurrency token is checked; if it doesn't match the DB, a DbUpdateConcurrencyException is thrown.
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency conflict
                throw new InvalidOperationException("Opdatering mislykkedes, fordi en anden bruger har ændret på lageret siden da. Refresh og prøv igen.");
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Der opstod en fejl under opdatering af udstyr i lageret. " +
                                                    "Tjek venligst forbindelsen til databasen og prøv igen.", ex);
            }
        }

        /// <summary>
        /// Sletter et specifikt udstyr fra lageret asynkront.
        /// </summary>
        /// <param name="id">Identifier (ID) på udstyr i databasen.</param>
        public async Task DeleteItemAsync(int id)
        {
            var exists = await _storageItemAccessPoint.ExistsAsync(id);

            if (!exists)
            {
                throw new KeyNotFoundException($"Kan ikke slette udstyret med ID {id}, da den ikke findes.");
            }

            try
            {
                var affectedRows = await _storageItemAccessPoint.DeleteAsync(id);
                if (affectedRows == 0)
                {
                    throw new InvalidOperationException($"Ingen udstyr blev slettet med ID {id}. " +
                                                        "Tjek venligst forbindelsen til databasen og prøv igen.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Der opstod en fejl under sletning af udstyr fra lageret. " +
                                                    "Tjek venligst forbindelsen til databasen og prøv igen.", ex);
            }
        }
    }

}
