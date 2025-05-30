using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ReservationSystemWebAPI.Models
{
    /// <summary>
    /// Represents a bookable equipment item stored in the inventory system.
    /// This entity maps directly to the database and holds metadata about the equipment's location and quantity.
    /// </summary>
    public class StorageItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for the storage item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the equipment item.
        /// </summary>
        public string? Navn { get; set; }

        /// <summary>
        /// Gets or sets the total quantity of this equipment item available for reservation.
        /// </summary>
        public int Antal { get; set; }

        /// <summary>
        /// Gets or sets the shelf (reol) where the equipment is stored.
        /// </summary>
        public string? Reol { get; set; }

        /// <summary>
        /// Gets or sets the level (hylde) of the shelf where the equipment is located.
        /// </summary>
        public string? Hylde { get; set; }

        /// <summary>
        /// Gets or sets the box (kasse) where the equipment is kept, if applicable.
        /// </summary>
        public string? Kasse { get; set; }

        /// <summary>
        /// Gets or sets the row version used for optimistic concurrency control (OCC).
        /// This value is automatically managed by the database and should not be exposed in API responses.
        /// </summary>
        [Timestamp]
        [JsonIgnore] // Prevent accidental exposure in responses
        public byte[] RowVersion { get; set; } = null!;
    }
}