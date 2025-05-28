using System.ComponentModel.DataAnnotations;

namespace ReservationSystemWebAPI.DTOs
{
    public class LoginResponse
    {
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Token { get; set; }
    }
}
