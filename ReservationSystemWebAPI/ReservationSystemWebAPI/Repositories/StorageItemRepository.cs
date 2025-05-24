using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    /// <summary>
    /// This class' main purpose is to handle data access logic when dealing with the database storage. <br />
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
            return await _dbContext.WEXO_DEPOT.ToListAsync();
        }

        public async Task<StorageItem?> GetByIdAsync(int id)
        {
            return await _dbContext.WEXO_DEPOT.FindAsync(id);
        }

        public async Task<int> AddAsync(StorageItem newItem)
        {
            await _dbContext.WEXO_DEPOT.AddAsync(newItem);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(StorageItem updateItem)
        {
            _dbContext.WEXO_DEPOT.Update(updateItem);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> RemoveAsync(int id)
        {
            var item = await _dbContext.WEXO_DEPOT.FindAsync(id);
            if (item == null) return 0;
            _dbContext.WEXO_DEPOT.Remove(item);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.WEXO_DEPOT.AnyAsync(e => e.Id == id);
        }
    }
}
