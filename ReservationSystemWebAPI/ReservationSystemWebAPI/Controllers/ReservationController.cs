using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Services;

namespace ReservationSystemWebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetAll()
        {
            var reservations = await _reservationService.GetAllAsync();

            // Manual mapping to DTOs
            var dtos = reservations.Select(r => new ReservationReadDto
            {
                Id = r.Id,
                Email = r.Email,
                CreatedAt = r.CreatedAt,
                IsCollected = r.IsCollected,
                Status = r.Status,
                RowVersion = Convert.ToBase64String(r.RowVersion),
                Items = r.Items.Select(i => new ReservationItemReadDto
                {
                    Id = i.Id,
                    Equipment = i.Equipment,
                    Quantity = i.Quantity,
                    IsReturned = i.IsReturned,
                    RowVersion = Convert.ToBase64String(i.RowVersion)
                }).ToList()
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetById(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        [HttpGet("user/{email}")]
        public async Task<ActionResult<IEnumerable<ReservationReadDto>>> GetByUser(string email)
        {
            var reservations = await _reservationService.GetByUserEmailAsync(email);

            // Map to DTOs
            var dtos = reservations.Select(r => new ReservationReadDto
            {
                Id = r.Id,
                Email = r.Email,
                CreatedAt = r.CreatedAt,
                IsCollected = r.IsCollected,
                Status = r.Status,
                RowVersion = Convert.ToBase64String(r.RowVersion), // Convert byte[] → Base64
                Items = r.Items.Select(i => new ReservationItemReadDto
                {
                    Id = i.Id,
                    Equipment = i.Equipment,
                    Quantity = i.Quantity,
                    IsReturned = i.IsReturned,
                    RowVersion = Convert.ToBase64String(i.RowVersion) // Convert byte[] → Base64
                }).ToList()
            }).ToList();

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationCreateDto dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest($"Invalid reservation data: {ModelState}");
            }

            try
            {
                var reservation = await _reservationService.CreateAsync(dto);
                
                if (reservation == null)
                {
                    // Something went wrong during creation
                    return StatusCode(500, "Reservation kunne ikke oprettes. Tjek venligst dine data.");
                }

                // Return 201 created with the location of the new resource
                return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
            }
            catch (Exception ex)
            {
                // Internal server error
                return BadRequest($"Fejl ved oprettelse af reservation: {ex.Message}");
            }
        }

        // General update endpoint for updating reservation (e.g. status, items)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReservationUpdateDto dto)
        {

            // Auto-validate required fields (Items + RowVersion)
            if(!ModelState.IsValid)
            {
                return BadRequest($"Invalid reservation update data: {ModelState}"); // Returns 400 if Items are missing
            }

            try
            {
                var success = await _reservationService.UpdateAsync(id, dto);
                return success ? NoContent() : NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Reservationen er blevet ændret af en anden bruger. Refresh og prøv venligst igen.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _reservationService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPatch("returnItems/{reservationId}")]
        public async Task<IActionResult> ReturnItems(int reservationId)
        {
            try
            {
                var success = await _reservationService.ReturnItemsAsync(reservationId);
                if (!success) return NotFound("Fejl ved returnering");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Reservationen er blevet markeret som afleveret af en anden bruger.");
            }
        }

        [HttpPost("createHistory/{reservationId}")]
        public async Task<IActionResult> CreateHistory(int reservationId)
        {
            var success = await _reservationService.CreateHistoryAsync(reservationId);
            if (!success) return NotFound("Fejl ved oprettelse af historik");
            return Ok("Historik gemt");
        }
    }
}
