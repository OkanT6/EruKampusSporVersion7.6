using System.Text.Json.Serialization;

namespace EruKampusSpor.Models
{
    public class Brans
    {

        public Brans()
        {
            Salonlar = new HashSet<SalonBrans>();
            
        }

        public int BransId { get; set; }
        public string BransAdı { get; set; }


        // Many-to-Many Relationship
        [JsonIgnore]
        public ICollection<SalonBrans> Salonlar { get; set; }

        
    }
}
