using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// This class' main purpose is to handle error handling and business logic applied to the system. <br />
    /// A contract (interface) IStorageItemRepository is used to define the methods that this class implements. <br />
    /// </summary>
    public class StorageItemRepository : IStorageItemRepository
    {
        private readonly ReservationDbContext _dbContext;

        public StorageItemRepository(ReservationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<StorageItem>> GetAllAsync()
        {
            try
            {
                // Fetch database items asynchronously and handle cases.
                List<StorageItem> items = await _dbContext.WEXO_DEPOT.ToListAsync();

                if(items == null || !items.Any())
                {
                    // In either scenario, return an empty list.
                    return new List<StorageItem>();
                }

                // Return non-empty, non-NULL list.
                return items;
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("The list of storage items is NULL.\n" +
                                                "Please validate your connection to the database.", ex);
            }
        }

        public async Task<StorageItem?> GetByIdAsync(int id)
        {
            try
            {
                // Fetch item by ID asynchronously. Possibly NULL if non-existant.
                StorageItem? item = await _dbContext.WEXO_DEPOT.FindAsync(id);

                if (item == null)
                {
                    // Item not found, return NULL.
                    return null;
                }
                return item;
            }
            catch (KeyNotFoundException ex)
            { 
                throw new KeyNotFoundException($"An error occurred while retrieving the item with ID: {id}. It is NULL or does not exist.\n" +
                                               $"Please validate your connection to the database.", ex);
            }
        }

        // Add a new item asynchronously
        public async Task<int> AddAsync(StorageItem newItem)
        {
            try
            {
                await _dbContext.WEXO_DEPOT.AddAsync(newItem);
                return await _dbContext.SaveChangesAsync();  // Returns number of affected rows
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding a new item.", ex);
            }
        }

        // Update an existing item asynchronously
        public async Task<int> UpdateAsync(StorageItem updateItem)
        {
            try
            {
                _dbContext.WEXO_DEPOT.Update(updateItem);
                return await _dbContext.SaveChangesAsync();  // Returns number of affected rows
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the item.", ex);
            }
        }

        // Remove an item by ID asynchronously
        public async Task<int> RemoveAsync(int id)
        {
            try
            {
                var item = await _dbContext.WEXO_DEPOT.FindAsync(id);
                if (item == null)
                {
                    return 0;  // Return 0 if the item doesn't exist
                }

                _dbContext.WEXO_DEPOT.Remove(item);
                return await _dbContext.SaveChangesAsync();  // Returns number of affected rows
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the item with ID {id}.", ex);
            }
        }

        // Check if an item exists by ID asynchronously
        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _dbContext.WEXO_DEPOT.AnyAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking if the item exists.", ex);
            }
        }
    }
}
