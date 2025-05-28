using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.DTOs;

namespace ReservationSystemWebAPI.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation?> GetByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email);
        Task<Reservation> CreateAsync(ReservationCreateDto dto);
        Task<int> UpdateAsync(int id, ReservationUpdateDto dto);
        Task<int> DeleteAsync(int id);
        Task<int> ReturnItemsAsync(int reservationId);
        Task<int> CreateHistoryAsync(int reservationId);
    }
}
