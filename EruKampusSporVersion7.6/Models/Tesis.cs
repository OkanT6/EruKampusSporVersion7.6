using System.Text.Json.Serialization;

namespace EruKampusSpor.Models
{
    public class Tesis
    {
        public Tesis()
        {
            Salonlar = new HashSet<Salon>();
            Seanslar = new HashSet<Seans>();
       }

        public int TesisId { get; set; } //Primary Key
        public string TesisAdı { get; set; }
        [JsonIgnore]
        public ICollection<Salon> Salonlar { get; set; }

        [JsonIgnore]
        public ICollection<Seans> Seanslar { get; set; } // Seanslarla ilişki

        [JsonIgnore]
        public Admin Admin { get; set; }

    }
}
