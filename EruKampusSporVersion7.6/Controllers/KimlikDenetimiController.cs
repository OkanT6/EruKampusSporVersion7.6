using EruKampusSpor.Data;
using EruKampusSpor.DTOs;
using EruKampusSpor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EruKampusSpor.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KimlikDenetimiController : ControllerBase
    {
        private readonly JwtAyarlari _jwtAyarlari;
        private readonly ApplicationDbContext _context;
        public KimlikDenetimiController(IOptions<JwtAyarlari> jwtAyarlari, ApplicationDbContext context)
        {
            _jwtAyarlari=jwtAyarlari.Value;
            _context=context;
        }

        [HttpPost]
        public IActionResult kullaniciGirisi([FromBody] KimlikKullanıcıDTO apiKullanıcıBilgileri)
        {
            var apiKullanicisi = KullaniciKimlikDenetimiYap(apiKullanıcıBilgileri);
            if (apiKullanicisi == null)
                return NotFound("Kullanıcı adı veya şifresi yanlış");

            var token= KullanıcıTokenOlustur(apiKullanicisi);
            return Ok(token);

        }
        [HttpPost]
        public IActionResult adminGirisi([FromBody] AdminDTO apiAdminBilgileri)
        {
            var apiAdmin = AdminKimlikDenetimiYap(apiAdminBilgileri);
            if (apiAdmin == null)
                return NotFound("Admin adı veya şifresi yanlış");

            var token = AdminTokenOlustur(apiAdmin);
            return Ok(token);

        }

        private string KullanıcıTokenOlustur(Kullanıcı apiKullanicisi)
        {
            if (_jwtAyarlari.Key == null)
                throw new Exception("Jwt Ayarlarındaki key değeri null olamaz");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAyarlari.Key));
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256 );
            var claimsDizisi = new[]
{
            new Claim(ClaimTypes.Name, apiKullanicisi.Name), // Kullanıcının adı
            new Claim(ClaimTypes.NameIdentifier, apiKullanicisi.KullanıcıId.ToString()), // Kullanıcı ID'si
            new Claim(ClaimTypes.Role, "User") // Rol
};

            var token = new JwtSecurityToken(
                _jwtAyarlari.Issuer,
                _jwtAyarlari.Audience,
                claimsDizisi,
                expires: DateTime.Now.AddHours(1.0),
                signingCredentials:credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string AdminTokenOlustur(Admin apiAdmin)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAyarlari.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsDizisi = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, apiAdmin.Id.ToString()), // Kullanıcı ID'si
        new Claim(ClaimTypes.Role, "Admin") // Admin rolü ekleniyor
    };

            var token = new JwtSecurityToken(
                _jwtAyarlari.Issuer,
                _jwtAyarlari.Audience,
                claimsDizisi,
                expires: DateTime.Now.AddHours(1.0),
                signingCredentials: credentials

            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private Kullanıcı? KullaniciKimlikDenetimiYap(KimlikKullanıcıDTO apiKullanıcıBilgileri)
        {
            var kullanıcı = _context.Kullanıcılar.Include(k=>k.KullanıcıDetay).FirstOrDefault(k=>k.TC==apiKullanıcıBilgileri.TC);
            //var admin= _context.Adminler.FirstOrDefault(a => a.adminTC == apiKullanıcıBilgileri.TC);

            if (kullanıcı == null /*|| admin==null*/)
                return null;
            if (kullanıcı.KullanıcıDetay.password != apiKullanıcıBilgileri.password)
                return null;
            return kullanıcı;

        }
        private Admin? AdminKimlikDenetimiYap(AdminDTO apiAdminBilgileri)
        {
            var admin = _context.Adminler.FirstOrDefault(a => a.adminKullanıcıAdı == apiAdminBilgileri.adminKullanıcıAdı && a.password==apiAdminBilgileri.password);
            //var admin= _context.Adminler.FirstOrDefault(a => a.adminTC == apiKullanıcıBilgileri.TC);

            if (admin == null)
                return null;
            return admin;

        }
    }
}
