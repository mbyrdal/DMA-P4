using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationSystemWebAPI.Models
{
    /// <summary>
    /// Represents an immutable snapshot of a reservation used for auditing and tracking changes over time.
    /// This entity allows historical inspection without altering the live reservation data.
    /// </summary>
    public class ReservationHistory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the reservation history record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the original reservation to which this history record corresponds.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the original reservation.
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// Gets or sets the date and time when this history snapshot was recorded.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets a value indicating whether the items were marked as collected at the time of this snapshot.
        /// </summary>
        public bool IsCollected { get; set; }

        /// <summary>
        /// Gets or sets the associated reservation entity that this history record references.
        /// This is a navigation property and may be null depending on the context.
        /// </summary>
        [ForeignKey("ReservationId")]
        public Reservation? Reservation { get; set; }

        /// <remarks>
        /// If future audit requirements include item-level tracking, consider adding a collection
        /// of reservation item snapshots to this entity.
        /// </remarks>
    }
}