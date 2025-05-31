using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.DTOs
{
    /// <summary>
    /// DTO used to create a new reservation.
    /// Contains user email, reservation status, collection flag, and a list of reservation items.
    /// </summary>
    public class ReservationCreateDto
    {
        /// <summary>
        /// Gets or sets the email address of the user making the reservation.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current status of the reservation.
        /// Defaults to "Inaktiv".
        /// </summary>
        public string Status { get; set; } = "Inaktiv";

        /// <summary>
        /// Gets or sets a value indicating whether the reserved items have been collected.
        /// Defaults to false.
        /// </summary>
        public bool IsCollected { get; set; } = false;

        /// <summary>
        /// Gets or sets the collection of items included in the reservation.
        /// Note: <c>IsReturned</c> is not required on creation and defaults to false in the domain model.
        /// </summary>
        public List<ReservationItemCreateDto> Items { get; set; } = new();
    }

    /// <summary>
    /// DTO representing an individual reservation item during creation.
    /// </summary>
    public class ReservationItemCreateDto
    {
        /// <summary>
        /// Gets or sets the name or identifier of the equipment.
        /// </summary>
        public string Equipment { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quantity of the equipment reserved.
        /// </summary>
        public int Quantity { get; set; }

        // Optional: Include IsReturned if tracking at creation becomes necessary.
        // public bool IsReturned { get; set; } = false;
    }

    /// <summary>
    /// DTO used to update an existing reservation.
    /// Supports partial updates and optimistic concurrency control via <see cref="RowVersion"/>.
    /// </summary>
    public class ReservationUpdateDto
    {
        /// <summary>
        /// Gets or sets the email address of the user who made the reservation.
        /// Nullable to allow partial updates.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the updated reservation status (e.g., "Aktiv", "Afsluttet").
        /// Nullable to allow partial updates.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the reservation has been collected.
        /// Nullable to allow partial updates.
        /// </summary>
        public bool? IsCollected { get; set; }

        /// <summary>
        /// Gets or sets the updated list of reservation items.
        /// Required for update operations.
        /// </summary>
        [Required]
        public List<ReservationItemUpdateDto> Items { get; set; } = new();

        /// <summary>
        /// Gets or sets the concurrency token for optimistic concurrency control.
        /// This is required.
        /// </summary>
        [Required]
        public string RowVersion { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating individual reservation items.
    /// Includes concurrency token for optimistic concurrency control.
    /// </summary>
    public class ReservationItemUpdateDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the reservation item being updated.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name or identifier of the equipment.
        /// </summary>
        public string Equipment { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the updated quantity of the equipment.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the equipment has been returned.
        /// </summary>
        public bool IsReturned { get; set; }

        /// <summary>
        /// Gets or sets the concurrency token for optimistic concurrency control.
        /// This is required.
        /// </summary>
        [Required]
        public string RowVersion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Read-only DTO used to transfer reservation data to clients.
    /// Mirrors the Reservation entity excluding internal tracking fields.
    /// </summary>
    public class ReservationReadDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the reservation.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user who made the reservation.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time the reservation was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the list of reservation items.
        /// </summary>
        public List<ReservationItemReadDto> Items { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether the reservation has been collected.
        /// </summary>
        public bool IsCollected { get; set; }

        /// <summary>
        /// Gets or sets the current status of the reservation.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the concurrency token used for optimistic concurrency control.
        /// </summary>
        public string RowVersion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Read-only DTO representing an individual reservation item.
    /// </summary>
    public class ReservationItemReadDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the reservation item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name or identifier of the equipment.
        /// </summary>
        public string Equipment { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quantity of the equipment reserved.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the equipment has been returned.
        /// </summary>
        public bool IsReturned { get; set; }

        /// <summary>
        /// Gets or sets the concurrency token used for optimistic concurrency control.
        /// </summary>
        public string RowVersion { get; set; } = string.Empty;
    }
}