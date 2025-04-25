using System.Text.Json.Serialization;

namespace EruKampusSpor.Models
{
    public class SalonBrans
    {
        // Composite Primary Key (SalonId + BransId)
        public int SalonId { get; set; }
        public int BransId { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public Salon Salon { get; set; }
        [JsonIgnore]
        public Brans Brans { get; set; }
    }
}
