using Microsoft.EntityFrameworkCore;
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

        [Required]
        public string Email { get; set; } = "hej123@abc.dk"; // valgfrit

        [Required]
        public string Password { get; set; } = "123abc"; // Store hashed password
    }
}
