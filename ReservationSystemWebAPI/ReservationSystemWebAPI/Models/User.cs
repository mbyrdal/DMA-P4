using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ReservationSystemWebAPI.Models
{
    /// <summary>
    /// Represents an authenticated user in the reservation system.
    /// Users may be assigned roles such as regular user or administrator.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user.
        /// This field is required.
        /// </summary>
        [Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the role assigned to the user.
        /// Valid values include <c>"Bruger"</c> (User) and <c>"Admin"</c> (Administrator).
        /// Defaults to <c>"Bruger"</c>.
        /// </summary>
        [Required]
        public string Role { get; set; } = "Bruger";

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// Gets or sets the hashed password of the user.
        /// Passwords are stored securely using the BCrypt hashing algorithm.
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// Gets or sets the row version used for optimistic concurrency control.
        /// This value is managed by the database and excluded from API responses for security and internal consistency.
        /// </summary>
        [Timestamp]
        [JsonIgnore]
        public byte[] RowVersion { get; set; } = null!;
    }
}