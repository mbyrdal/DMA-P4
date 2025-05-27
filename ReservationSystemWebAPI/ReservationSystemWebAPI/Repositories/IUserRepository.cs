using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<int> AddAsync(User user);
        Task<int> UpdateAsync(User user);
        Task<int> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
