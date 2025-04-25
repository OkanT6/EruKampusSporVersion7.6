using System.Text.Json.Serialization;

namespace EruKampusSpor.Models
{
    public class Kullanıcı
    {
        public Kullanıcı()
        {
            Rezervasyonlar = new HashSet<Rezervasyon>();
        }

        public int KullanıcıId { get; set; }
        public string TC { get; set; } 
        public string Name { get; set; }

        


        [JsonIgnore]
        public KullanıcıDetay KullanıcıDetay { get; set; }

        
        [JsonIgnore]
        public ICollection<Rezervasyon> Rezervasyonlar { get; set; }




    }
}
