using EruKampusSpor.Data;
using EruKampusSpor.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EruKampusSpor.DTOs
{
    public class SeansDTO
    {




        public int TesisId { get; set; } // Foreign Key
        public int SalonId { get; set; } // Foreign Key
        public TimeSpan SeansSaati { get; set; } // Saati tutar
        public DateTime Tarih { get; set; } // Tarihi tutar
                                            // public bool SeansRezerveEdildiMi { get; set; } // Durum bilgisi
                                            //public int? KullanıcıId { get; set; } // Nullable Foreign Key
        public int SeansId { get; set; } //Alternate Key
        public int Kontenjan { get; set; }

        public string? yapılanBrans { get; set; }



        // Bu property EF Core tarafından veritabanına kaydedilmeyecek
        public DateTime SeansBaslangicZamani { get; set; }
        public bool Dolu { get; set; }

        public SeansCinsiyet SeansCinsiyet { get; set; }

        public int RezerveEdenKisiSayisi { get; set; }





    }
}
