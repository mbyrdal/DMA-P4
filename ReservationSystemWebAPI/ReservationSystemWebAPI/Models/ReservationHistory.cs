using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationSystemWebAPI.Models
{
    /// <summary>
    /// Represents a historical snapshot of a reservation.
    /// This is used for tracking or auditing purposes without affecting the active reservation data.
    /// </summary>
    public class ReservationHistory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the reservation history entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the original reservation this history entry refers to.
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the reservation.
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// Gets or sets the timestamp when this history record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets a value indicating whether the reserved items had been collected at the time of this history entry.
        /// </summary>
        public bool IsCollected { get; set; }

        /// <summary>
        /// Gets or sets the original reservation entity this history entry is associated with.
        /// This serves as a navigation property and is not required for all operations.
        /// </summary>
        [ForeignKey("ReservationId")]
        public Reservation? Reservation { get; set; }

        // Note: If historical tracking of reservation items is added later,
        // consider including a List<ReservationItems> here with deep copies of items.
    }
}