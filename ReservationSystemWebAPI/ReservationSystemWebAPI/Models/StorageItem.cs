using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ReservationSystemWebAPI.Models
{
    /// <summary>
    /// Represents a bookable equipment item in the inventory system.
    /// Includes metadata such as storage location and available quantity.
    /// </summary>
    public class StorageItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for the storage item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name or description of the equipment item.
        /// </summary>
        public string? Navn { get; set; }

        /// <summary>
        /// Gets or sets the total quantity of this item available for reservation.
        /// </summary>
        public int Antal { get; set; }

        /// <summary>
        /// Gets or sets the shelf (<c>Reol</c>) where the item is stored.
        /// </summary>
        public string? Reol { get; set; }

        /// <summary>
        /// Gets or sets the level (<c>Hylde</c>) of the shelf where the item is placed.
        /// </summary>
        public string? Hylde { get; set; }

        /// <summary>
        /// Gets or sets the box (<c>Kasse</c>) containing the item, if applicable.
        /// </summary>
        public string? Kasse { get; set; }

        /// <summary>
        /// Gets or sets the row version used for optimistic concurrency control.
        /// This value is automatically maintained by the database and is excluded from API responses.
        /// </summary>
        [Timestamp]
        [JsonIgnore]
        public byte[] RowVersion { get; set; } = null!;
    }
}