using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ReservationSystemWebAPI.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string? UserName { get; set; } // valgfrit, kan bruges senere med login
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<ReservationItem> Items { get; set; } = new();
        public bool IsCollected { get; set; }
        public DateTime? CollectedAt { get; set; }
    }

    public class ReservationItem
    {
        public int Id { get; set; }
        public string EquipmentName { get; set; } = "";
        public int Quantity { get; set; }
        public int ReservationId { get; set; }

        [JsonIgnore] // Ignoreres ved serialization
        [ForeignKey("ReservationId")]
        public Reservation? Reservation { get; set; }
    }

}
