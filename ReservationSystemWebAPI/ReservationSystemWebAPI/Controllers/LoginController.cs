using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.DTOs;

namespace ReservationSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ReservationDbContext _context;

        public LoginController(ReservationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized($"Bruger ikke fundet med denne e-mail: {request.Email}");
            }

            // Check if hashed password matches the password stored in the database
            bool passwordMatch = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!passwordMatch)
            {
                return Unauthorized("Forkert adgangskode.");
            }

            return Ok(new { user.Email, user.Role });
        }
    }

}
