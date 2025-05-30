using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.Models
{
    /// <summary>
    /// Represents a user of the reservation system.
    /// Users can be either regular users or administrators, based on their role.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// This field is required.
        /// </summary>
        [Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the role of the user.
        /// Can be either <c>"Bruger"</c> (User) or <c>"Admin"</c>.
        /// Defaults to <c>"Bruger"</c>.
        /// </summary>
        [Required]
        public string Role { get; set; } = "Bruger";

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// Gets or sets the user's password.
        /// Stored as a bcrypt hash using BCrypt.Net for security.
        /// </summary>
        public string Password { get; set; } = "";

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
