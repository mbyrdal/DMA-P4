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

        // Constructor: Inject the database context
        public ReservationRepository(ReservationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all reservations including their associated items asynchronously.
        /// </summary>
        /// <returns>A collection of all reservations, each including its items.</returns>
        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations.Include(r => r.Items).ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific reservation by its ID, including its items, asynchronously.
        /// Returns null if the reservation is not found.
        /// </summary>
        /// <param name="id">The ID of the reservation to retrieve.</param>
        /// <returns>The reservation with included items, or null if not found.</returns>
        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations.Include(r => r.Items).FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// Retrieves all reservations associated with the specified user email asynchronously.
        /// Each reservation includes its items.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>A collection of reservations associated with the specified email.</returns>
        public async Task<IEnumerable<Reservation>> GetByUserEmailAsync(string email)
        {
            return await _context.Reservations
                .Include(r => r.Items)
                .Where(r => r.Email == email)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new reservation and decrements the inventory for each reserved item asynchronously.
        /// Assumes the reservation entity is fully constructed by the service.
        /// Throws <see cref="InvalidOperationException"/> if there is insufficient inventory for any item.
        /// </summary>
        /// <param name="reservation">The reservation entity to create.</param>
        /// <returns>The created reservation entity.</returns>
        public async Task<Reservation> CreateAsync(Reservation reservation)
        {
            // Decrement inventory counts for reserved items
            foreach (var item in reservation.Items)
            {
                var existing = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.Equipment);
                if (existing == null || existing.Antal < item.Quantity)
                {
                    throw new InvalidOperationException($"Ikke nok af '{item.Equipment}' på lager.");
                }
                existing.Antal -= item.Quantity;
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        /// <summary>
        /// Updates an existing reservation and its items asynchronously.
        /// Uses RowVersion concurrency tokens to detect conflicting updates.
        /// Returns:
        /// -1 if a concurrency conflict occurs,
        /// 0 if the reservation is not found,
        /// otherwise the number of affected rows.
        /// Throws <see cref="ArgumentException"/> if any concurrency tokens are missing.
        /// </summary>
        /// <param name="id">The ID of the reservation to update.</param>
        /// <param name="updatedReservation">The reservation entity with updated data and concurrency tokens.</param>
        /// <returns>The number of affected rows or a status code indicating concurrency conflict or not found.</returns>
        public async Task<int> UpdateAsync(int id, Reservation updatedReservation)
        {
            if (updatedReservation.RowVersion == null)
            {
                throw new ArgumentException("Manlgende concurrency token (RowVersion) for update.");
            }

            var existingReservation = await _context.Reservations
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingReservation == null)
            {
                return 0; // Not found
            }

            // Set original concurrency token for reservation
            _context.Entry(existingReservation).OriginalValues["RowVersion"] = updatedReservation.RowVersion;

            // Update Reservation properties (status indicating activity and IsCollected whether items have been collected)
            existingReservation.Status = updatedReservation.Status;
            existingReservation.IsCollected = updatedReservation.IsCollected;

            // Update items - assuming updatedReservation.Items contains full updated items with concurrency tokens
            if (updatedReservation.Items != null && updatedReservation.Items.Any())
            {
                foreach (var updatedItem in updatedReservation.Items)
                {
                    var existingItem = existingReservation.Items.FirstOrDefault(i => i.Id == updatedItem.Id);
                    if (existingItem == null) continue;

                    if (updatedItem.RowVersion == null)
                    {
                        throw new ArgumentException($"Missing concurrency token for item with ID {updatedItem.Id}.");
                    }

                    // Set original concurrency token for item
                    _context.Entry(existingItem).OriginalValues["RowVersion"] = updatedItem.RowVersion;

                    // Update fields (e.g., Quantity, IsReturned)
                    existingItem.Quantity = updatedItem.Quantity;
                    existingItem.IsReturned = updatedItem.IsReturned;
                }
            }

            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return -1; // Concurrency conflict
            }
        }

        /// <summary>
        /// Deletes a reservation and returns reserved items to inventory asynchronously.
        /// Handles concurrency conflicts explicitly.
        /// Returns:
        /// -1 if a concurrency conflict occurs,
        /// 0 if the reservation is not found,
        /// otherwise the number of affected rows.
        /// </summary>
        /// <param name="id">The ID of the reservation to delete.</param>
        /// <returns>The number of affected rows or a status code indicating concurrency conflict or not found.</returns>
        public async Task<int> DeleteAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return 0; // Reservation not found
            }

            // Return reserved quantities back to inventory
            foreach (var item in reservation.Items)
            {
                var existing = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.Equipment);
                if (existing != null)
                {
                    existing.Antal += item.Quantity;
                }
            }

            _context.Reservations.Remove(reservation);

            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Concurrency conflict detected
                return -1;
            }
            catch (Exception)
            {
                // Rethrow unexpected exceptions
                throw;
            }
        }

        /// <summary>
        /// Marks all items in a reservation as returned and increments inventory accordingly asynchronously.
        /// Uses concurrency tokens to detect update conflicts.
        /// Sets the reservation status to "Inaktiv".
        /// Returns:
        /// -1 if a concurrency conflict occurs,
        /// 0 if the reservation or items are not found,
        /// otherwise the number of affected rows.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation whose items are being returned.</param>
        /// <returns>The number of affected rows or a status code indicating concurrency conflict or not found.</returns>
        public async Task<int> ReturnItemsAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null || !reservation.Items.Any())
            {
                return 0;
            }

            foreach (var item in reservation.Items)
            {
                if (!item.IsReturned)
                {
                    // Set concurrency token for item
                    _context.Entry(item).OriginalValues["RowVersion"] = item.RowVersion;

                    item.IsReturned = true; // Mark as returned

                    var equipment = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.Equipment);
                    if (equipment != null)
                    {
                        // Set concurrency token for equipment
                        _context.Entry(equipment).OriginalValues["RowVersion"] = equipment.RowVersion;
                        equipment.Antal += item.Quantity; // Increment inventory
                    }
                }
            }

            reservation.Status = "Inaktiv"; // Update reservation status

            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Concurrency conflict detected
                return -1;
            }
        }

        /// <summary>
        /// Creates a history record for the specified reservation asynchronously.
        /// Returns 0 if the reservation is not found,
        /// otherwise the number of affected rows.
        /// </summary>
        /// <param name="reservationId">The ID of the reservation for which to create history.</param>
        /// <returns>The number of affected rows or 0 if the reservation is not found.</returns>
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