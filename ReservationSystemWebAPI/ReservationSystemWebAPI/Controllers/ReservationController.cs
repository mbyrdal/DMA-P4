using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

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

        [HttpPost]
        public async Task<ActionResult<Reservation>> Create(Reservation reservation)
        {
            reservation.CreatedAt = DateTime.Now;
            reservation.IsConvertedToLoan = false;

            foreach (var item in reservation.Items)
            {
                var existing = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.EquipmentName);
                if (existing == null || existing.Antal < item.Quantity)
                {
                    return BadRequest($"Ikke nok af '{item.EquipmentName}' på lager.");
                }

                existing.Antal -= item.Quantity;
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        }

        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmToLoan(int id)
        {
            var res = await _context.Reservations.FindAsync(id);
            if (res == null) return NotFound();

            res.IsConvertedToLoan = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _context.Reservations.Include(r => r.Items).FirstOrDefaultAsync(r => r.Id == id);
            if (res == null) return NotFound();

            foreach (var item in res.Items)
            {
                var existing = await _context.WEXO_DEPOT.FirstOrDefaultAsync(e => e.Navn == item.EquipmentName);
                if (existing != null)
                {
                    existing.Antal += item.Quantity;
                }
            }

            _context.Reservations.Remove(res);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
