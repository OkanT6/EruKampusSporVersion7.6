namespace EruKampusSpor.Data
{
    public class EruUser
    {

        public int EruUserId { get; set; }
        public string name { get; set; }
        public string TC { get; set; }

        public Cinsiyet Cinsiyet { get; set; }

        public string obisis_password { get; set; }
    }
}
