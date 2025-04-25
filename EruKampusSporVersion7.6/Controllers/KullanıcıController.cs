using EruKampusSpor.Data;
using EruKampusSpor.DTOs;
using EruKampusSpor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Web;

namespace EruKampusSpor.Controllers
{
    [Route("api/Kullanici/[action]")]
    [ApiController]
    
    public class KullanıcıController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KullanıcıController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet()]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetKullanıcıById(int Id)
        {



            var kullanıcı = _context.Kullanıcılar.FirstOrDefault(k => k.KullanıcıId == Id);

            if (kullanıcı == null)
                return NotFound("Kullanıcı bulunamadı");
            return Ok(kullanıcı);
        }
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAllUser()
        {
            try
            {
                // Kullanıcılar listesini veritabanından çekiyoruz.
                var users = _context.Kullanıcılar.ToList(); //Select * from Users


                // Liste boşsa 404 döndür
                if (users == null || !users.Any())
                {
                    return NotFound("Kullanıcı bulunamadı.");
                }

                //İleride Kullanıcı DTO'ları gönder.

                // Başarılı durum koduyla listeyi döndür
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Hata durumunda 500 döndür
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }


        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult CreateUser([FromBody] KullanıcıDTO kullanıcıDTO)
        {
           
            if (kullanıcıDTO == null)
                return BadRequest();
            if (kullanıcıDTO.TC.Length != 11)
                return BadRequest();
            bool IsTcMatching = false;
            string matchingName;

            var appKullanıcısıTcleri = _context.Kullanıcılar.Select(k => k.TC).ToList();
            foreach (var appKullanıcısıTc in appKullanıcısıTcleri)
            {
                if (kullanıcıDTO.TC == appKullanıcısıTc)
                {
                    return BadRequest("Kullanıcı zaten kayıtlı");
                }
            }


            foreach (var eruMember in ApplicationDbContext.bilgiIslem)
            {
                if (eruMember.TC == kullanıcıDTO.TC)
                {
                    IsTcMatching = true;
                    matchingName = eruMember.name;
                    break;
                }
            }
            if (!IsTcMatching)
                return NotFound("You are not related to Erciyes University");


            var target = ApplicationDbContext.bilgiIslem.FirstOrDefault(x => x.TC == kullanıcıDTO.TC); //LINQ Kullanımı
            Kullanıcı kullanıcı = new() { TC = target.TC, Name = target.name, };
            kullanıcı.KullanıcıDetay = new()
            {

                Adres = "Default Kayseri Adresi",
                Telefon = "Default Telefon Numarası",
                EMail = "Default Mail Adresi",
                password = target.obisis_password,
                Cinsiyet = target.Cinsiyet

            };



            _context.Kullanıcılar.Add(kullanıcı);
            _context.SaveChanges();



            return Ok(kullanıcı);

        }

        [HttpDelete]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteUser([FromBody] int id)
        {
            try
            {
                // Kullanıcıyı id ile bul
                var user = _context.Kullanıcılar.FirstOrDefault(u => u.KullanıcıId == id);

                // Eğer kullanıcı yoksa, 404 döndür
                if (user == null)
                {
                    return NotFound($"Id'si {id} olan kullanıcı bulunamadı.");
                }

                // Kullanıcıyı veritabanından sil
                _context.Kullanıcılar.Remove(user);
                _context.SaveChanges();

                // Başarılı durum kodu ile mesaj döndür
                return Ok($"Id'si {id} olan kullanıcı başarıyla silindi.");
            }
            catch (Exception ex)
            {
                // Hata durumunda 500 döndür
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }


        //[HttpGet]

        //public IActionResult Login(string Tc, string password)
        //{

        //    bool tcMatching = false;
        //    bool passwordMatching = false;

        //    Kullanıcı? kullanıcı = _context.Kullanıcılar.FirstOrDefault(k => k.TC == Tc);

        //    if (kullanıcı == null)
        //        return NotFound("Lütfen kullanıcı Tc'sini doğru giriniz");

        //    tcMatching = true;

        //    KullanıcıDetay? kullanıcıDetayı = _context.KullanıcıDetayları.Find(kullanıcı.KullanıcıId);

        //    if (kullanıcıDetayı.password == password)
        //        return Ok("Giriş Başarılı");

        //    return NotFound("Lütfen şifrenizi doğru girdiğinizden emin olunuz");

        //}

        [HttpPost]
        //[Authorize(Policy = "AdminOrUser")]
        public IActionResult Register(string TC) {

            


            var appKullanıcısıTcleri = _context.Kullanıcılar.Select(k => k.TC).ToList();
            foreach (var appKullanıcısıTc in appKullanıcısıTcleri)
            {
                if (TC == appKullanıcısıTc)
                {
                    return BadRequest("Kullanıcı zaten kayıtlı");
                }
            }

            bool IsTcMatching = false;
            string matchingName;

            foreach (var eruMember in ApplicationDbContext.bilgiIslem)
            {
                if (eruMember.TC == TC)
                {
                    IsTcMatching = true;
                    matchingName = eruMember.name;
                    break;
                }
            }
            if (!IsTcMatching)
                return NotFound("You are not related to Erciyes University");


            var target = ApplicationDbContext.bilgiIslem.FirstOrDefault(x => x.TC == TC); //LINQ Kullanımı

            

            Kullanıcı kullanıcı = new() { TC = target.TC, Name = target.name, };
            kullanıcı.KullanıcıDetay = new()
            {

                Adres = "Default Kayseri Adresi",
                Telefon = "Default Telefon Numarası",
                EMail = "Default Mail Adresi",
                password = target.obisis_password,
                Cinsiyet = target.Cinsiyet,
                ProfilFotografiUrl = ""

            };

            if (kullanıcı.KullanıcıDetay.Cinsiyet == Cinsiyet.Kadın)
            {
                kullanıcı.KullanıcıDetay.ProfilFotografiUrl = "https://res.cloudinary.com/davayayg8/image/upload/v1737917826/defaultKad%C4%B1nSporcuProfilPhoto_fstyqz.jpg";
            }
            else
            {
                kullanıcı.KullanıcıDetay.ProfilFotografiUrl = "https://res.cloudinary.com/davayayg8/image/upload/v1737917807/defaultErkekSporcuProfilPhoto_cknkam.jpg";
            }


            _context.Kullanıcılar.Add(kullanıcı);
            _context.SaveChanges();



            return Ok(kullanıcı);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult adminKullaniciKayıt(string TC)
        {




            var appKullanıcısıTcleri = _context.Kullanıcılar.Select(k => k.TC).ToList();
            foreach (var appKullanıcısıTc in appKullanıcısıTcleri)
            {
                if (TC == appKullanıcısıTc)
                {
                    return BadRequest("Kullanıcı zaten kayıtlı");
                }
            }

            bool IsTcMatching = false;
            string matchingName;

            foreach (var eruMember in ApplicationDbContext.bilgiIslem)
            {
                if (eruMember.TC == TC)
                {
                    IsTcMatching = true;
                    matchingName = eruMember.name;
                    break;
                }
            }
            if (!IsTcMatching)
                return NotFound("You are not related to Erciyes University");


            var target = ApplicationDbContext.bilgiIslem.FirstOrDefault(x => x.TC == TC); //LINQ Kullanımı
            Kullanıcı kullanıcı = new() { TC = target.TC, Name = target.name, };
            kullanıcı.KullanıcıDetay = new()
            {

                Adres = "Default Kayseri Adresi",
                Telefon = "Default Telefon Numarası",
                EMail = "Default Mail Adresi",
                password = target.obisis_password,
                Cinsiyet = target.Cinsiyet,
                ProfilFotografiUrl=""

            };

            if (kullanıcı.KullanıcıDetay.Cinsiyet == Cinsiyet.Kadın)
            {
                kullanıcı.KullanıcıDetay.ProfilFotografiUrl = "https://res.cloudinary.com/davayayg8/image/upload/v1737917826/defaultKad%C4%B1nSporcuProfilPhoto_fstyqz.jpg";
            }
            else
            {
                kullanıcı.KullanıcıDetay.ProfilFotografiUrl = "https://res.cloudinary.com/davayayg8/image/upload/v1737917807/defaultErkekSporcuProfilPhoto_cknkam.jpg";
            }



            _context.Kullanıcılar.Add(kullanıcı);
            _context.SaveChanges();



            return Ok(kullanıcı);
        }

        [HttpGet]
        [Authorize(Policy ="AdminOnly")]
        public IActionResult kullanicilarAramaTC(string tc)
        {
            // TC'nin uzunluğunu kontrol et
            if (tc.Length > 11)
            {
                return BadRequest("Geçersiz TC numarası! 11 haneli olmalı.");
            }

            if(!tc.All(char.IsDigit))
            {
                return BadRequest("TC numarası haneleri sadece rakamlardan oluşmalıdır");
            }

            // TC'nin 1 ile 10 haneli arasında olduğunu kontrol et
            if (tc.Length >= 1 && tc.Length <= 10)
            {
                var users = _context.Kullanıcılar
                    .Where(u => u.TC.StartsWith(tc))  // Kısmi eşleşme
                    .ToList();

                if (users.Any())
                {
                    return Ok(users);
                }

                return NotFound("Kullanıcı bulunamadı.");
            }

            // TC'nin 11 haneli olduğu durumda da kısmi eşleşme yapılacak
            if (tc.Length == 11)
            {
                var users = _context.Kullanıcılar
                    .Where(u => u.TC.StartsWith(tc))  // 11 haneli için de kısmi eşleşme
                    .ToList();

                if (users.Any())
                {
                    return Ok(users);
                }

                return NotFound("Kullanıcı bulunamadı.");
            }

            return BadRequest("Geçersiz TC numarası! 1 ile 11 hane arasında olmalı.");
        }


        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult kullanicilarAramaIsim(string isimSoyisim)
        {
            var decodedIsimSoyisim = HttpUtility.UrlDecode(isimSoyisim).ToLower();

            // Parametrenin uzunluğunu kontrol et
            if (string.IsNullOrWhiteSpace(decodedIsimSoyisim))
            {
                return BadRequest("Geçersiz isim veya soyisim! Değer boş olamaz.");
            }

            if (decodedIsimSoyisim.Length > 100)
            {
                return BadRequest("Geçersiz isim veya soyisim! Maksimum 100 karakter olmalı.");
            }

            // Hem tam eşleşme hem de kısmi eşleşme yap
            var users = _context.Kullanıcılar
                .Where(u => u.Name.ToLower().StartsWith(decodedIsimSoyisim.ToLower()) || u.Name.ToLower() == decodedIsimSoyisim.ToLower())  // Tam eşleşme ve kısmi eşleşme
                .ToList();

            if (users.Any())
            {
                return Ok(users);
            }

            return NotFound("Kullanıcı bulunamadı.");
        }




    }


}
