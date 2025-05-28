namespace ReservationSystemWebAPI.DTOs;

/// <summary>
/// DTO used when creating a new reservation.
/// Includes user email, status, and the list of items to reserve.
/// </summary>
public class ReservationCreateDto
{
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = "Inaktiv";
    public List<ReservationItemCreateDto> Items { get; set; } = new();
}

public class ReservationItemCreateDto
{
    public string Equipment { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

/// <summary>
/// DTO used when updating an existing reservation (e.g., changing status, marking as collected).
/// Includes concurrency control through RowVersion.
/// </summary>
public class ReservationUpdateDto
{
    /// <summary>
    /// Optional updated status (e.g., "Aktiv", "Afsluttet").
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Whether the reservation has been collected.
    /// </summary>
    public bool? IsCollected { get; set; }

    /// <summary>
    /// Updated list of items in the reservation.
    /// </summary>
    public List<ReservationItemUpdateDto> Items { get; set; } = new();

    /// <summary>
    /// Used for optimistic concurrency control (OCC).
    /// </summary>
    public byte[]? RowVersion { get; set; } = Array.Empty<byte>();
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
    /// Used for optimistic concurrency control (OCC).
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

/// <summary>
/// Optional: Read-only DTO used when returning reservation data to the frontend.
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
}

public class ReservationItemReadDto
{
    public string Equipment { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsReturned { get; set; }
}