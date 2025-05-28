using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ReservationSystemWebAPI.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<ReservationItems> Items { get; set; } = new();
        public bool IsCollected { get; set; }
        public string? Status { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }

    public class ReservationItems
    {
        public int Id { get; set; }
        public string Equipment { get; set; } = "";
        public int Quantity { get; set; }
        public int ReservationId { get; set; }
        public bool IsReturned { get; set; } = false;


        [JsonIgnore] // Ignoreres ved serialization
        [ForeignKey("ReservationId")]
        public Reservation? Reservation { get; set; }
    }

}
