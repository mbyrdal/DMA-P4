using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ReservationSystemWebAPI.Models
{
    /// <summary>
    /// Represents a reservation made by a user, including metadata such as email, status, and associated reserved items.
    /// </summary>
    public class Reservation
    {
        /// <summary>
        /// Gets or sets the unique identifier for the reservation.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user who created the reservation.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the timestamp indicating when the reservation was created.
        /// Defaults to the current date and time.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the list of equipment items associated with this reservation.
        /// </summary>
        public List<ReservationItems> Items { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether the reserved items have been collected by the user.
        /// </summary>
        public bool IsCollected { get; set; }

        /// <summary>
        /// Gets or sets the current status of the reservation (e.g., pending, confirmed, cancelled).
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the row version used for optimistic concurrency control (OCC).
        /// This value is used internally and is excluded from API responses.
        /// </summary>
        [Timestamp]
        [JsonIgnore]
        public byte[]? RowVersion { get; set; }
    }

    /// <summary>
    /// Represents a specific item and quantity reserved as part of a reservation.
    /// Each item links to a parent reservation.
    /// </summary>
    public class ReservationItems
    {
        /// <summary>
        /// Gets or sets the unique identifier for the reservation item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name or identifier of the equipment being reserved.
        /// </summary>
        public string Equipment { get; set; } = "";

        /// <summary>
        /// Gets or sets the quantity of this equipment being reserved.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the ID of the parent reservation this item belongs to.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item has been returned.
        /// </summary>
        public bool IsReturned { get; set; } = false;

        /// <summary>
        /// Gets or sets the row version used for optimistic concurrency control (OCC).
        /// This field is excluded from API responses.
        /// </summary>
        [Timestamp]
        [JsonIgnore]
        public byte[] RowVersion { get; set; } = null!;

        /// <summary>
        /// Gets or sets the parent reservation associated with this item.
        /// This reference is ignored in API responses.
        /// </summary>
        [JsonIgnore]
        [ForeignKey("ReservationId")]
        public Reservation? Reservation { get; set; }
    }
}