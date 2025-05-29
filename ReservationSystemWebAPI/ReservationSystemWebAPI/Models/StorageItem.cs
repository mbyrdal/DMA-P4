using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace ReservationSystemWebAPI.Models
{
    public class StorageItem
    {
        public int Id { get; set; }
        public string? Navn { get; set; }
        public int Antal { get; set; }
        public string? Reol { get; set; }
        public string? Hylde { get; set; }
        public string? Kasse { get; set; }

        // OCC approach using RowVersion column
        [Timestamp]
        [JsonIgnore] // Prevent accidental exposure in responses
        public byte[] RowVersion { get; set; } = null!;
    }
}