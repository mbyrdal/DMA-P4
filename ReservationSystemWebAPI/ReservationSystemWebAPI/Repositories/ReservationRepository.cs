using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ReservationDbContext _context;

        public ReservationRepository(ReservationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations.Include(r => r.Items).ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations.Include(r => r.Items).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email)
        {
            return await _context.Reservations
                .Include(r => r.Items)
                .Where(r => r.Email == email)
                .ToListAsync();
        }

        public async Task<Reservation> CreateAsync(ReservationDto dto)
        {
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
                }).ToList()
            };

            foreach (var item in reservation.Items)
            {
                var existing = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.Equipment);
                if (existing == null || existing.Antal < item.Quantity)
                    throw new InvalidOperationException($"Ikke nok af '{item.Equipment}' på lager.");

                existing.Antal -= item.Quantity;
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<bool> ConfirmAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null || reservation.Status == "Aktiv") return false;

            reservation.Status = "Aktiv";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsCollectedAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return false;

            reservation.IsCollected = true;
            reservation.Status = "Aktiv";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var res = await _context.Reservations.Include(r => r.Items).FirstOrDefaultAsync(r => r.Id == id);
            if (res == null) return false;

            foreach (var item in res.Items)
            {
                var existing = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.Equipment);
                if (existing != null)
                    existing.Antal += item.Quantity;
            }

            _context.Reservations.Remove(res);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReturnItemsAsync(int reservationId)
        {
            var reservation = await _context.Reservations.Include(r => r.Items).FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null || !reservation.Items.Any()) return false;

            foreach (var item in reservation.Items)
            {
                if (!item.IsReturned)
                {
                    item.IsReturned = true;
                    var equipment = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.Equipment);
                    if (equipment != null)
                        equipment.Antal += item.Quantity;
                }
            }

            reservation.Status = "Inaktiv";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateHistoryAsync(int reservationId)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null) return false;

            var history = new ReservationHistory
            {
                ReservationId = reservation.Id,
                Email = reservation.Email,
                CreatedAt = DateTime.Now,
                IsCollected = reservation.IsCollected
            };

            _context.ReservationHistory.Add(history);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return false;

            reservation.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
