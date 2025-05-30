namespace ReservationSystemWebAPI.DTOs
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Role { get; set; } = "Bruger"; // Default role is "User"
        public string RowVersion { get; set; } = ""; // RowVersion as a string for easier handling in JSON
    }
}