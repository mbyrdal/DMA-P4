using System.ComponentModel.DataAnnotations.Schema;

namespace ReservationSystemWebAPI.Models
{
    public class ReservationHistory
    {
        public int Id { get; set; }

        // Dette er en fremmednøgle, der refererer til den oprindelige reservation
        public int ReservationId { get; set; }

        public string Email { get; set; } = ""; // Email for reservationen
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Tidspunkt for historikken
        public bool IsCollected { get; set; } // Om reservationen er blevet indsamlet

        // Eventuelt kan vi tilføje en liste af ReservationItems, hvis vi vil gemme elementerne i historikken også
        // public List<ReservationItems> Items { get; set; } = new();

        [ForeignKey("ReservationId")]
        public Reservation Reservation { get; set; } // Navigation til den oprindelige reservation
    }
}