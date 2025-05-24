using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Services
{
    public interface IStorageItemService
    {
        Task<IEnumerable<StorageItem>> GetAllItemsAsync();
        Task<StorageItem> GetItemByIdAsync(int id);
        Task AddItemAsync(StorageItem newItem);
        Task UpdateItemAsync(StorageItem updatedItem);
        Task DeleteItemAsync(int id);
    }
}
