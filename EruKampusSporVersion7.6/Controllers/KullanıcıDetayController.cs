using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
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
    [Route("api/KullaniciDetayController/[action]")]
    [ApiController]
    //[Authorize]
    public class KullanıcıDetayController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly Cloudinary _cloudinary;
        public KullanıcıDetayController(ApplicationDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }
        [HttpPut()]
        [Authorize(Policy = "UserOnly")]
        public IActionResult UpdateKullaniciDetay([FromBody] KullanıcıDetayDTO yeniKullanıcıDetay) 
        {
            //var kullaniciId = yeniKullanıcıDetay.Id;
            

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            // Token'daki ID'yi int'e dönüştür
            if (!int.TryParse(userIdClaim, out var id))
                return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            var kullaniciId = id;

            var kullanıcıDetayı =  _context.KullanıcıDetayları.Find(kullaniciId); 
            if (kullanıcıDetayı == null) 
            {
                return NotFound($"ID'si {kullaniciId} olan kullanıcı bulunamadı.");
            }

            
            var mevcutKullanıcıDetay = _context.KullanıcıDetayları.Include(kd => kd.Kullanıcı).FirstOrDefault(kd => kd.Id == kullaniciId);
                

            if (mevcutKullanıcıDetay == null)
            {
                return NotFound($"ID'si {kullaniciId} olan kullanıcıya ait adres bulunamadı.");
            }


            if (!(yeniKullanıcıDetay.telefon.StartsWith("+")))
                return BadRequest("Telefon Numaraları + ile başlamalıdır");


            kullanıcıDetayı.Adres = yeniKullanıcıDetay.adres;
            kullanıcıDetayı.EMail =yeniKullanıcıDetay.email;
            kullanıcıDetayı.Telefon =yeniKullanıcıDetay.telefon;
            _context.SaveChanges();

            //KullanıcıDetayDTO güncellenmisKullanıcıDetayları=new KullanıcıDetayDTO();
            //güncellenmisKullanıcıDetayları.name = kullanıcıDetayı.Kullanıcı!.Name;
            //güncellenmisKullanıcıDetayları.adres = kullanıcıDetayı.Adres;
            //güncellenmisKullanıcıDetayları.telefon = kullanıcıDetayı.Telefon;
            //güncellenmisKullanıcıDetayları.email=kullanıcıDetayı.EMail;
            //güncellenmisKullanıcıDetayları.cinsiyet= kullanıcıDetayı.Cinsiyet;






            return Ok("Sporcu bilgileri başarılı bir şekilde güncellenmiştir");


        }



        [HttpPut()]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult UpdateKullaniciDetayAdmin([FromBody] KullaniciGuncellemeAdminDTO kullaniciGuncellemeAdminDTO, [FromQuery] int kullaniciId)
        {
            ////var kullaniciId = yeniKullanıcıDetay.Id;


            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdClaim))
            //    return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            //// Token'daki ID'yi int'e dönüştür
            //if (!int.TryParse(userIdClaim, out var id))
            //    return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            //var kullaniciId = id;

            var kullaniciDetayi = _context.KullanıcıDetayları.Find(kullaniciId);
            if (kullaniciDetayi == null)
            {
                return NotFound($"ID'si {kullaniciId} olan kullanıcı bulunamadı.");
            }


            var mevcutKullaniciDetay = _context.KullanıcıDetayları.Include(kd => kd.Kullanıcı).FirstOrDefault(kd => kd.Id == kullaniciId);


            if (mevcutKullaniciDetay == null)
            {
                return NotFound($"ID'si {kullaniciId} olan kullanıcıya ait adres bulunamadı.");
            }


            if (!(kullaniciGuncellemeAdminDTO.telefon.StartsWith("+")))
                return BadRequest("Telefon Numaraları + ile başlamalıdır");


            kullaniciDetayi.Adres = kullaniciGuncellemeAdminDTO.adres;
            kullaniciDetayi.EMail = kullaniciGuncellemeAdminDTO.email;
            kullaniciDetayi.Telefon = kullaniciGuncellemeAdminDTO.telefon;
            _context.SaveChanges();

            //KullanıcıDetayDTO güncellenmisKullanıcıDetayları=new KullanıcıDetayDTO();
            //güncellenmisKullanıcıDetayları.name = kullanıcıDetayı.Kullanıcı!.Name;
            //güncellenmisKullanıcıDetayları.adres = kullanıcıDetayı.Adres;
            //güncellenmisKullanıcıDetayları.telefon = kullanıcıDetayı.Telefon;
            //güncellenmisKullanıcıDetayları.email=kullanıcıDetayı.EMail;
            //güncellenmisKullanıcıDetayları.cinsiyet= kullanıcıDetayı.Cinsiyet;






            return Ok("Sporcu bilgileri başarılı bir şekilde güncellenmiştir");


        }

        //[HttpGet()]
        //[Authorize(Policy = "AdminOrUser")]
        //public IActionResult GetKullanıcıDetay([FromQuery]int ID)
        //{


        //    var kullanıcıDetayı = _context.KullanıcıDetayları.Find(ID);
        //    if (kullanıcıDetayı == null)
        //        return NotFound("Böyle bir kullanıcı yoktur");
        //    return Ok(kullanıcıDetayı);
        //}

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public IActionResult GetKullaniciDetay()
        {
            // Token'daki ID'yi al
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            // Token'daki ID'yi int'e dönüştür
            if (!int.TryParse(userIdClaim, out var id))
                return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            var userId = id;

            // Veritabanından kullanıcıyı getir
            var kullanıcıDetayı = _context.KullanıcıDetayları
                .Include(kd => kd.Kullanıcı) // Kullanıcıyı dahil et
                .Where(kd => kd.Id == userId)  // KullanıcıDetayId'si ile filtrele
                .Select(kd => new
                {
                    //kd.Id, // KullanıcıDetayId'sini dahil ediyoruz
                    kd.Kullanıcı.Name,
                    //kd.Kullanıcı.TC,
                    kd.Adres,
                    kd.Telefon,
                    kd.EMail,
                    kd.Cinsiyet,
                    kd.ProfilFotografiUrl
                    
                })
                .SingleOrDefault(); // İlk bulduğu değeri al

            if (kullanıcıDetayı == null)
                return NotFound("Böyle bir kullanıcı yoktur");

            return Ok(kullanıcıDetayı);
        }

        [HttpGet()]
        [Authorize(Policy = "AdminOrUser")]
        public IActionResult GetAllKullanıcıDetayları()
        {
           var kullanıcıDetayları= _context.KullanıcıDetayları;
            if (kullanıcıDetayları == null)
                return Ok("Hiç bir kullanıcı sistemde kayıtlı olmadığı için detay bilgisi yoktur");
            return Ok(kullanıcıDetayları);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetKullaniciDetayAdmin([FromQuery] int kullaniciId)
        {
            

            var userId = kullaniciId;

            // Veritabanından kullanıcıyı getir
            var kullanıcıDetayı = _context.KullanıcıDetayları
                .Include(kd => kd.Kullanıcı) // Kullanıcıyı dahil et
                .Where(kd => kd.Id == userId)  // KullanıcıDetayId'si ile filtrele
                .Select(kd => new
                {
                    //kd.Id, // KullanıcıDetayId'sini dahil ediyoruz
                    kd.Kullanıcı.Name,
                    //kd.Kullanıcı.TC,
                    kd.Adres,
                    kd.Telefon,
                    kd.EMail,
                    kd.Cinsiyet,
                    kd.ProfilFotografiUrl
                })
                .SingleOrDefault(); // İlk bulduğu değeri al

            if (kullanıcıDetayı == null)
                return NotFound("Böyle bir kullanıcı yoktur");

            return Ok(kullanıcıDetayı);
        }

        [HttpPost()]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UploadProfilFoto(IFormFile file, [FromQuery] int kullaniciId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Geçersiz dosya.");
            }

            // Desteklenen formatları belirle
            var allowedFormats = new List<string> { "image/jpeg", "image/png", "image/bmp" };

            if (!allowedFormats.Contains(file.ContentType.ToLower()))
            {
                return BadRequest("Sadece JPEG, PNG ve BMP formatındaki fotoğraflar yüklenebilir.");
            }


            // Cloudinary'ye yükleme
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "EruKampusSporProfilPhotos" // Cloudinary'deki klasör adı
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return StatusCode(500, "Fotoğraf yüklenemedi.");
            }

            // URL'yi KullaniciDetay tablosuna kaydet
            //var kullaniciId = int.Parse(User.FindFirst("id").Value); // Token'dan kullanıcı ID'si alınır
            var kullaniciDetay = await _context.KullanıcıDetayları.FirstOrDefaultAsync(k => k.Id == kullaniciId);

            if (kullaniciDetay == null)
            {
                return NotFound("Kullanıcı detayları bulunamadı.");
            }

            kullaniciDetay.ProfilFotografiUrl = uploadResult.SecureUrl.ToString();
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Fotoğraf başarıyla yüklendi.", Url = kullaniciDetay.ProfilFotografiUrl });
        }

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> GetProfilFoto()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            // Token'daki ID'yi int'e dönüştür
            if (!int.TryParse(userIdClaim, out var id))
                return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            var kullaniciId = id;

            // Veritabanında KullanıcıDetay tablosundan ilgili kaydı bul
            var kullaniciDetay = await _context.KullanıcıDetayları
                                               .FirstOrDefaultAsync(k => k.Id == kullaniciId);

            if (kullaniciDetay == null || string.IsNullOrEmpty(kullaniciDetay.ProfilFotografiUrl))
            {
                return NotFound("Profil fotoğrafı bulunamadı.");
            }

            // Profil fotoğrafı URL'sini döndür
            return Ok(kullaniciDetay.ProfilFotografiUrl);
        }
    }
}
