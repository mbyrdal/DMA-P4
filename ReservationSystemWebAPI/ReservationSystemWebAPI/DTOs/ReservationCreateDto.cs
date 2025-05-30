using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.DTOs;

/// <summary>
/// DTO used when creating a new reservation.
/// Includes user email, status, and the list of items to reserve.
/// </summary>
public class ReservationCreateDto
{
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = "Inaktiv";
    public bool IsCollected { get; set; } = false;

    // New items: IsReturned not needed on creation, defaults to false in domain
    public List<ReservationItemCreateDto> Items { get; set; } = new();
}

public class ReservationItemCreateDto
{
    public string Equipment { get; set; } = string.Empty;
    public int Quantity { get; set; }

    // Optional: can add IsReturned if needing to track at creation, but typically not needed
    // public bool IsReturned { get; set; } = false;
}

/// <summary>
/// DTO used when updating an existing reservation (e.g., changing status, marking as collected).
/// Includes concurrency control through RowVersion.
/// </summary>
public class ReservationUpdateDto
{
    /// <summary>
    /// The email address of the user making the reservation.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Updated status (e.g., "Aktiv", "Afsluttet").
    /// Nullable to allow partial updates if desired.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Whether the reservation has been collected.
    /// Nullable to allow partial updates.
    /// </summary>
    public bool? IsCollected { get; set; }

    /// <summary>
    /// Updated list of items in the reservation.
    /// </summary>
    [Required]
    public List<ReservationItemUpdateDto> Items { get; set; } = new();

    /// <summary>
    /// Used for optimistic concurrency control (OCC).
    /// Required.
    /// </summary>
    [Required]
    public string RowVersion { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating individual reservation items.
/// Includes RowVersion for concurrency.
/// </summary>
public class ReservationItemUpdateDto
{
    /// <summary>
    /// ID of the reservation item being updated.
    /// </summary>
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// The equipment name or identifier.
    /// </summary>
    public string Equipment { get; set; } = string.Empty;

    /// <summary>
    /// Updated quantity of equipment.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Whether the item has been returned.
    /// </summary>
    public bool IsReturned { get; set; }

    /// <summary>
    /// Used for optimistic concurrency control (OCC).
    /// Required.
    /// </summary>
    [Required]
    public string RowVersion { get; set; } = string.Empty;
}

/// <summary>
/// Read-only DTO used when returning reservation data to the frontend.
/// Mirrors the Reservation entity without internal tracking fields.
/// </summary>
public class ReservationReadDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<ReservationItemReadDto> Items { get; set; } = new();
    public bool IsCollected { get; set; }
    public string Status { get; set; } = string.Empty;

    // For concurrency control
    public string RowVersion { get; set; } = string.Empty;
}

public class ReservationItemReadDto
{
    public int Id { get; set; }
    public string Equipment { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsReturned { get; set; }

    // For concurrency control
    public string RowVersion { get; set; } = string.Empty;
}