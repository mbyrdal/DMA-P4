using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Services;

namespace ReservationSystemWebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            // Inject user service to handle user-related operations
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a list of all users in the system.
        /// </summary>
        /// <returns>List of User entities.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Return 500 with Danish error message for user feedback
                return StatusCode(500, $"Fejl under hentning af brugere: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a single user by their unique ID.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User entity or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                var foundUser = await _userService.GetByIdAsync(id);
                return Ok(foundUser);
            }
            catch (KeyNotFoundException)
            {
                // User not found
                return NotFound($"Bruger med ID {id} blev ikke fundet.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fejl under hentning af bruger: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="user">User entity to create</param>
        /// <returns>Created User entity with 201 status or error response.</returns>
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            try
            {
                await _userService.AddAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fejl under oprettelse af bruger: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing user based on their ID, with optimistic concurrency control.
        /// </summary>
        /// <param name="id">User ID to update</param>
        /// <param name="dto">User update DTO containing new data and concurrency token</param>
        /// <returns>NoContent on success or appropriate error response.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("Bruger-ID stemmer ikke overens med den opdaterede bruger-ID.");
            }

            byte[] rowVersion;
            try
            {
                // Convert RowVersion from Base64 string to byte array for concurrency check
                rowVersion = Convert.FromBase64String(dto.RowVersion);
            }
            catch (FormatException)
            {
                return BadRequest("Ugyldig RowVersion format. Skal være en Base64-kodet streng.");
            }

            try
            {
                var user = new User
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Role = dto.Role,
                    RowVersion = rowVersion
                };

                await _userService.UpdateAsync(user);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Bruger med ID {id} findes ikke.");
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Brugeren er blevet ændret eller slettet af en anden.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict("Konflikt under opdatering: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fejl under opdatering af bruger: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes an existing user by their ID.
        /// </summary>
        /// <param name="id">User ID to delete</param>
        /// <param name="dto">DTO containing concurrency token for optimistic concurrency control</param>
        /// <returns>NoContent on success or appropriate error response.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id, [FromBody] UserDeleteDto dto)
        {
            try
            {
                await _userService.DeleteAsync(id, dto.RowVersion);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Bruger med ID {id} findes ikke.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fejl under sletning af bruger: {ex.Message}");
            }
        }
    }
}