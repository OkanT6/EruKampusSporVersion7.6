using System.Text.Json.Serialization;

namespace EruKampusSpor.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string adminKullanıcıAdı {  get; set; }

        public string password { get; set; }

        public int TesisId {  get; set; }

        [JsonIgnore]
        public Tesis Tesis { get; set; }
    }
}
