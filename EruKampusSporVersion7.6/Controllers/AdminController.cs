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
    public class AdminController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult adminEkle([FromQuery] int tesisId,[FromQuery] string adminKullanıcıAdı, [FromQuery] string adminPassword)
        {
            var tesis=_context.Tesisler.FirstOrDefault(t=>t.TesisId==tesisId);

            if (tesis == null)
                return BadRequest("Böyle bir tesis yoktur");
            var existingAdmin = _context.Adminler.FirstOrDefault(a => a.TesisId == tesisId);
            if (existingAdmin != null)
                return BadRequest("Zaten böyle bir zaten admin var!");

            Admin admin = new Admin { TesisId = tesisId, adminKullanıcıAdı = adminKullanıcıAdı, password = adminPassword, };
            _context.Adminler.Add(admin);
            _context.SaveChanges();
            return Ok(admin);




        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult getTesisOfAdmin([FromQuery] int adminId)
        {

            var tesis = _context.Tesisler.Include(t => t.Admin).FirstOrDefault(t => t.Admin.Id == adminId);
            if (tesis == null)
                return BadRequest("Böyle bir admin veya tesis yoktur");
            TesisDTO tesisDTO = new TesisDTO()
            {
                TesisId = tesis.TesisId,
                TesisAdı = tesis.TesisAdı
            };

            return Ok(tesisDTO);


            
        }
    }
}
