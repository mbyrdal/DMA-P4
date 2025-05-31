using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) for updating a storage item.
    /// Includes optimistic concurrency control token as a Base64 string.
    /// </summary>
    public class StorageItemUpdateDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the storage item.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the storage item.
        /// </summary>
        [Required]
        public string Navn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quantity available.
        /// Must be a non-negative integer.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Antal skal være et positivt heltal.")]
        public int Antal { get; set; }

        /// <summary>
        /// Gets or sets the shelf (reol) where the item is stored.
        /// Optional.
        /// </summary>
        public string? Reol { get; set; }

        /// <summary>
        /// Gets or sets the level (hylde) on the shelf.
        /// Optional.
        /// </summary>
        public string? Hylde { get; set; }

        /// <summary>
        /// Gets or sets the box (kasse) where the item is kept.
        /// Optional.
        /// </summary>
        public string? Kasse { get; set; }

        /// <summary>
        /// Gets or sets the concurrency token for optimistic concurrency control.
        /// Provided by client as a Base64-encoded string.
        /// </summary>
        [Required]
        public string RowVersion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data Transfer Object (DTO) for returning storage item data to the client (read-only).
    /// </summary>
    public class StorageItemReadDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the storage item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the storage item.
        /// </summary>
        public string? Navn { get; set; }

        /// <summary>
        /// Gets or sets the quantity available.
        /// </summary>
        public int Antal { get; set; }

        /// <summary>
        /// Gets or sets the shelf (reol) location.
        /// </summary>
        public string? Reol { get; set; }

        /// <summary>
        /// Gets or sets the level (hylde) on the shelf.
        /// </summary>
        public string? Hylde { get; set; }

        /// <summary>
        /// Gets or sets the box (kasse) location.
        /// </summary>
        public string? Kasse { get; set; }

        /// <summary>
        /// Gets or sets the concurrency token (RowVersion) in Base64 format.
        /// </summary>
        public string RowVersion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data Transfer Object (DTO) for creating a new storage item.
    /// </summary>
    public class StorageItemCreateDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the storage item.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the storage item.
        /// </summary>
        [Required]
        public string Navn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quantity available.
        /// Must be a non-negative integer.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Antal skal være et positivt heltal.")]
        public int Antal { get; set; }

        /// <summary>
        /// Gets or sets the shelf (reol) where the item is stored.
        /// Optional.
        /// </summary>
        public string? Reol { get; set; }

        /// <summary>
        /// Gets or sets the level (hylde) on the shelf.
        /// Optional.
        /// </summary>
        public string? Hylde { get; set; }

        /// <summary>
        /// Gets or sets the box (kasse) where the item is kept.
        /// Optional.
        /// </summary>
        public string? Kasse { get; set; }
    }
}