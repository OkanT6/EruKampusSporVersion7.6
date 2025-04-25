using EruKampusSpor.Data;
using EruKampusSpor.DTOs;
using EruKampusSpor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EruKampusSpor.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]





    public class SalonController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public SalonController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/tesisler/{tesisId}/salonlar")]
        [Authorize(Policy = "AdminOrUser")]
        public IActionResult GetSalonlar(int tesisId)
        {
            // Tesis'i kontrol et
            var tesis = _context.Tesisler
                                .Include(t => t.Salonlar)  // Salonlar ile birlikte Tesis'i al
                                .FirstOrDefault(t => t.TesisId == tesisId);

            if (tesis == null)
            {
                return NotFound($"Tesis with ID {tesisId} bulunamadı.");
            }

            return Ok(tesis.Salonlar);  // Tesis'in Salonları'nı döndür
        }

        [HttpGet("salonlar/{salonId}")]
        [Authorize(Policy = "AdminOrUser")]
        public IActionResult GetSalonById(int salonId)
        {
            var salon = _context.Salonlar.FirstOrDefault(t => t.SalonId == salonId);

            if (salon == null)
            {
                return NotFound(); // Eğer salon bulunamazsa 404 döner.
            }

            return Ok(salon); // Salon bulunduysa 200 ve salonun verilerini döner.
        }


        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult PostSalon([FromBody] SalonDTO salonDTO)
        {
            // TesisId ile Tesis var mı kontrol et
            var tesis = _context.Tesisler.Find(salonDTO.TesisId);
            if (tesis == null)
            {
                return NotFound(); // Eğer tesis bulunmazsa 404 döner.
            }

            // Salon nesnesinde TesisId'yi manuel olarak ilişkilendir
            Salon salon=new Salon();

            salon.TesisId = salonDTO.TesisId;
            salon.SalonaAdı=salonDTO.SalonaAdı;


            // Salon nesnesini veritabanına ekle
            _context.Salonlar.Add(salon);
            _context.SaveChanges();

            // Yeni oluşturduğumuz salonun detaylarını dönmek için CreatedAtAction kullan
            return CreatedAtAction(nameof(GetSalonById), new { tesisId = salonDTO.TesisId, salonId = salon.SalonId }, salon);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrUser")]
        public IActionResult bransVeTesiseGoreSalonArama([FromQuery] int bransId,[FromQuery] int tesisId)
        {
            //var salonlar = _context.Salonlar.Where(s=>s.TesisId==tesisId).Include(s=>s.Branslar).ToList();

            var salonlar = _context.Salonlar
            .Where(s => s.TesisId == tesisId && s.Branslar.Any(b => b.BransId == bransId))
            .Include(s => s.Branslar)
            .ToList();

            return Ok(salonlar);
        }

    }

    

}
