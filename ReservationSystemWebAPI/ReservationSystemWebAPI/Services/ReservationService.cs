using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;

namespace ReservationSystemWebAPI.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repository;

        public ReservationService(IReservationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            var reservations = await _repository.GetAllAsync();
            if (reservations == null || !reservations.Any())
            {
                throw new InvalidOperationException("Ingen reservationer fundet.");
            }
            return reservations;
        }

        public async Task<Reservation> GetByIdAsync(int id)
        {
            var reservation = await _repository.GetByIdAsync(id);
            if (reservation == null)
            {
                throw new KeyNotFoundException($"Reservation med ID {id} blev ikke fundet.");
            }
            return reservation;
        }

        public async Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email)
        {
            var reservations = await _repository.GetByUserEmailAsync(email);
            if (reservations == null || !reservations.Any())
            {
                throw new InvalidOperationException("Ingen reservationer fundet for den angivne bruger.");
            }
            return reservations;
        }


        public async Task<Reservation> CreateAsync(ReservationDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || dto.Items == null || !dto.Items.Any())
            {
                throw new ArgumentException("Ugyldig reservation.");
            }
            return await _repository.CreateAsync(dto);
        }

        public async Task<bool> ConfirmAsync(int id)
        {
            return await _repository.ConfirmAsync(id);
        }

        public async Task<bool> MarkAsCollectedAsync(int id)
        {
            return await _repository.MarkAsCollectedAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ReturnItemsAsync(int reservationId)
        {
            return await _repository.ReturnItemsAsync(reservationId);
        }

        public async Task<bool> CreateHistoryAsync(int reservationId)
        {
            return await _repository.CreateHistoryAsync(reservationId);
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status kan ikke være tom.", nameof(status));
            }
            return await _repository.UpdateStatusAsync(id, status);
        }
    }
}
