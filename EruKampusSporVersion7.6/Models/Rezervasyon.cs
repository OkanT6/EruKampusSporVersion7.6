namespace EruKampusSpor.Models
{
    public class Rezervasyon
    {
        public int KullanıcıId { get; set; } 
        public int SeansId {  get; set; }

        public DateTime RezerveEdilmeTarihi { get; set; }

        public bool IptalEdildi { get; set; }

        public string? IptalEdilenBrans {  get; set; }

        public Kullanıcı Kullanıcı {  get; set; }
        public Seans Seans { get; set; }




    }
}
