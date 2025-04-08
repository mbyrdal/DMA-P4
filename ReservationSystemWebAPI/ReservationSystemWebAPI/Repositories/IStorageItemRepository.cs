using ReservationSystemWebAPI.Models;

public interface IStorageItemRepository
{
    Task<IEnumerable<StorageItem>> GetAllAsync();
    Task<StorageItem?> GetByIdAsync(int id);
    Task<int> AddAsync(StorageItem newItem);
    Task<int> UpdateAsync(StorageItem updateItem);
    Task<int> RemoveAsync(int id);
    Task<bool> ExistsAsync(int id);
}
