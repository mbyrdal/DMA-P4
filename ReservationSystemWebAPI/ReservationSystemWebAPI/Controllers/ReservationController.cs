using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<Reservation>> Create(ReservationDto dto)
        {
            var created = await _reservationService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            var success = await _reservationService.ConfirmAsync(id);
            if (!success) return BadRequest("Reservation kunne ikke bekræftes");
            return Ok("Reservation bekræftet");
        }

        [HttpPatch("markcollected/{id}")]
        public async Task<IActionResult> MarkCollected(int id)
        {
            var success = await _reservationService.MarkAsCollectedAsync(id);
            if (!success) return NotFound("Reservation ikke fundet");
            return NoContent();
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
            var success = await _reservationService.ReturnItemsAsync(reservationId);
            if (!success) return NotFound("Fejl ved returnering");
            return NoContent();
        }

        [HttpPost("createHistory/{reservationId}")]
        public async Task<IActionResult> CreateHistory(int reservationId)
        {
            var success = await _reservationService.CreateHistoryAsync(reservationId);
            if (!success) return NotFound("Fejl ved oprettelse af historik");
            return Ok("Historik gemt");
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] Reservation update)
        {
            var success = await _reservationService.UpdateStatusAsync(id, update.Status);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
