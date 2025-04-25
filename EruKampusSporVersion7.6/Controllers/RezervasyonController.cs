using EruKampusSpor.Data;
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
    public class RezervasyonController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        public RezervasyonController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAllRezervasyonlar()
        {
            var rezervasyonlar = _context.Rezervasyonlar.ToList();

            if (rezervasyonlar == null)
                return NotFound("Hiç bir rezervasyon yok");
            return Ok(rezervasyonlar);
        }


        [HttpPost]
        [Authorize(Policy = "UserOnly")]
        public IActionResult rezervasyonYap([FromQuery] int seansId, [FromQuery] int bransId, [FromQuery] int salonId)
        {


            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            // Token'daki ID'yi int'e dönüştür
            if (!int.TryParse(userIdClaim, out var id))
                return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            var kullanıcıId = id;


            var kullanıcı = _context.Kullanıcılar.Include(k => k.KullanıcıDetay).FirstOrDefault(k => k.KullanıcıId == kullanıcıId);
                

            var seans=_context.Seanslar.FirstOrDefault(s=>s.SeansId== seansId);



            
            if (kullanıcı == null)
                return BadRequest("Böyle bir kullanıcı yoktur");
            if (seans == null)
                return BadRequest("Böyle bir seans yoktur");

            var arananSalonunBransSayısı = _context.SalonBrans.Where(sb => sb.SalonId == salonId).Count();
            var brans =_context.Branslar.Find(bransId);
            if(brans == null)
                return BadRequest("Boyle bir brans yoktur");
            string bransAdı = brans.BransAdı;

            if (arananSalonunBransSayısı > 1 && seans.RezerveEdenKisiSayisi >= 1)
            {
                if (seans.yapılanBrans != bransAdı)
                    return BadRequest("Bu seans saati sadece " + seans.yapılanBrans + " bransı icin uygundur");
            }


            if (seans.SeansCinsiyet != SeansCinsiyet.Karma)
            { 
                if(kullanıcı.KullanıcıDetay.Cinsiyet.ToString()!=seans.SeansCinsiyet.ToString())
                {
                    if (kullanıcı.KullanıcıDetay.Cinsiyet.ToString() == "Erkek")
                        return StatusCode(400, "Bu seans sadece kadınlara özeldir");
                    else return StatusCode(400, "Bu seans sadece erkeklere özeldir");

                }

            }




            var rezerveEdilmisAynıSeans2= _context.Rezervasyonlar.FirstOrDefault(r => r.SeansId == seansId && r.KullanıcıId == kullanıcıId);
            if (rezerveEdilmisAynıSeans2 != null && rezerveEdilmisAynıSeans2.IptalEdildi == false)
                return BadRequest("Aynı seansı zaten rezerve etmişsin.");


            if (seans.Dolu == true)
                return BadRequest("Seçilen seans doludur.");

            
            var rezerveEdilmisAynıSeans=_context.Rezervasyonlar.FirstOrDefault(r => r.SeansId == seansId && r.KullanıcıId == kullanıcıId);

            if (rezerveEdilmisAynıSeans != null && rezerveEdilmisAynıSeans.IptalEdildi==false)
                return BadRequest("Aynı seansı zaten rezerve etmiş bulunmaktasınız.");
            else if (rezerveEdilmisAynıSeans != null && rezerveEdilmisAynıSeans.IptalEdildi == true)
            {

                _context.Rezervasyonlar.Remove(rezerveEdilmisAynıSeans);
                _context.SaveChanges();
                var yeniRezervasyon2 = new Rezervasyon
                {
                    KullanıcıId = kullanıcıId,
                    SeansId = seansId
                };

                seans.RezerveEdenKisiSayisi++;
                if (seans.Kontenjan == seans.RezerveEdenKisiSayisi)
                    seans.Dolu = true;

                var bransadı2 = _context.SalonBrans
                .Where(sb => sb.BransId == bransId) // bransId'yi koşul olarak kullanıyoruz
                .Include(sb => sb.Brans)
                .Select(sb => sb.Brans.BransAdı)
                .FirstOrDefault();
                seans.yapılanBrans = bransadı2;
                _context.Rezervasyonlar.Add(yeniRezervasyon2);
                _context.SaveChanges();


                //var rezervasyonBilgileri2 = _context.Rezervasyonlar.Where(r => r.KullanıcıId == kullanıcıId).Include(r => r.Seans).ToList();
                return Ok("İptal ettiğiniz seans yeniden başarılı bir şekilde yeniden rezerve edilmiştir.");


            }

            var yeniRezervasyon = new Rezervasyon { 
            KullanıcıId= kullanıcıId,
                SeansId=seansId
            };
            seans.RezerveEdenKisiSayisi++;
            if (seans.Kontenjan==seans.RezerveEdenKisiSayisi)
                seans.Dolu = true;

            var bransAdı2 = _context.SalonBrans
            .Where(sb => sb.BransId == bransId) // bransId'yi koşul olarak kullanıyoruz
            .Include(sb => sb.Brans)
            .Select(sb => sb.Brans.BransAdı)
            .FirstOrDefault();
            seans.yapılanBrans = bransAdı2;

            _context.Rezervasyonlar.Add(yeniRezervasyon);
            _context.SaveChanges();

            
            //var rezervasyonBilgileri=_context.Rezervasyonlar.Where(r=>r.KullanıcıId==kullanıcıId).Include(r=>r.Seans).ToList();
            return Ok("Seans başarılı bir şekilde rezerve edilmiştir.");


            //// Parametreleri kullanarak işlemler yapabilirsiniz.
            //return Ok(new { Id = id, Name = name });


            



        }
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult rezervasyonYapAdmin([FromQuery] int kullaniciId, [FromQuery] int seansId, [FromQuery] int bransId, [FromQuery] int salonId)
        {
            int kullanıcıId = kullaniciId;


            var kullanıcı = _context.Kullanıcılar.Include(k => k.KullanıcıDetay).FirstOrDefault(k => k.KullanıcıId == kullanıcıId);


            var seans = _context.Seanslar.FirstOrDefault(s => s.SeansId == seansId);




            if (kullanıcı == null)
                return BadRequest("Böyle bir kullanıcı yoktur");
            if (seans == null)
                return BadRequest("Böyle bir seans yoktur");

            var arananSalonunBransSayısı = _context.SalonBrans.Where(sb => sb.SalonId == salonId).Count();
            var brans = _context.Branslar.Find(bransId);
            if (brans == null)
                return BadRequest("Boyle bir brans yoktur");
            string bransAdı = brans.BransAdı;

            if (arananSalonunBransSayısı > 1 && seans.RezerveEdenKisiSayisi >= 1)
            {
                if (seans.yapılanBrans != bransAdı)
                    return BadRequest("Bu seans saati sadece " + seans.yapılanBrans + " bransı icin uygundur");
            }


            if (seans.SeansCinsiyet != SeansCinsiyet.Karma)
            {
                if (kullanıcı.KullanıcıDetay.Cinsiyet.ToString() != seans.SeansCinsiyet.ToString())
                {
                    if (kullanıcı.KullanıcıDetay.Cinsiyet.ToString() == "Erkek")
                        return BadRequest("Bu seans sadece kadınlara özeldir");
                    else return BadRequest("Bu seans sadece erkeklere özeldir");
                }

            }





            if (seans.Dolu == true)
                return BadRequest("Seans Doludur");


            var rezerveEdilmisAynıSeans = _context.Rezervasyonlar.FirstOrDefault(r => r.SeansId == seansId && r.KullanıcıId == kullanıcıId);

            if (rezerveEdilmisAynıSeans != null && rezerveEdilmisAynıSeans.IptalEdildi == false)
                return BadRequest("Aynı seansı zaten rezerve etmişsin.");
            else if (rezerveEdilmisAynıSeans != null && rezerveEdilmisAynıSeans.IptalEdildi == true)
            {

                _context.Rezervasyonlar.Remove(rezerveEdilmisAynıSeans);
                _context.SaveChanges();
                var yeniRezervasyon2 = new Rezervasyon
                {
                    KullanıcıId = kullanıcıId,
                    SeansId = seansId
                };

                seans.RezerveEdenKisiSayisi++;
                if (seans.Kontenjan == seans.RezerveEdenKisiSayisi)
                    seans.Dolu = true;

                var bransadı2 = _context.SalonBrans
                .Where(sb => sb.BransId == bransId) // bransId'yi koşul olarak kullanıyoruz
                .Include(sb => sb.Brans)
                .Select(sb => sb.Brans.BransAdı)
                .FirstOrDefault();
                seans.yapılanBrans = bransadı2;
                _context.Rezervasyonlar.Add(yeniRezervasyon2);
                _context.SaveChanges();


                var rezervasyonBilgileri2 = _context.Rezervasyonlar.Where(r => r.KullanıcıId == kullanıcıId).Include(r => r.Seans).ToList();
                return Ok(yeniRezervasyon2);


            }

            var yeniRezervasyon = new Rezervasyon
            {
                KullanıcıId = kullanıcıId,
                SeansId = seansId
            };
            seans.RezerveEdenKisiSayisi++;
            if (seans.Kontenjan == seans.RezerveEdenKisiSayisi)
                seans.Dolu = true;

            var bransAdı2 = _context.SalonBrans
            .Where(sb => sb.BransId == bransId) // bransId'yi koşul olarak kullanıyoruz
            .Include(sb => sb.Brans)
            .Select(sb => sb.Brans.BransAdı)
            .FirstOrDefault();
            seans.yapılanBrans = bransAdı2;

            _context.Rezervasyonlar.Add(yeniRezervasyon);
            _context.SaveChanges();


            //var rezervasyonBilgileri = _context.Rezervasyonlar.Where(r => r.KullanıcıId == kullanıcıId).Include(r => r.Seans).ToList();
            return Ok("Seans başarılı bir şekilde rezerve edilmiştir.");


            //// Parametreleri kullanarak işlemler yapabilirsiniz.
            //return Ok(new { Id = id, Name = name });






        }

        [HttpPut]
        [Authorize(Policy = "UserOnly")]
        public IActionResult RezervasyonIptal([FromQuery] int SeansId)
        {
            string iptalEdilenBrans;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            // Token'daki ID'yi int'e dönüştür
            if (!int.TryParse(userIdClaim, out var id))
                return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            var KullanıcıId = id;


            var kullanıcı = _context.Kullanıcılar.Include(k => k.KullanıcıDetay).FirstOrDefault(k => k.KullanıcıId == KullanıcıId);


            var seans = _context.Seanslar.FirstOrDefault(s => s.SeansId == SeansId);

            


            if (kullanıcı == null)
                return BadRequest("Böyle bir kullanıcı yoktur");
            if (seans == null)
                return BadRequest("Böyle bir seans yoktur");


            var rezervasyon = _context.Rezervasyonlar
    .FirstOrDefault(r => r.KullanıcıId == KullanıcıId && r.SeansId == SeansId);

            if (rezervasyon == null)
                return BadRequest("Böyle bir rezervasyonun yoktur");

            if(rezervasyon.IptalEdildi==true)
            {
                return BadRequest("Zaten rezervasyonunuzu iptal ettiniz");
            }

            

            DateTime time1= DateTime.UtcNow.AddHours(3);
            var time2= seans.SeansBaslangicZamani.AddHours(-24);

            if (time1 > time2)
                return BadRequest("Başlangıç saatine 24 saatten az kalan seansların rezervasyonları iptal edilemez.");

            rezervasyon.IptalEdildi=true;
            iptalEdilenBrans = seans.yapılanBrans;
            rezervasyon.IptalEdilenBrans = iptalEdilenBrans;

            seans.RezerveEdenKisiSayisi--;
            if (seans.Dolu == true)
                seans.Dolu =false;


            
            _context.SaveChanges();

            var aynıSeans = _context.Seanslar.FirstOrDefault(s => s.SeansId == SeansId);

            if(aynıSeans.RezerveEdenKisiSayisi==0)
                seans.yapılanBrans=null;

            _context.SaveChanges();




            //var rezervasyonBilgileri = _context.Rezervasyonlar.Where(r => r.KullanıcıId == KullanıcıId).Include(r => r.Seans).ToList();
            return Ok("Rezervasyonunuz başarılı bir şekilde iptal edilmiştir.");





        }

        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult RezervasyonIptalAdmin([FromQuery] int kullaniciId,[FromQuery] int SeansId)
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdClaim))
            //    return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            //// Token'daki ID'yi int'e dönüştür
            //if (!int.TryParse(userIdClaim, out var id))
            //    return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            var KullanıcıId = kullaniciId;
            string iptalEdilenBrans;



            var kullanıcı = _context.Kullanıcılar.Include(k => k.KullanıcıDetay).FirstOrDefault(k => k.KullanıcıId == KullanıcıId);


            var seans = _context.Seanslar.FirstOrDefault(s => s.SeansId == SeansId);




            if (kullanıcı == null)
                return BadRequest("Böyle bir kullanıcı yoktur");
            if (seans == null)
                return BadRequest("Böyle bir seans yoktur");


            var rezervasyon = _context.Rezervasyonlar
    .FirstOrDefault(r => r.KullanıcıId == KullanıcıId && r.SeansId == SeansId);

            if (rezervasyon == null)
                return BadRequest("Böyle bir rezervasyonun yoktur");

            if (rezervasyon.IptalEdildi == true)
            {
                return BadRequest("Zaten rezervasyonunuzu iptal ettinizs");
            }



            DateTime time1 = DateTime.UtcNow.AddHours(3);
            var time2 = seans.SeansBaslangicZamani.AddHours(-24);

            if (time1 > time2)
                return BadRequest("Başlangıç saatine 24 saatten az kalan seansların rezervasyonları iptal edilemez.");

            rezervasyon.IptalEdildi = true;
            iptalEdilenBrans = seans.yapılanBrans;
            rezervasyon.IptalEdilenBrans = iptalEdilenBrans;

            seans.RezerveEdenKisiSayisi--;
            if (seans.Dolu == true)
                seans.Dolu = false;



            _context.SaveChanges();

            var aynıSeans = _context.Seanslar.FirstOrDefault(s => s.SeansId == SeansId);

            if (aynıSeans.RezerveEdenKisiSayisi == 0)
                seans.yapılanBrans = null;

            _context.SaveChanges();




            var rezervasyonBilgileri = _context.Rezervasyonlar.Where(r => r.KullanıcıId == KullanıcıId).Include(r => r.Seans).ToList();
            return Ok(rezervasyonBilgileri);





        }
        [HttpGet]
        [Authorize(Policy ="AdminOnly")]
        public IActionResult rezervasyonAramaAdmin([FromQuery] int kullaniciId)
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim))
                return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            // Token'daki ID'yi int'e dönüştür
            if (!int.TryParse(adminIdClaim, out var id))
                return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            var adminId = id;

            int tesisId = _context.Adminler
                .Where(a => a.Id == adminId)
                .Include(a => a.Tesis)
                .Select(a => a.TesisId)
                .FirstOrDefault();

            var kullanıcı=_context.Kullanıcılar.Find(kullaniciId);

            if(kullanıcı==null)
            {
                return BadRequest("Böyle bir kullanıcı yoktur");
            }

            //var rezervasyonlar = _context.Rezervasyonlar.Where(r=>r.KullanıcıId==kullaniciId);

            //var query = from rezervasyon in _context.Rezervasyonlar
            //            join kullanici in _context.Kullanıcılar
            //                on rezervasyon.KullanıcıId equals kullanici.KullanıcıId
            //            join seans in _context.Seanslar
            //                on rezervasyon.SeansId equals seans.SeansId
            //            where rezervasyon.KullanıcıId == kullaniciId //&& seans.SeansBaslangicZamani>DateTime.Now
            //            // Filtreleme
            //            select new
            //            {
            //                SeansId = rezervasyon.SeansId,
            //                KullanıcıAdı = kullanıcı.Name,
            //                SeansTarihi = seans.Tarih,
            //                SeansSaati = seans.SeansSaati,
            //                BransAdı = seans.yapılanBrans,
            //                SalonId = seans.SalonId
            //            };

            //var result =  query.ToListAsync();

            var rezervasyonlar = _context.Rezervasyonlar
                .Where(r => r.KullanıcıId == kullaniciId && r.Seans.Salon.TesisId==tesisId && r.IptalEdildi==false)
                .Include(r => r.Kullanıcı)
                .Include(r => r.Seans)
                .Select(r => new
                    {
                    SeansTarihi = r.Seans.Tarih,
                    SeansSaati = r.Seans.SeansSaati,
                    Brans=r.Seans.yapılanBrans,

                    SeansId = r.SeansId,
                    KullaniciId = r.Kullanıcı.KullanıcıId,
                    KullaniciAdi = r.Kullanıcı.Name,
                    TC = r.Kullanıcı.TC,
                    
                    
                    seansBaslangicZamani = r.Seans.SeansBaslangicZamani
                    })
                .Where(r=>r.seansBaslangicZamani>DateTime.Now)
    .ToList();

            return Ok(rezervasyonlar);
        }

        //public bool IptalEdilebilirMi(Seans seans)
        //{
        //    if (DateTime.UtcNow < seans.SeansBaslangicZamani.AddHours(-24))
        //        return true;
        //    else
        //        return false;
        //}

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public IActionResult aktifRezervasyonAramaKullanici()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            // Token'daki ID'yi int'e dönüştür
            if (!int.TryParse(userIdClaim, out var id))
                return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            var userId = id;

            

            var kullanıcı = _context.Kullanıcılar.Find(userId);

            if (kullanıcı == null)
            {
                return BadRequest("Böyle bir kullanıcı yoktur");
            }

            //var rezervasyonlar = _context.Rezervasyonlar.Where(r=>r.KullanıcıId==kullaniciId);

            //var query = from rezervasyon in _context.Rezervasyonlar
            //            join kullanici in _context.Kullanıcılar
            //                on rezervasyon.KullanıcıId equals kullanici.KullanıcıId
            //            join seans in _context.Seanslar
            //                on rezervasyon.SeansId equals seans.SeansId
            //            where rezervasyon.KullanıcıId == kullaniciId //&& seans.SeansBaslangicZamani>DateTime.Now
            //            // Filtreleme
            //            select new
            //            {
            //                SeansId = rezervasyon.SeansId,
            //                KullanıcıAdı = kullanıcı.Name,
            //                SeansTarihi = seans.Tarih,
            //                SeansSaati = seans.SeansSaati,
            //                BransAdı = seans.yapılanBrans,
            //                SalonId = seans.SalonId
            //            };

            //var result =  query.ToListAsync();

            var rezervasyonlar = _context.Rezervasyonlar
                .Where(r => r.KullanıcıId == userId && r.IptalEdildi==false)
                .Include(r => r.Kullanıcı)
                .Include(r => r.Seans)
                .Select(r => new
                {

                    SeansTarihi = r.Seans.Tarih,
                    SeansSaati = r.Seans.SeansSaati,
                    TesisAdı=r.Seans.Tesis.TesisAdı,
                    Brans = r.Seans.yapılanBrans,
                    SalonAdı=r.Seans.Salon.SalonaAdı,

                    SeansId = r.SeansId,
                    KullaniciId = r.Kullanıcı.KullanıcıId,
                    KullaniciAdi = r.Kullanıcı.Name,
                    TC = r.Kullanıcı.TC,


                    seansBaslangicZamani = r.Seans.SeansBaslangicZamani,
                    iptalEdildi = r.IptalEdildi

                })
                .Where(r => r.seansBaslangicZamani > DateTime.Now)
                .OrderBy(r => r.seansBaslangicZamani)
    .ToList();

            return Ok(rezervasyonlar);
        }

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public IActionResult iptalEdilenRezervasyonAramaKullanici()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            // Token'daki ID'yi int'e dönüştür
            if (!int.TryParse(userIdClaim, out var id))
                return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            var userId = id;



            var kullanıcı = _context.Kullanıcılar.Find(userId);

            if (kullanıcı == null)
            {
                return BadRequest("Böyle bir kullanıcı yoktur");
            }

            //var rezervasyonlar = _context.Rezervasyonlar.Where(r=>r.KullanıcıId==kullaniciId);

            //var query = from rezervasyon in _context.Rezervasyonlar
            //            join kullanici in _context.Kullanıcılar
            //                on rezervasyon.KullanıcıId equals kullanici.KullanıcıId
            //            join seans in _context.Seanslar
            //                on rezervasyon.SeansId equals seans.SeansId
            //            where rezervasyon.KullanıcıId == kullaniciId //&& seans.SeansBaslangicZamani>DateTime.Now
            //            // Filtreleme
            //            select new
            //            {
            //                SeansId = rezervasyon.SeansId,
            //                KullanıcıAdı = kullanıcı.Name,
            //                SeansTarihi = seans.Tarih,
            //                SeansSaati = seans.SeansSaati,
            //                BransAdı = seans.yapılanBrans,
            //                SalonId = seans.SalonId
            //            };

            //var result =  query.ToListAsync();

            var rezervasyonlar = _context.Rezervasyonlar
                .Where(r => r.KullanıcıId == userId && r.IptalEdildi == true)
                .Include(r => r.Kullanıcı)
                .Include(r => r.Seans)
                .Select(r => new
                {
                    SeansTarihi = r.Seans.Tarih,
                    SeansSaati = r.Seans.SeansSaati,
                    TesisAdı = r.Seans.Tesis.TesisAdı,
                    Brans = r.IptalEdilenBrans,
                    SalonAdı = r.Seans.Salon.SalonaAdı,

                    SeansId = r.SeansId,
                    KullaniciId = r.Kullanıcı.KullanıcıId,
                    KullaniciAdi = r.Kullanıcı.Name,
                    TC = r.Kullanıcı.TC,


                    seansBaslangicZamani = r.Seans.SeansBaslangicZamani,
                    iptalEdildi = r.IptalEdildi

                })

                .OrderByDescending(r => r.seansBaslangicZamani)
    .ToList();

            return Ok(rezervasyonlar);
        }

        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public IActionResult gecmisRezervasyonAramaKullanici()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Kimlik doğrulaması yapılırken hata oluştu.");

            // Token'daki ID'yi int'e dönüştür
            if (!int.TryParse(userIdClaim, out var id))
                return BadRequest("Token'daki kullanıcı ID'si geçerli bir sayı değil.");

            var userId = id;



            var kullanıcı = _context.Kullanıcılar.Find(userId);

            if (kullanıcı == null)
            {
                return BadRequest("Böyle bir kullanıcı yoktur");
            }

            //var rezervasyonlar = _context.Rezervasyonlar.Where(r=>r.KullanıcıId==kullaniciId);

            //var query = from rezervasyon in _context.Rezervasyonlar
            //            join kullanici in _context.Kullanıcılar
            //                on rezervasyon.KullanıcıId equals kullanici.KullanıcıId
            //            join seans in _context.Seanslar
            //                on rezervasyon.SeansId equals seans.SeansId
            //            where rezervasyon.KullanıcıId == kullaniciId //&& seans.SeansBaslangicZamani>DateTime.Now
            //            // Filtreleme
            //            select new
            //            {
            //                SeansId = rezervasyon.SeansId,
            //                KullanıcıAdı = kullanıcı.Name,
            //                SeansTarihi = seans.Tarih,
            //                SeansSaati = seans.SeansSaati,
            //                BransAdı = seans.yapılanBrans,
            //                SalonId = seans.SalonId
            //            };

            //var result =  query.ToListAsync();

            var rezervasyonlar = _context.Rezervasyonlar
                .Where(r => r.KullanıcıId == userId)
                .Include(r => r.Kullanıcı)
                .Include(r => r.Seans)
                .Select(r => new
                {
                    
                    SeansTarihi = r.Seans.Tarih,
                    SeansSaati = r.Seans.SeansSaati,
                    TesisAdı = r.Seans.Tesis.TesisAdı,
                    Brans = r.Seans.yapılanBrans,
                    SalonAdı = r.Seans.Salon.SalonaAdı,

                    SeansId = r.SeansId,
                    KullaniciId = r.Kullanıcı.KullanıcıId,
                    KullaniciAdi = r.Kullanıcı.Name,
                    TC = r.Kullanıcı.TC,


                    seansBaslangicZamani = r.Seans.SeansBaslangicZamani,
                    iptalEdildi = r.IptalEdildi

                })
                .Where(r => r.seansBaslangicZamani < DateTime.Now)
                .OrderByDescending(r => r.seansBaslangicZamani)
    .ToList();

            return Ok(rezervasyonlar);
        }


    }
}
