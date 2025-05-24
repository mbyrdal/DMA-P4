using System.ComponentModel.DataAnnotations.Schema;


namespace ReservationSystemWebAPI.Models
{
    public class StorageItem
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public int Antal { get; set; }
        public string Reol { get; set; }
        public string Hylde { get; set; }
        public string Kasse { get; set; }
    }
}
