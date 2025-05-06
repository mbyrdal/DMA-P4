using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;

namespace ReservationSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ReservationDbContext _context;

        public UserController(ReservationDbContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            if (id != updatedUser.Id) return BadRequest();

            _context.Entry(updatedUser).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(u => u.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
