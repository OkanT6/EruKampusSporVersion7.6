using EruKampusSpor.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EruKampusSpor.Models
{
    
        public class Seans
        {

        public Seans()
        {
            Rezervasyonlar = new HashSet<Rezervasyon>();
        }
        public int TesisId { get; set; } // Foreign Key
            public int SalonId { get; set; } // Foreign Key
            public TimeSpan SeansSaati { get; set; } // Saati tutar
            public DateTime Tarih { get; set; } // Tarihi tutar
        
            public int SeansId {  get; set; } //Alternate Key
            public int Kontenjan {  get; set; }

            public string? yapılanBrans { get; set; }



       
        public DateTime SeansBaslangicZamani { get; set; }
        public bool Dolu {  get; set; }

            public SeansCinsiyet SeansCinsiyet { get; set; }

            public int RezerveEdenKisiSayisi {  get; set; }

        
        [JsonIgnore]
        public Tesis Tesis { get; set; }
        [JsonIgnore]
        public Salon Salon { get; set; }
        
        //public Kullanıcı Kullanıcılar { get; set; }
        [JsonIgnore]
            public ICollection<Rezervasyon> Rezervasyonlar { get; set; }


    }
    
}
