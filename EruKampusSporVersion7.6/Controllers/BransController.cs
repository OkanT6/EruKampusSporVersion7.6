using EruKampusSpor.Data;
using EruKampusSpor.DTOs;
using EruKampusSpor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EruKampusSpor.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class BransController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BransController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("api/branslar")]
        [Authorize(Policy = "AdminOrUser")]
        public IActionResult GetBranslar()
        {
            var branslar = _context.Branslar.ToList();  // Tüm Tesisleri getir
            if (branslar == null || !branslar.Any())
            {
                return NotFound("Hiçbir Brans bulunamadı.");
            }



            return Ok(branslar);  // Başarıyla bulunan Tesisleri döndür



        }


        [HttpPost]
        [Route("api/branslar")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult AddBrans([FromBody] BransDTO bransDTO)
        {
            var branslar = _context.Branslar.ToList();
            foreach (Brans b in branslar)
            {
                if (String.Equals(b.BransAdı, bransDTO.BransAdı))
                    return BadRequest("Brans Zaten Mevcut");

            }

            if (bransDTO == null)
            {
                return BadRequest("Brans verisi eksik.");
            }

            Brans yeniBrans = new Brans { BransAdı = bransDTO.BransAdı };


            // Tesis'i veritabanına ekle
            _context.Branslar.Add(yeniBrans);
            _context.SaveChanges();  // Değişiklikleri kaydet

            // Başarıyla eklenen Tesis'i döndür
            //return CreatedAtAction(nameof(GetTesisById), new { id = tesis.TesisId }, tesis);
            return Ok(yeniBrans);
        }

        [HttpDelete]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteBrans([FromQuery] int bransId)
        {

            var brans = _context.Branslar.Find(bransId);
            if (brans == null)
                return NotFound("Böyle bir brans yoktur");
            _context.Branslar.Remove(brans);
            _context.SaveChanges(true);
            return Ok(brans);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult tesisAdminineGoreBransAra()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            // Token'daki ID'yi int'e dönüştür
            if (!int.TryParse(userIdClaim, out var id))
                return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            var adminId = id;

            //var branslar = _context.SalonBrans
            //.Include(sb => sb.Salon)
            //.ThenInclude(s => s.Tesis)
            //.ThenInclude(t => t.Admin)
            //.Where(sb => sb.Salon.Tesis.Admin.Id == adminId)
            //.GroupBy(sb => sb.BransId) // BransId'ye göre gruplama
            //.Select(g => g.FirstOrDefault()) // Her gruptan ilk öğeyi seçme
            //.ToList();


            var branslar2 = _context.Branslar
                .Include(b => b.Salonlar)
                    .ThenInclude(sb => sb.Salon)
                    .ThenInclude(s => s.Tesis)
                    .ThenInclude(t => t.Admin)
                .Where(b => b.Salonlar.Any(sb => sb.Salon.Tesis.Admin.Id == adminId))
                .ToList();            /* 
             var branslar = _context.SalonBrans
    .Include(sb => sb.Salon)
    .ThenInclude(s => s.Tesis)
    .ThenInclude(t => t.Admin)
    .Where(sb => sb.Salon.Tesis.Admin.Id == adminId)
    .Join()
    .ToList();
             */
            return Ok(branslar2);

            

        }
    }
}
