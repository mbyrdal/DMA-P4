using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Services
{
    public interface IStorageItemService
    {
        Task<IEnumerable<StorageItem>> GetAllItemsAsync();
        Task<StorageItem> GetItemByIdAsync(int id);
        Task AddItemAsync(StorageItemCreateDto newItem);
        Task UpdateItemAsync(StorageItemUpdateDto updatedItem);
        Task DeleteItemAsync(int id);
    }
}
