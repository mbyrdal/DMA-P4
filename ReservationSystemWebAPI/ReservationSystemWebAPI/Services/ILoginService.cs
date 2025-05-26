using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Services
{
    public interface ILoginService
    {
        Task<User> AuthenticateUserAsync(string email, string password);
    }
}
