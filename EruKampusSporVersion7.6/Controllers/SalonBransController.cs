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
    public class SalonBransController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        public SalonBransController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("{salonId}/branslar")]
        [Authorize(Policy = "AdminOrUser")]
        public IActionResult GetBranslarBySalon(int salonId)
        {
            var branslar = _context.SalonBrans
                .Where(sb => sb.SalonId == salonId)
                .Include(sb => sb.Brans)
                .Select(sb => sb.Brans)
                .ToList();

            if (!branslar.Any()) return NotFound();

            return Ok(branslar);
        }

        [HttpPost]
        [Route("api/salon-brans")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult AddSalonBrans([FromBody] SalonBransDTO salonBransDTO)
        {
            if (salonBransDTO == null) return BadRequest();
            if (!ModelState.IsValid) // Model doğrulama başarısız
            {
                return BadRequest(ModelState); // Hataları döndür
            }

            var salon = _context.Salonlar.FirstOrDefault(s => s.SalonId == salonBransDTO.SalonId);
            var brans = _context.Branslar.FirstOrDefault(b => b.BransId == salonBransDTO.BransId);
            if (salon == null && brans == null) return BadRequest();

            SalonBrans salonBrans=new SalonBrans();
            salonBrans.SalonId = salonBransDTO.SalonId;
            salonBrans.BransId = salonBransDTO.BransId;







            _context.SalonBrans.Add(salonBrans);
            _context.SaveChanges();

            return Ok(salonBransDTO);
        }


    }
}
