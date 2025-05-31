namespace ReservationSystemWebAPI.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) used for deleting a user.
    /// Contains the concurrency token to ensure safe deletion.
    /// </summary>
    public class UserDeleteDto
    {
        /// <summary>
        /// The concurrency token (RowVersion) represented as a Base64 string.
        /// Used for optimistic concurrency control during delete operations.
        /// </summary>
        public string RowVersion { get; set; } = string.Empty;
    }
}