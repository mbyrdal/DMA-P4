using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.DTOs
{
    /// <summary>
    /// Represents the data required from a user to perform login authentication.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// The email address of the user attempting to log in.
        /// This field is required.
        /// </summary>
        [Required]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The user's password in plain text (will be verified against the stored hash).
        /// This field is required.
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The frontend URL (including port) from which the login request is made.
        /// This can be used for audience validation or redirects.
        /// </summary>
        public string Audience { get; set; } = string.Empty;
    }
}