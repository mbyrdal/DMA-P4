using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.DTOs;

namespace ReservationSystemWebAPI.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation?> GetByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email);
        Task<Reservation> CreateAsync(ReservationDto dto);
        Task<bool> ConfirmAsync(int id);
        Task<bool> MarkAsCollectedAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> ReturnItemsAsync(int reservationId);
        Task<bool> CreateHistoryAsync(int reservationId);
        Task<bool> UpdateStatusAsync(int id, string status);
    }
}