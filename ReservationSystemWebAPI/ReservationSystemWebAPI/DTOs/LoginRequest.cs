using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.DTOs
{
    /// <summary>
    /// Represents the data submitted by a user when attempting to authenticate.
    /// Used in login requests to verify user credentials and issue a token.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the email address of the user attempting to log in.
        /// This field is required.
        /// </summary>
        [Required]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password in plain text.
        /// This value is compared against the stored hash during authentication.
        /// It should always be transmitted over a secure HTTPS connection.
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the frontend origin (including port) making the login request.
        /// This may be used for audience validation or post-login redirection.
        /// </summary>
        public string Audience { get; set; } = string.Empty;
    }
}