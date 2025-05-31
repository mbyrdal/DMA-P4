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
            // Inject the reservation service for business logic and data access
            _reservationService = reservationService;
        }

        /// <summary>
        /// Retrieves all reservations with their associated items.
        /// Maps entities to read-only DTOs before returning.
        /// </summary>
        /// <returns>List of all reservations as DTOs.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetAll()
        {
            var reservations = await _reservationService.GetAllAsync();

            // Manual mapping of Reservation entities to ReservationReadDto objects
            var dtos = reservations.Select(r => new ReservationReadDto
            {
                Id = r.Id,
                Email = r.Email,
                CreatedAt = r.CreatedAt,
                IsCollected = r.IsCollected,
                Status = r.Status,
                RowVersion = Convert.ToBase64String(r.RowVersion), // Convert concurrency token for client
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

        /// <summary>
        /// Retrieves a single reservation by its ID.
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>Reservation object or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetById(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        /// <summary>
        /// Retrieves all reservations for a specific user identified by email.
        /// Maps entities to DTOs for client consumption.
        /// </summary>
        /// <param name="email">User email address</param>
        /// <returns>List of reservations for the user.</returns>
        [HttpGet("user/{email}")]
        public async Task<ActionResult<IEnumerable<ReservationReadDto>>> GetByUser(string email)
        {
            var reservations = await _reservationService.GetByUserEmailAsync(email);

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

        /// <summary>
        /// Creates a new reservation from the provided DTO.
        /// Validates the incoming model and returns appropriate responses.
        /// </summary>
        /// <param name="dto">Data transfer object with reservation details</param>
        /// <returns>Created reservation or error response.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Invalid reservation data: {ModelState}");
            }

            try
            {
                var reservation = await _reservationService.CreateAsync(dto);

                if (reservation == null)
                {
                    // Creation failed for some reason
                    return StatusCode(500, "Reservation kunne ikke oprettes. Tjek venligst dine data.");
                }

                // Return 201 Created with the location header pointing to the new resource
                return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
            }
            catch (Exception ex)
            {
                return BadRequest($"Fejl ved oprettelse af reservation: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing reservation using the provided DTO.
        /// Supports concurrency handling and validation.
        /// </summary>
        /// <param name="id">ID of the reservation to update</param>
        /// <param name="dto">Data transfer object with updated reservation details</param>
        /// <returns>Status code indicating success or failure</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReservationUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Invalid reservation update data: {ModelState}"); // Validation error
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

        /// <summary>
        /// Deletes a reservation by its ID.
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>NoContent if deleted, NotFound otherwise</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _reservationService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Marks items in the reservation as returned.
        /// Handles concurrency and returns appropriate status codes.
        /// </summary>
        /// <param name="reservationId">ID of the reservation</param>
        /// <returns>NoContent on success, NotFound or Conflict on failure</returns>
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

        /// <summary>
        /// Creates a history record for the reservation.
        /// </summary>
        /// <param name="reservationId">Reservation ID</param>
        /// <returns>Success message or NotFound if reservation does not exist</returns>
        [HttpPost("createHistory/{reservationId}")]
        public async Task<IActionResult> CreateHistory(int reservationId)
        {
            var success = await _reservationService.CreateHistoryAsync(reservationId);
            if (!success) return NotFound("Fejl ved oprettelse af historik");
            return Ok("Historik gemt");
        }
    }
}