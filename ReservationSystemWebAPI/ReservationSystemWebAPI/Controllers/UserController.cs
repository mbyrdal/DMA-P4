using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Services;

namespace ReservationSystemWebAPI.Controllers
{
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
        /// <returns>
        /// 200 OK with a list of users if successful, 
        /// or 500 Internal Server Error if an unexpected error occurs.
        /// </returns>
        // GET: api/User
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
        /// <param name="id">The unique ID of the user.</param>
        /// <returns>
        /// 200 OK with the user data if found, 
        /// 404 Not Found if no user exists with the given ID,
        /// or 500 Internal Server Error for unexpected errors.
        // GET: api/User/5
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
        /// <param name="user">The user data to create.</param>
        /// <returns>
        /// 201 Created with the created user and location header, 
        /// 400 Bad Request if validation fails,
        /// or 500 Internal Server Error if something goes wrong.
        /// </returns>
        // POST: api/User
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
        /// Updates an existing user based on their ID.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="updatedUser">The updated user data.</param>
        /// <returns>
        /// 204 No Content if the update is successful, 
        /// 400 Bad Request if the ID does not match the user data,
        /// 404 Not Found if the user does not exist,
        /// or 500 Internal Server Error if an error occurs.
        /// </returns>
        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest("Bruger-ID stemmer ikke overens.");
            }

            try
            {
                await _userService.UpdateAsync(updatedUser);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Bruger med ID {id} findes ikke.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fejl under opdatering af bruger: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes an existing user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>
        /// 204 No Content if the deletion is successful,
        /// 404 Not Found if the user does not exist,
        /// or 500 Internal Server Error if an error occurs.
        /// </returns>
        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch(KeyNotFoundException)
            {
                return NotFound($"Bruger med ID {id} findes ikke.");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Fejl under sletning af bruger: {ex.Message}");
            }
        }
    }
}
