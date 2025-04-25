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
    public class TesisController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public TesisController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("api/tesisler")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetTesisler()
        {
            var tesisler = _context.Tesisler.ToList();  // Tüm Tesisleri getir
            if (tesisler == null || !tesisler.Any())
            {
                return NotFound("Hiçbir Tesis bulunamadı.");
            }

            return Ok(tesisler);  // Başarıyla bulunan Tesisleri döndür



        }


        [HttpPost]
        [Route("api/tesisler")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult AddTesis([FromBody] TesisDTO tesisDTO)
        {
            if (tesisDTO == null)
            {
                return BadRequest("Tesis verisi eksik.");
            }

            Tesis tesis = new Tesis
            {
                TesisAdı=tesisDTO.TesisAdı
            };

            // Tesis'i veritabanına ekle
            _context.Tesisler.Add(tesis);
            _context.SaveChanges();  // Değişiklikleri kaydet

            // Başarıyla eklenen Tesis'i döndür
            //return CreatedAtAction(nameof(GetTesisById), new { id = tesis.TesisId }, tesis);
            return Ok(tesis);
        }

        
        [HttpGet]
        [Authorize(Policy = "AdminOrUser")]
        public ActionResult bransaGoreTesisArama([FromQuery] int bransId)
        {
            var tesisler = _context.Tesisler
                .Include(t => t.Salonlar)
                .ThenInclude(s => s.Branslar)
                .ThenInclude(sb=>sb.Brans)
                .Where(t => t.Salonlar.Any(s => s.Branslar.Any(b => b.BransId == bransId)))
                .ToList();
            

            


            return Ok(tesisler);
          }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult admininTesisiniGetir(int adminId)
        {

            var tesis= _context.Tesisler.Include(t=>t.Admin).FirstOrDefault(t=>t.Admin.Id == adminId);
            if (tesis == null)
                throw new Exception("TesisId'si olmayan bir admin olamaz");
            return Ok(tesis);

        }




    }
}
