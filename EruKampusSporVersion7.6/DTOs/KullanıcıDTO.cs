using EruKampusSpor.Data;

namespace EruKampusSpor.DTOs
{
    public class KullanıcıDTO
    {
        public int KullanıcıId { get; set; }
        public string TC { get; set; }
        public string Name { get; set; }
        public Cinsiyet Cinsiyet { get; set; }
    }

}
