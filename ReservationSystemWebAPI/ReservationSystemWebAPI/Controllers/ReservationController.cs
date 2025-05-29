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
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetById(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        [HttpGet("user/{email}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetByUser(string email)
        {
            var reservations = await _reservationService.GetByUserEmailAsync(email);
            return Ok(reservations);
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
            try
            {
                var success = await _reservationService.UpdateAsync(id, dto);
                if (!success)
                    return NotFound();

                return NoContent();
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
