using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.DTOs;

namespace ReservationSystemWebAPI.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation?> GetByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email);
        Task<Reservation> CreateAsync(ReservationCreateDto dto);
        Task<bool> UpdateAsync(int id, ReservationUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ReturnItemsAsync(int reservationId);
        Task<bool> CreateHistoryAsync(int reservationId);
    }
}