using EruKampusSpor.Data;
using System.Text.Json.Serialization;

namespace EruKampusSpor.Models
{
    public class KullanıcıDetay
    {
        public int Id { get; set; } //KullanıcıId ile aynı olsun hem foreignkey hem de primaryKey
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public string EMail { get; set; }
        public string password { get; set; }

        public Cinsiyet Cinsiyet { get; set; }

        public string ProfilFotografiUrl { get; set; } // Yeni sütun

        // Foreign Key
        //public int KullanıcıId { get; set; }

        // Navigation Property

        [JsonIgnore]
        
        public Kullanıcı? Kullanıcı { get; set; }
    }
}
