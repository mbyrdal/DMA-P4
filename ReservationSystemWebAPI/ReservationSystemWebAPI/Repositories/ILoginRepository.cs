using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    public interface ILoginRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
    }
}
