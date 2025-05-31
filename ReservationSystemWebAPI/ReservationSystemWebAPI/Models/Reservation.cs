using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ReservationSystemWebAPI.Models
{
    /// <summary>
    /// Represents a reservation created by a user.
    /// Contains metadata such as the user's email, reservation status, creation time, and associated items.
    /// </summary>
    public class Reservation
    {
        /// <summary>
        /// Gets or sets the unique identifier for the reservation.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user who made the reservation.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the reservation was created.
        /// Defaults to the current server time.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the list of items included in the reservation.
        /// </summary>
        public List<ReservationItems> Items { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether the reserved items have been collected by the user.
        /// </summary>
        public bool IsCollected { get; set; }

        /// <summary>
        /// Gets or sets the current status of the reservation (e.g., <c>pending</c>, <c>confirmed</c>, <c>cancelled</c>).
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the row version used for optimistic concurrency control.
        /// This property is used internally and is excluded from API responses.
        /// </summary>
        [Timestamp]
        [JsonIgnore]
        public byte[]? RowVersion { get; set; }
    }

    /// <summary>
    /// Represents an individual item reserved as part of a reservation,
    /// including quantity and return status.
    /// </summary>
    public class ReservationItems
    {
        /// <summary>
        /// Gets or sets the unique identifier for the reservation item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name or identifier of the reserved equipment.
        /// </summary>
        public string Equipment { get; set; } = "";

        /// <summary>
        /// Gets or sets the quantity of the equipment being reserved.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the ID of the reservation to which this item belongs.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item has been returned.
        /// </summary>
        public bool IsReturned { get; set; } = false;

        /// <summary>
        /// Gets or sets the row version used for optimistic concurrency control.
        /// This property is used internally and is excluded from API responses.
        /// </summary>
        [Timestamp]
        [JsonIgnore]
        public byte[] RowVersion { get; set; } = null!;

        /// <summary>
        /// Gets or sets the parent reservation associated with this item.
        /// This navigation property is ignored in API responses.
        /// </summary>
        [JsonIgnore]
        [ForeignKey("ReservationId")]
        public Reservation? Reservation { get; set; }
    }
}