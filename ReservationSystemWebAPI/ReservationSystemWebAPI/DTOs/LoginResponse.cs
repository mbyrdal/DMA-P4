using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.DTOs
{
    /// <summary>
    /// Represents the response returned to the client after a successful login.
    /// Includes the user's email, role, and a JWT token.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// The email address of the authenticated user.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The role assigned to the authenticated user (e.g., "Bruger", "Admin").
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// The JSON Web Token (JWT) used for authenticating future requests.
        /// </summary>
        public string? Token { get; set; }
    }
}