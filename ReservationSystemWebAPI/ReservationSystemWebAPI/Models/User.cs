using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Role { get; set; } = "Bruger"; // Kan være "Bruger" eller "Admin"

        public string? Email { get; set; } // valgfrit
    }
}
