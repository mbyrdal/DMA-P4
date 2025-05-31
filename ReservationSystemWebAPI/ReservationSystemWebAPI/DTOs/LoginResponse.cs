using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.DTOs
{
    /// <summary>
    /// Represents the data returned to the client upon successful authentication.
    /// Includes the user's identity and an access token for authorized requests.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the email address of the authenticated user.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the role assigned to the user (e.g., <c>"Bruger"</c>, <c>"Admin"</c>).
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Gets or sets the JSON Web Token (JWT) issued to the user.
        /// This token must be included in the Authorization header of subsequent requests.
        /// </summary>
        public string? Token { get; set; }
    }
}