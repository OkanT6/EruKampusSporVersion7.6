using EruKampusSpor.Data;
using EruKampusSpor.DTOs;
using EruKampusSpor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EruKampusSpor.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SeansController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/seans
        // GET: api/seans
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public ActionResult<IEnumerable<SeansDTO>> GetSeanslar()
        {
            var seanslar = _context.Seanslar
                .Include(s => s.Tesis)
                .Include(s => s.Salon)
                
                .ToList();



            return Ok(seanslar);
        }
        
        

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult seansEkle(SeansDTO Yeniseans)
        {
            if (Yeniseans == null)
                return BadRequest();

            var tesis = _context.Tesisler.Find(Yeniseans.TesisId);
            if (tesis == null)
                return BadRequest("Yanlış tesisId");

            var salon = _context.Salonlar.Find(Yeniseans.SalonId);

            if (tesis == null)
                return BadRequest("Yanlış salonId");

            //if ((DateTime)Yeniseans.Tarih == null)
            //    return BadRequest("Tarih girilmesi zorunludur");

            //if ((TimeSpan)Yeniseans.SeansSaati == null)
            //    return BadRequest("Seans saati girilmesi zorunludur");

            
            if ((int)Yeniseans.SeansCinsiyet != 0 && ((int)Yeniseans.SeansCinsiyet) != 1 && (int)Yeniseans.SeansCinsiyet != 2)
                return BadRequest("Geçersiz sean cinsiyet değeri");

            Yeniseans.SeansCinsiyet = (SeansCinsiyet)Yeniseans.SeansCinsiyet;
            if (Yeniseans.Kontenjan <= 0)
            {
                return BadRequest("Kontenjan değeri negatif veya sıfır olamaz");
            }

            Seans seans = new Seans
            {
                TesisId = Yeniseans.TesisId,
                SalonId = Yeniseans.SalonId,

                Tarih = (DateTime)Yeniseans.Tarih,
                SeansSaati = (TimeSpan)Yeniseans.SeansSaati,
                Kontenjan = Yeniseans.Kontenjan,
                SeansCinsiyet = (SeansCinsiyet)Yeniseans.SeansCinsiyet,
                yapılanBrans = null,
                SeansBaslangicZamani = (DateTime)Yeniseans.Tarih + (TimeSpan)Yeniseans.SeansSaati

            };

            if ((seans.SeansBaslangicZamani < DateTime.Now))
                return BadRequest("Seans başlangıç zamanı geçmiş tarih olamaz");

            // Geçerli mi kontrolü(seans zaten var mı ?)
            if (SeansVarlıkKontrol(seans))
            {
                return BadRequest("Bu seans zaten mevcut.");
            }

            //        var result = _context.SalonBrans
            //.GroupBy(x => x.SalonId)              // SalonId'ye göre grupla
            //.Select(g => new
            //{
            //    SalonId = g.Key,                  // Gruplama anahtarı (SalonId)
            //    BransCount = g.Count()            // Grup içerisindeki eleman sayısı (BransCount)
            //})
            //.Where(x => x.BransCount == 1)        // BransCount değeri 1 olanları filtrele
            //.Select(x => x.SalonId)               // Sadece SalonId'yi seç
            //.ToList();                            // Liste haline getir

            //        var salondaTekBransYapılanSalonlar= _context.SalonBrans.Where(x => x.SalonId =={ })


            var salonIdList = _context.SalonBrans
    .GroupBy(x => x.SalonId)
    .Select(g => new
    {
        SalonId = g.Key,
        BransCount = g.Count()
    })
    .Where(x => x.BransCount == 1)
    .Select(x => x.SalonId)
    .ToList(); // Bu listeyi alıyoruz

            var salondaTekBransYapılanSalonlar = _context.SalonBrans
                .Where(x => salonIdList.Contains(x.SalonId)) // salonId'si listede olanları alıyoruz
                .ToList();

            var joinedResults = salondaTekBransYapılanSalonlar
                    .Join(_context.Branslar,
                          s => s.BransId,
                          b => b.BransId,
                          (s, b) => new
                          {
                              SalonId = s.SalonId,
                              BransId = b.BransId,
                              BransName = b.BransAdı,  // Branslar tablosundaki ad alanı
                          });

            seans.yapılanBrans = joinedResults
    .Where(t => t.SalonId == Yeniseans.SalonId)
    .Select(t => t.BransName)
    .FirstOrDefault();  // Bu, sadece ilk branşı alır












            _context.Seanslar.Add(seans);
            _context.SaveChanges();

            // var seansBilgi = _context.Seanslar.Include(s=>s.Salon).ThenInclude(s=>s.Branslar).FirstOrDefault(s => s.SeansId == seans.SeansId);

            return Ok(new
            {
                Success = true,
                Message = "Seans başarıyla eklendi!",
                
            });



        }

        [HttpDelete]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult SeansSil([FromBody] int SeansId)
        {
            try
            {
                // Kullanıcıyı id ile bul
                var seans = _context.Seanslar.FirstOrDefault(s => s.SeansId == SeansId);

                // Eğer kullanıcı yoksa, 404 döndür
                if (seans == null)
                {
                    return NotFound($"Id'si {SeansId} olan kullanıcı bulunamadı.");
                }

                // Kullanıcıyı veritabanından sil
                _context.Seanslar.Remove(seans);
                _context.SaveChanges();

                // Başarılı durum kodu ile mesaj döndür
                return Ok($"Id'si {SeansId} olan kullanıcı başarıyla silindi.");
            }
            catch (Exception ex)
            {
                // Hata durumunda 500 döndür
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }


        [HttpGet]
        [Authorize(Policy = "AdminOrUser")]

        public IActionResult seansAramaByBransIdveTesisIdveSalonId([FromQuery] int bransId, [FromQuery] int tesisId, [FromQuery] int salonId, [FromQuery] DateTime tarih)

        {

            var arananSalonunBransSayısı = _context.SalonBrans.Where(sb => sb.SalonId == salonId).Count();
            DateTime date1 = DateTime.Now;


            if(arananSalonunBransSayısı==1)
            {
                var seanslar = _context.Seanslar.Where(s => s.TesisId == tesisId && s.SalonId == salonId && s.SeansBaslangicZamani > date1 && s.Tarih==tarih).ToList();
                if (seanslar == null)
                    return NotFound();
                return Ok(seanslar);
            }
            else if(arananSalonunBransSayısı>1)
            {
                var arananBrans = _context.Branslar.Find(bransId);
                if (arananBrans == null)
                    return NotFound("Böyle bir brans yoktur");


                var Seanslar2 = _context.Seanslar.Where(s => s.TesisId == tesisId && s.SalonId == salonId && s.SeansBaslangicZamani > date1 && s.Tarih == tarih).ToList();


                //string arananBransAdı = arananBrans.BransAdı;
                //foreach (Seans seans in Seanslar2)
                //{
                //    List<Seans> gonderilecekSeanslarDoluSeanslar = new List<Seans>();
                //    if(arananBransAdı!=seans.yapılanBrans)
                //    {
                //        SeansDTO seansDTO = new SeansDTO
                //        {
                //            TesisId = seans.SeansId,
                //            SalonId = seans.SalonId,
                //            SeansSaati = seans.SeansSaati,
                //            Tarih = seans.Tarih,
                //            Kontenjan = seans.Kontenjan,
                //            SeansCinsiyet = seans.SeansCinsiyet,
                //            Dolu = false
                //        };
                //        gonderilecekSeanslarDoluSeanslar.Add(seans);
                //    }
                //}

                string arananBransAdı = arananBrans.BransAdı;
                List<SeansDTO> gonderilecekSeanslar = new List<SeansDTO>();

                foreach(Seans seans in Seanslar2 )
                {
                    gonderilecekSeanslar.Add(new SeansDTO
                    {
                        TesisId = seans.TesisId,
                        SalonId = seans.SalonId,
                        SeansSaati = seans.SeansSaati,
                        Tarih = seans.Tarih,
                        SeansId = seans.SeansId,
                        Kontenjan = seans.Kontenjan,
                        yapılanBrans = seans.yapılanBrans,
                        SeansBaslangicZamani = seans.SeansBaslangicZamani,
                        Dolu = seans.Dolu,
                        SeansCinsiyet = seans.SeansCinsiyet,
                        RezerveEdenKisiSayisi = seans.RezerveEdenKisiSayisi

                    });
                }

                foreach(SeansDTO seansDTO in gonderilecekSeanslar)
                {
                    if(seansDTO.yapılanBrans!=arananBransAdı && seansDTO.yapılanBrans!=null)
                    {
                        seansDTO.Dolu = true;
                    }
                }

                
                //foreach (Seans seans in Seanslar2)
                //{
                //    if(seans.yapılanBrans== arananBransAdı && seans.RezerveEdenKisiSayisi==0)
                //    {
                //        gonderilecekSeanslar.Add(new SeansDTO {
                //            TesisId=seans.TesisId,
                //            SalonId=seans.SalonId,
                //            SeansSaati=seans.SeansSaati,
                //            Tarih=seans.Tarih,
                //            SeansId=seans.SeansId,
                //            Kontenjan=seans.Kontenjan,
                //            yapılanBrans=seans.yapılanBrans,
                //            SeansBaslangicZamani=seans.SeansBaslangicZamani,
                //            Dolu=seans.Dolu,
                //            SeansCinsiyet=seans.SeansCinsiyet,
                //            RezerveEdenKisiSayisi=seans.RezerveEdenKisiSayisi             
                //        });
                //    }
                //    else if (seans.yapılanBrans == arananBransAdı)
                //    {
                //        gonderilecekSeanslar.Add(new SeansDTO
                //        {
                //            TesisId = seans.TesisId,
                //            SalonId = seans.SalonId,
                //            SeansSaati = seans.SeansSaati,
                //            Tarih = seans.Tarih,
                //            SeansId = seans.SeansId,
                //            Kontenjan = seans.Kontenjan,
                //            yapılanBrans = seans.yapılanBrans,
                //            SeansBaslangicZamani = seans.SeansBaslangicZamani,
                //            Dolu = seans.Dolu,
                //            SeansCinsiyet = seans.SeansCinsiyet,
                //            RezerveEdenKisiSayisi = seans.RezerveEdenKisiSayisi
                //        });
                //    }
                    
                //    else 
                //    {
                //        gonderilecekSeanslar.Add(new SeansDTO {
                //            TesisId = seans.TesisId,
                //            SalonId = seans.SalonId,
                //            SeansSaati = seans.SeansSaati,
                //            Tarih = seans.Tarih,
                //            SeansId = seans.SeansId,
                //            Kontenjan = seans.Kontenjan,
                //            yapılanBrans = arananBransAdı,
                //            SeansBaslangicZamani = seans.SeansBaslangicZamani,
                //            Dolu = true,
                //            SeansCinsiyet = seans.SeansCinsiyet,
                //            RezerveEdenKisiSayisi = seans.RezerveEdenKisiSayisi
                //        });
                //    }
                
                    



                    //List<Seans> ArananTümSeanslar=new List<Seans>();

                    //foreach (var seans in seanslar)
                    //{
                    //    ArananTümSeanslar.Add(seans);
                    //}

                    //foreach (var seans in Seanslar2)
                    //{
                    //    ArananTümSeanslar.Add(seans);
                    //}



                    if (gonderilecekSeanslar == null)
                    return NotFound();
                return Ok(gonderilecekSeanslar);



            }
            else
                return BadRequest("Seçilen salon için branş ataması yapılmamıştır");




            //foreach (var s in seanslar)
            //{
            //    Console.WriteLine(s.SeansBaslangicZamani);
            //}



        }






        //// POST: api/seans
        //[HttpPost]
        //public ActionResult<SeansDTO> PostSeans(SeansDTO seansDto)
        //{
        //    // DTO'dan gelen veriyi Seans modeline dönüştür
        //    var seans = new Seans
        //    {
        //        TesisId = seansDto.TesisId,
        //        SalonId = seansDto.SalonId,
        //        BransId = seansDto.BransId,
        //        SeansSaati = seansDto.SeansSaati,
        //        Tarih = seansDto.Tarih,


        //    };

        //    // Geçerli mi kontrolü (seans zaten var mı?)
        //    if (SeansVarlıkKontrol(seans))
        //    {
        //        return BadRequest("Bu seans zaten mevcut.");
        //    }

        //    // Seans ekleme
        //    _context.Seanslar.Add(seans);
        //    _context.SaveChanges();

        //    // Yeni eklenen seansı DTO olarak dön
        //    var createdSeansDto = new SeansDTO
        //    {
        //        TesisId = seans.TesisId,
        //        SalonId = seans.SalonId,
        //        BransId = seans.BransId,
        //        SeansSaati = seans.SeansSaati,
        //        Tarih = seans.Tarih,

        //    };

        //    return CreatedAtAction(nameof(GetSeanslar),
        //        new { tesisId = seans.TesisId, salonId = seans.SalonId, bransId = seans.BransId, seansSaati = seans.SeansSaati, tarih = seans.Tarih },
        //        createdSeansDto);
        //}


        ////// PUT: api/seans/choose/{tesisId}/{salonId}/{bransId}/{seansSaati}/{tarih}
        ////[HttpPut("choose/{tesisId}/{salonId}/{bransId}/{seansSaati}/{tarih}")]
        ////public IActionResult PutSeansSec(int tesisId, int salonId, int bransId, TimeSpan seansSaati, DateTime tarih, [FromBody] int kullanıcıId)
        ////{
        ////    var seans = _context.Seanslar
        ////        .FirstOrDefault(s => s.TesisId == tesisId && s.SalonId == salonId && s.BransId == bransId && s.SeansSaati == seansSaati && s.Tarih == tarih);

        ////    if (seans == null)
        ////    {
        ////        return NotFound("Seans bulunamadı.");
        ////    }

        ////    // Seans zaten bir kullanıcı tarafından seçilmişse başka bir kullanıcı bu seans'ı alamaz




        ////    // Seansı rezerve et


        ////    _context.Entry(seans).State = EntityState.Modified;
        ////    _context.SaveChanges();

        ////    // Güncellenmiş seansı DTO olarak döndür
        ////    var seansDto = new SeansDTO
        ////    {
        ////        TesisId = seans.TesisId,
        ////        SalonId = seans.SalonId,
        ////        BransId = seans.BransId,
        ////        SeansSaati = seans.SeansSaati,
        ////        Tarih = seans.Tarih,

        ////    };

        ////    return Ok(seansDto);
        ////}

        //// Seans var mı kontrolü (composite key kontrolü)
        private bool SeansVarlıkKontrol(Seans seans)
        {
            return _context.Seanslar.Any(s =>
                s.TesisId == seans.TesisId &&
                s.SalonId == seans.SalonId &&
                
                s.SeansSaati == seans.SeansSaati &&
                s.Tarih == seans.Tarih);
        }
    }
}
