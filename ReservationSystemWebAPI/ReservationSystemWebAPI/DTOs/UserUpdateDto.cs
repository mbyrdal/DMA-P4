using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) for updating a user’s details.
    /// Includes concurrency token for optimistic concurrency control.
    /// </summary>
    public class UserUpdateDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user being updated.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the updated name of the user.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the role of the user (e.g., "Bruger" or "Admin").
        /// Defaults to "Bruger".
        /// </summary>
        public string Role { get; set; } = "Bruger";

        /// <summary>
        /// Gets or sets the concurrency token (RowVersion) as a Base64 string.
        /// Required for optimistic concurrency control during update operations.
        /// </summary>
        [Required]
        public string RowVersion { get; set; } = "";
    }
}