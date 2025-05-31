using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.DTOs
{
    /// Data Transfer Object (DTO) for updating a storage item.
    public class StorageItemUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Navn { get; set; } = string.Empty;
        [Range(0, int.MaxValue, ErrorMessage = "Antal skal være et positivt heltal.")]
        public int Antal { get; set; }
        public string? Reol { get; set; }
        public string? Hylde { get; set; }
        public string? Kasse { get; set; }

        // OCC token sent by client (as a Base64 string)
        [Required]
        public string RowVersion { get; set; } = string.Empty;
    }

    // / Data Transfer Object (DTO) foor returning a storage item to the client (read-only)
    public class StorageItemReadDto
    {
        public int Id { get; set; }
        public string? Navn { get; set; }
        public int Antal { get; set; }
        public string? Reol { get; set; }
        public string? Hylde { get; set; }
        public string? Kasse { get; set; }

        // Return RowVersion in base64 format
        public string RowVersion { get; set; } = string.Empty;
    }

    // Data Transfer Object (DTO) for creating a new storage item
    public class StorageItemCreateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Navn { get; set; } = string.Empty;
        [Range(0, int.MaxValue, ErrorMessage = "Antal skal være et positivt heltal.")]
        public int Antal { get; set; }
        public string? Reol { get; set; }
        public string? Hylde { get; set; }
        public string? Kasse { get; set; }
    }
}
