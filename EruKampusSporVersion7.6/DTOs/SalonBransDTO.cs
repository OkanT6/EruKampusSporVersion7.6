using System.ComponentModel.DataAnnotations;

namespace EruKampusSpor.DTOs
{
    public class SalonBransDTO
    {
        [Required]
        public int SalonId { get; set; }
        [Required]
        public int BransId { get; set; }
    }
}
