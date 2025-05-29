using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;

namespace ReservationSystemWebAPI.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repository;
        public enum UpdateStatus { Success, NoChanges, NotFound, ConcurrencyConflict }

        public ReservationService(IReservationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            var reservations = await _repository.GetAllAsync();
            if (reservations == null || !reservations.Any())
                throw new InvalidOperationException("Ingen reservationer fundet.");
            return reservations;
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            var reservation = await _repository.GetByIdAsync(id);
            if (reservation == null)
                throw new KeyNotFoundException($"Reservation med ID {id} blev ikke fundet.");
            return reservation;
        }

        public async Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email)
        {
            var reservations = await _repository.GetByUserEmailAsync(email);
            if (reservations == null || !reservations.Any())
                throw new InvalidOperationException("Ingen reservationer fundet for den angivne bruger.");
            return reservations;
        }

        public async Task<Reservation> CreateAsync(ReservationCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || dto.Items == null || !dto.Items.Any())
            {
                throw new ArgumentException("Ugyldige data for reservation. Sørg for at email og items er angivet korrekt.");
            }
                
            // Create reservation entity using DTO
            var reservation = new Reservation
            {
                Email = dto.Email,
                Status = dto.Status,
                CreatedAt = DateTime.Now,
                Items = dto.Items.Select(i => new ReservationItems
                {
                    Equipment = i.Equipment,
                    Quantity = i.Quantity,
                    IsReturned = false
                }).ToList(),
                IsCollected = false
            };

            // Using repository, create the reservation
            return await _repository.CreateAsync(dto);
        }

        public async Task<bool> UpdateAsync(int id, ReservationUpdateDto dto)
        {
            var updatedCount = await _repository.UpdateAsync(id, dto);

            // Handle return codes
            if (updatedCount > 0) return true;
            if (updatedCount == 0) throw new KeyNotFoundException($"Reservation med ID {id} blev ikke fundet!");
            if (updatedCount < 0) throw new DbUpdateConcurrencyException("Der opstod en konflikt ved opdatering af reservation. Tjek venligst at data er korrekte og prøv igen.");

            throw new InvalidOperationException("Uventet resultat ved opdatering af reservation.");

            /*
            var updatedCount = await _repository.UpdateAsync(id, dto);
            if (updatedCount == 0)
                throw new InvalidOperationException("Opdatering mislykkedes eller reservationen blev ikke fundet.");
            return true;
            */
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var affected = await _repository.DeleteAsync(id);
            if (affected == 0)
                throw new KeyNotFoundException($"Reservation med ID {id} blev ikke fundet.");
            return true;
        }

        public async Task<bool> ReturnItemsAsync(int reservationId)
        {
            var affected = await _repository.ReturnItemsAsync(reservationId);

            // Handle return codes
            if (affected > 0) return true;
            if (affected == 0) throw new KeyNotFoundException($"Reservation med ID {reservationId} blev ikke fundet eller har intet tilknyttet udstyr.");
            if (affected < 0) throw new DbUpdateConcurrencyException("Der opstod en konflikt ved returnering af items. Tjek venligst at data er korrekte og prøv igen.");

            throw new InvalidOperationException("Uventet resultat ved returnering af items.");

            /*
            var affected = await _repository.ReturnItemsAsync(reservationId);
            if (affected == 0)
                throw new KeyNotFoundException($"Reservation med ID {reservationId} blev ikke fundet eller har ingen items.");
            return true;
            */
        }

        public async Task<bool> CreateHistoryAsync(int reservationId)
        {
            var affected = await _repository.CreateHistoryAsync(reservationId);
            if (affected == 0)
                throw new KeyNotFoundException($"Reservation med ID {reservationId} blev ikke fundet.");
            return true;
        }
    }
}