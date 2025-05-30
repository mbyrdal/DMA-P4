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
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a list of all users in the system.
        /// </summary>
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
                return StatusCode(500, $"Fejl under hentning af brugere: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a single user by their unique ID.
        /// </summary>
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