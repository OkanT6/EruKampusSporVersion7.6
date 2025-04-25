using EruKampusSpor.Data;

namespace EruKampusSpor.DTOs
{
    public class KullanıcıDetayDTO
    {
        public string name { get; set; }
        public string adres { get; set; }
        public string telefon { get; set; }
        public string email { get; set; }
        public Cinsiyet cinsiyet { get; set; }

    }
}
