using System.Text.Json.Serialization;

namespace EruKampusSpor.Models
{
        public class Salon
        {

        public Salon()
        {
            Branslar = new HashSet<SalonBrans>();
            Seanslar = new HashSet<Seans>();
        }
        public int SalonId { get; set; } //Primary Key
            public string SalonaAdı { get; set;}

            [JsonIgnore]

            public Tesis Tesis { get; set; }

            public int TesisId { get; set; }//Foreign Key


            // Many-to-Many Relationship
            [JsonIgnore]
            public ICollection<SalonBrans> Branslar { get; set; }
            [JsonIgnore]
            public ICollection<Seans> Seanslar { get; set; } // Seanslarla ilişki


    }
}
