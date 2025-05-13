using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.DTOs;

namespace ReservationSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationDbContext _context;

        public ReservationController(ReservationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetAll()
        {
            return await _context.Reservations.Include(r => r.Items).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetById(int id)
        {
            var res = await _context.Reservations.Include(r => r.Items).FirstOrDefaultAsync(r => r.Id == id);
            if (res == null) return NotFound();
            return res;
        }

        [HttpGet("user/{email}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetByUser(string email)
        {
            return await _context.Reservations
                .Where(r => r.Email == email)
                .Include(r => r.Items)
                .ToListAsync();
        }


        [HttpPost]
        public async Task<ActionResult<Reservation>> Create(ReservationDto dto)
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
                {
                    return BadRequest($"Ikke nok af '{item.Equipment}' på lager.");
                }

                existing.Antal -= item.Quantity;
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        }

        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmReservation(int id)
        {

            var res = await _context.Reservations.FindAsync(id);
            if (res == null) return NotFound();
            if (res.Status == "Aktiv")
            {
                return BadRequest("Fejl. Reservation er allerede aktiv.");
            }
            res.Status = "Aktiv";

            await _context.SaveChangesAsync();

            return Ok("Success. Reservationen er nu Aktiv.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _context.Reservations.Include(r => r.Items).FirstOrDefaultAsync(r => r.Id == id);
            if (res == null) return NotFound();

            foreach (var item in res.Items)
            {
                var existing = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.Equipment);
                if (existing != null)
                {
                    existing.Antal += item.Quantity;
                }
            }

            _context.Reservations.Remove(res);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("markcollected/{id}")]
        public async Task<IActionResult> ConfirmCollection(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            reservation.IsCollected = true;
            reservation.Status = "Aktiv";

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpPatch("{reservationId}/return/{itemId}")]
        public async Task<IActionResult> ReturnItem(int reservationId, int itemId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
                return NotFound("Reservation not found.");

            var item = reservation.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return NotFound("Item not found in reservation.");

            item.IsReturned = true;

            var equipment = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.Equipment);
            if (equipment != null)
            {
                equipment.Antal += item.Quantity;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("createHistory/{reservationId}")]
        public async Task<IActionResult> CreateHistory(int reservationId)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null)
                return NotFound("Reservation not found.");

            var history = new ReservationHistory
            {
                ReservationId = reservation.Id,
                Email = reservation.Email,
                CreatedAt = DateTime.Now,
                IsCollected = reservation.IsCollected
            };

            // Gem historikken
            _context.ReservationHistory.Add(history);
            await _context.SaveChangesAsync();

            return Ok("Reservation history created.");
        }
    }
}
