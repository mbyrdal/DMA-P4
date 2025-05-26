using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.DTOs;

namespace ReservationSystemWebAPI.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation?> GetByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email);
        Task<Reservation> CreateAsync(ReservationDto dto);
        Task<bool> MarkAsCollectedAsync(int reservationId);
        Task<bool> ConfirmAsync(int id);
        Task<bool> ReturnItemsAsync(int reservationId);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> CreateHistoryAsync(int reservationId);
    }
}
