namespace EruKampusSpor.DTOs
{
    public class SalonDTO
    {
        public int SalonId { get; set; } //Primary Key
        public string SalonaAdı { get; set; }

        public int TesisId { get; set; } //Foreign Key'i tutar
    }
}
