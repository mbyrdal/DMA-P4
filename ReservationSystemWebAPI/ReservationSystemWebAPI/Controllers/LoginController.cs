using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.DTOs;
using ReservationSystemWebAPI.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace ReservationSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IConfiguration _configuration;

        public LoginController(ILoginService loginService, IConfiguration configuration)
        {
            // Inject the login service and configuration for JWT settings
            _loginService = loginService;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates the user credentials and returns a JWT token on successful authentication.
        /// </summary>
        /// <param name="request">LoginRequest containing Email and Password.</param>
        /// <returns>JWT token with user email and role on success.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Authenticate the user using the service layer
                var user = await _loginService.AuthenticateUserAsync(request.Email, request.Password);

                // Determine audience for the token, allowing dynamic frontend URLs during development
                var audience = !string.IsNullOrEmpty(request.Audience) ? request.Audience : _configuration["JwtSettings:Audience"];

                // Generate JWT token with claims for user identity and role
                var token = GenerateJwtToken(user.Email, user.Role, audience);

                // Return user info and token to the client
                return Ok(new LoginResponse
                {
                    Email = user.Email,
                    Role = user.Role,
                    Token = token
                });
            }
            catch (KeyNotFoundException ex)
            {
                // User not found in database
                return NotFound(new { message = ex.Message }); // "Bruger ikke fundet."
            }
            catch (UnauthorizedAccessException ex)
            {
                // Invalid password or unauthorized access
                return Unauthorized(new { message = ex.Message }); // "Ugyldige legitimationsoplysninger."
            }
            catch (Exception ex)
            {
                // General server error
                return StatusCode(500, $"Intern serverfejl: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a JWT token with the provided user email, role, and audience.
        /// </summary>
        /// <param name="email">User email to include in the token claims.</param>
        /// <param name="role">User role to include in the token claims.</param>
        /// <param name="audience">Intended audience for the token.</param>
        /// <returns>Serialized JWT token string.</returns>
        private string GenerateJwtToken(string email, string role, string audience)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            /*
             * Audience is passed dynamically, so the hardcoded audience from config is ignored here.
             */

            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]);

            // Create symmetric security key for signing the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Define claims for the JWT token, including subject (email), role, and unique identifier (JTI)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Create the JWT token object
            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: creds);

            // Return the serialized token string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}