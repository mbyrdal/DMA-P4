using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using System.Data;

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

        public async Task<Reservation> CreateAsync(ReservationCreateDto dto)
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

            // Decrement inventory for each reserved item
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

        public async Task<int> UpdateAsync(int id, ReservationUpdateDto dto)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return 0;
            }

            // Set original RowVersion for concurrency check (modified to proper syntax)
            _context.Entry(reservation).OriginalValues["RowVersion"] = dto.RowVersion;

            // Update reservation properties
            if (dto.Status != null)
            {
                reservation.Status = dto.Status;
            }

            if (dto.IsCollected.HasValue)
            {
                reservation.IsCollected = dto.IsCollected.Value;
            }

            // Update reservation items if provided
            if (dto.Items != null)
            {
                foreach (var itemDto in dto.Items)
                {
                    var item = reservation.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                    if (item == null) continue;

                    // Set concurrency token for item
                    _context.Entry(item).OriginalValues["RowVersion"] = itemDto.RowVersion;

                    // Update quantity and other fields
                    item.Quantity = itemDto.Quantity;
                }
            }

            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Return -1 to indicate concurrency conflict (instead of 0)
                return -1;
            }
            catch (Exception ex)
            {
                // Rethrow unexpected exceptions
                throw;
            }
        }

        /*
        public async Task<int> UpdateAsync(int id, ReservationUpdateDto dto)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return 0;
            }

            // Set original RowVersion for concurrency check on Reservation
            _context.Entry(reservation).Property("RowVersion").OriginalValue = dto.RowVersion;

            // Update reservation properties
            if (dto.Status != null)
            {
                reservation.Status = dto.Status;
            }

            if (dto.IsCollected.HasValue)
            {
                reservation.IsCollected = dto.IsCollected.Value;
            }

            // Update reservation items if provided
            if (dto.Items != null)
            {
                foreach (var itemDto in dto.Items)
                {
                    var item = reservation.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                    if (item == null) continue;

                    // Set original RowVersion for concurrency check on the ReservationItem
                    _context.Entry(item).Property("RowVersion").OriginalValue = itemDto.RowVersion;

                    // Update quantity and other fields
                    item.Quantity = itemDto.Quantity;
                }
            }

            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency exception - return 0 to indicate concurrency conflict
                // throw new InvalidOperationException("Der opstod en konflikt ved opdatering af reservationen. Genindlæs venligst siden og prøv igen.");
                return 0;
            }
            catch (Exception ex)
            {
                // Rethrow unexpected exceptions (or wrap them)
                throw;
            }
        }
        */

        public async Task<int> DeleteAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return 0; // Reservation not found
            }

            // Return items to inventory (WEXO DEPOT)
            foreach (var item in reservation.Items)
            {
                var existing = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.Equipment);
                if (existing != null)
                {
                    existing.Antal += item.Quantity; // Return the quantity to inventory
                }
            }

            _context.Reservations.Remove(reservation);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> ReturnItemsAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null || !reservation.Items.Any())
            {
                return 0;
            }

            // Add concurrency tracking for items
            var itemVersions = new Dictionary<int, byte[]>();

            foreach (var item in reservation.Items)
            {
                if (!item.IsReturned)
                {
                    // Store current rowVersion before updating
                    itemVersions[item.Id] = item.RowVersion;

                    item.IsReturned = true; // Mark item as returned

                    var equipment = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.Equipment);
                    if (equipment != null)
                    {
                        // Set item version before saving
                        _context.Entry(equipment).OriginalValues["RowVersion"] = equipment.RowVersion;
                        equipment.Antal += item.Quantity; // Return the quantity to inventory
                    }
                }
            }

            reservation.Status = "Inaktiv"; // Set status to inactive
            
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                // Return -1 to indicate concurrency conflict
                return -1;
            }
        }

        public async Task<int> CreateHistoryAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
            {
                return 0; // Reservation not found
            }

            var history = new ReservationHistory
            {
                ReservationId = reservation.Id,
                Email = reservation.Email,
                CreatedAt = reservation.CreatedAt,
                IsCollected = reservation.IsCollected
            };

            _context.ReservationHistory.Add(history);
            return await _context.SaveChangesAsync();
        }
    }
}
