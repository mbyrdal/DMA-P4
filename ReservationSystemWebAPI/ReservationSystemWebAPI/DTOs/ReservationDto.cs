namespace ReservationSystemWebAPI.DTOs;

public class ReservationDto
{
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = "Inaktiv";
    public List<ReservationItemDto> Items { get; set; } = new();
}

public class ReservationItemDto
{
    public string Equipment { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
