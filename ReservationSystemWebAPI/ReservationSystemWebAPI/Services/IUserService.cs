using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<int> AddAsync(User user);
        Task<int> UpdateAsync(User user);
        Task<int> DeleteAsync(int id, string rowVersion);
    }
}
