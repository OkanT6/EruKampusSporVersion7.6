using EruKampusSpor.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace EruKampusSpor.Data
{
    public class ApplicationDbContext:DbContext
    {

        


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Kullanıcı> Kullanıcılar { get; set; }
        public DbSet<KullanıcıDetay> KullanıcıDetayları { get; set; }

       public DbSet<Rezervasyon> Rezervasyonlar {  get; set; }
        public DbSet<Tesis> Tesisler { get; set; }
        public DbSet<Brans> Branslar { get; set; }

        public DbSet<Salon> Salonlar { get; set; }

        public DbSet<SalonBrans> SalonBrans { get; set; }

        public DbSet<Seans> Seanslar { get; set; }
        public DbSet<Admin> Adminler { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.Tesis)
                .WithOne(t => t.Admin)
                .HasForeignKey<Admin>(a => a.TesisId);


            modelBuilder.Entity<Kullanıcı>()
        .HasKey(k => k.KullanıcıId);

            modelBuilder.Entity<KullanıcıDetay>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Kullanıcı>()
                .HasOne(a => a.KullanıcıDetay)
                .WithOne(k => k.Kullanıcı)
                .HasForeignKey<KullanıcıDetay>(a => a.Id);

            modelBuilder.Entity<KullanıcıDetay>()
                .HasKey(kd => kd.Id);



            modelBuilder.Entity<Tesis>()
                .HasKey(t => t.TesisId);

            modelBuilder.Entity<Brans>()
                .HasKey(b => b.BransId);

            modelBuilder.Entity<Salon>()
                .HasKey(s => s.SalonId);

            // Tesis ve Salon arasında 1'e çok ilişki
            modelBuilder.Entity<Salon>()
                .HasOne(s => s.Tesis)       // Salon, bir Tesis'e bağlı
                .WithMany(t => t.Salonlar)  // Tesis, birden fazla Salon'a sahip
                .HasForeignKey(s => s.TesisId)  // Salon'daki TesisId, dış anahtar
                .OnDelete(DeleteBehavior.Cascade); // Tesis silindiğinde, ilgili salonlar da silinsin


            //many-to-many between salon and brans

            // SalonBrans Ara Tablo Konfigürasyonu
            modelBuilder.Entity<SalonBrans>()
                .HasKey(sb => new { sb.SalonId, sb.BransId }); // Composite Primary Key

            modelBuilder.Entity<SalonBrans>()
                .HasOne(sb => sb.Salon) // Ara tablo ile Salon arasında ilişki 1-n
                .WithMany(s => s.Branslar)
                .HasForeignKey(sb => sb.SalonId);

            modelBuilder.Entity<SalonBrans>()
                .HasOne(sb => sb.Brans) // Ara tablo ile Brans arasında ilişki 1-n
                .WithMany(b => b.Salonlar)
                .HasForeignKey(sb => sb.BransId);

            // Seans için Composite Key tanımlama
            modelBuilder.Entity<Seans>()
                .HasKey(s => new { s.TesisId, s.SalonId, s.SeansSaati, s.Tarih });

            // Tesis ve Seans İlişkisi
            modelBuilder.Entity<Seans>()
                .HasOne(s => s.Tesis)
                .WithMany(t => t.Seanslar)
                .HasForeignKey(s => s.TesisId)
                .OnDelete(DeleteBehavior.Restrict);

            // Salon ve Seans İlişkisi
            modelBuilder.Entity<Seans>()
                .HasOne(s => s.Salon)
                .WithMany(salon => salon.Seanslar)
                .HasForeignKey(s => s.SalonId)
                .OnDelete(DeleteBehavior.Restrict);

            //// Branş ve Seans İlişkisi
            //modelBuilder.Entity<Seans>()
            //    .HasOne(s => s.Brans)
            //    .WithMany(b => b.Seanslar)
            //    .HasForeignKey(s => s.BransId)
            //    .OnDelete(DeleteBehavior.Restrict);




            //// Kullanıcı ve Seans İlişkisi (Nullable Foreign Key)
            //modelBuilder.Entity<Seans>()
            //    .HasOne(s => s.Kullanıcı)
            //    .WithMany(k => k.Seanslar)
            //    .HasForeignKey(s => s.KullanıcıId)
            //    .OnDelete(DeleteBehavior.SetNull);
            //    //.IsRequired(false); bu olmalı mı ?




            // SeansSaati ve Tarih alanları için tür atamaları
            modelBuilder.Entity<Seans>()
                .Property(s => s.SeansSaati)
                .HasColumnType("time"); // Saat bilgisi için uygun bir tür

            modelBuilder.Entity<Seans>()
                .Property(s => s.Tarih)
                .HasColumnType("date"); // Tarih bilgisi için uygun bir tür

            // SeansDurumu alanı için varsayılan değer
            //modelBuilder.Entity<Seans>()
            //    .Property(s => s.SeansRezerveEdildiMi)
            //    .HasDefaultValue(false);

            

            modelBuilder.Entity<Seans>()
                .HasAlternateKey(s => s.SeansId);
            modelBuilder.Entity<Seans>()
                .HasIndex(s => s.SeansId);

            modelBuilder.Entity<Seans>()
                .Property(s => s.SeansId)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<Seans>()
                .Property(s => s.Kontenjan)
                .HasDefaultValue(1);

            modelBuilder.Entity<Seans>()
                .Property(s => s.Dolu)
                .HasDefaultValue(false);

    //        modelBuilder.Entity<Seans>()
    //.HasCheckConstraint("CHK_RezerveEdenKisi_Kontenjan", "[RezerveEdenKisiSayisi] <= [Kontenjan]");


            //modelBuilder.Entity<Seans>()
            //    .Property(s => s.RezerveEdenKisiSayısı)
            //    .HasDefaultValue(0); 

            //        modelBuilder.Entity<Seans>()
            //            .Property(s => s.RezerveEdenKisiSayısı)
            //            .HasComputedColumnSql(@"
            //(SELECT COUNT(*)
            // FROM Rezervasyon r
            // WHERE r.SeansID = [Seans.SeansID]"

            modelBuilder.Entity<Rezervasyon>()
                .Property(r=>r.RezerveEdilmeTarihi)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Seans>()
                .Property(s => s.SeansBaslangicZamani)
                .HasComputedColumnSql("CAST([Tarih] AS DATETIME) + CAST([SeansSaati] AS DATETIME)");





            //          .HasComputedColumnSql(@"
            //(SELECT COUNT(*)
            // FROM Rezervasyon r
            // INNER JOIN Seans s ON r.SeansId = s.SeansId  
            // WHERE r.SeansId  = Seans.SeansId  
            // GROUP BY r.SeansId)");
            //GROUP BY r.SeansID

            modelBuilder.Entity<Rezervasyon>()
                .HasKey(r => new { r.SeansId, r.KullanıcıId });

            modelBuilder.Entity<Rezervasyon>()
                .HasOne(r=>r.Kullanıcı)
                .WithMany(k=>k.Rezervasyonlar)
                .HasForeignKey(r=>r.KullanıcıId)
            .OnDelete(DeleteBehavior.Cascade); // Restrict ile silmeyi engelleme

            modelBuilder.Entity<Rezervasyon>()
                .HasOne(r => r.Seans)
                .WithMany(s => s.Rezervasyonlar)
                .HasForeignKey(r => r.SeansId)
              .HasPrincipalKey(s => s.SeansId)// Alternate key'i kullanıyoruz
            .OnDelete(DeleteBehavior.Cascade); // Restrict ile silmeyi engelleme



            //        modelBuilder.Entity<Seans>()
            //.Property(s => s.RezerveEdenKisiSayisi)
            //.HasComputedColumnSql(@"
            //    (SELECT COUNT(*)
            //     FROM Rezervasyon r
            //     WHERE r.SeansId = Seans.SeansId)
            //");

    //        modelBuilder.Entity<Seans>()
    //.Property(s => s.RezerveEdenKisiSayisi)
    //.HasComputedColumnSql(@"
    //    (SELECT COUNT(*) 
    //     FROM Rezervasyonlar r
    //     WHERE r.SeansId = Seans.SeansId)
    //");








            //modelBuilder.Entity<Seans>()
            //    .Property(s => s.KullanıcıId)
            //    .HasDefaultValue(null); //Oluşturulan seanslar null olarak müsait bir sekilde oluşturul.
            //Seans bir kullanıcı tarafından rezerve edildiği taktirdi o kullanıcının Id değeri
            //KullanıcıId foreignKey kolonuna karşılık gelecektir. 


        }


        public static List<EruUser> bilgiIslem = new List<EruUser>
        {
            new EruUser{ EruUserId = 1, TC = "11111111111",name="Okan Topdemir", obisis_password="Okan11", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 2, TC = "22222222222",name="Berkay Biçer", obisis_password="Berkay22", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 3, TC = "33333333333",name="Eren Gençer",obisis_password="Eren33", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 4, TC = "44444444444",name="Furkan İnal", obisis_password = "Furkan44", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 5, TC = "55555555555",name="Tuğba Melisa Güngör", obisis_password = "Melisa55",Cinsiyet=Cinsiyet.Kadın},
            new EruUser{ EruUserId = 6, TC = "66666666666",name="Mehmet Kurnaz", obisis_password = "Mehmet66", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 7, TC = "77777777777",name="Mürşide Öden", obisis_password = "Mürşide77",Cinsiyet=Cinsiyet.Kadın},
            new EruUser{ EruUserId = 8, TC = "88888888888",name="Ahmet Emin Yılmaz", obisis_password = "Ahmet88", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 9, TC = "99999999999",name="Berke Balcı", obisis_password = "Berke99", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 10, TC = "99999999900",name="Fehim Köylü", obisis_password = "Fehim00", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 11, TC = "99999999901",name="Mete Çelik", obisis_password = "Mete01", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 12, TC = "99999999902",name="Beyza Görkemli", obisis_password = "Beyza02",Cinsiyet=Cinsiyet.Kadın},
            new EruUser{ EruUserId = 13, TC = "99999999903",name="Fatma Özge", obisis_password = "FatmaÖzge03",Cinsiyet=Cinsiyet.Kadın},
            new EruUser{ EruUserId = 14, TC = "99999999904",name="Osman", obisis_password = "Osman04", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 15, TC = "99999999905",name="Bahriye", obisis_password = "Bahriye05",Cinsiyet=Cinsiyet.Kadın},
            new EruUser{ EruUserId = 16, TC = "99999999906",name="Alper", obisis_password = "Alper06", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 17, TC = "99999999907",name="Bilal", obisis_password = "Bilal07", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 18, TC = "99999999908",name="Serkan", obisis_password = "Serkan08", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 19, TC = "99999999909",name="Tayyip", obisis_password = "Tayyip09", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 20, TC = "99999999910",name="Damla", obisis_password = "Damla10",Cinsiyet=Cinsiyet.Kadın},
            new EruUser{ EruUserId = 21, TC = "12345678901",name="Berra", obisis_password = "Berra1234" ,Cinsiyet=Cinsiyet.Kadın},
            new EruUser{ EruUserId = 22, TC = "99999999911",name="Esra Sayın Hazretleri", obisis_password = "Esra1234",Cinsiyet=Cinsiyet.Kadın},
            new EruUser{ EruUserId = 23, TC = "99999999000",name="Ahmet Nusret Toprak", obisis_password = "Ahmet000", Cinsiyet=Cinsiyet.Erkek},
            new EruUser{ EruUserId = 24, TC = "99999999001",name="Alperen Kuzu", obisis_password = "Alperen001", Cinsiyet=Cinsiyet.Erkek},
           new EruUser{ EruUserId = 24, TC = "99999999002",name="Alpe", obisis_password = "Alperen001", Cinsiyet=Cinsiyet.Erkek},
           new EruUser{ EruUserId = 25, TC = "99999999003",name="Teoman", obisis_password = "Teoman003", Cinsiyet=Cinsiyet.Erkek}





        };
    }

    
}
