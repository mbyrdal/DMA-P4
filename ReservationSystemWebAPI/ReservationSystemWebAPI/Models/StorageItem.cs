namespace ReservationSystemWebAPI.Models
{
    public class StorageItem
    {
        public int ID { get; set; }
        public string Navn { get; set; } = string.Empty;
        public int Antal { get; set; }
        public string Lokation { get; set; } = string.Empty;

    }
}