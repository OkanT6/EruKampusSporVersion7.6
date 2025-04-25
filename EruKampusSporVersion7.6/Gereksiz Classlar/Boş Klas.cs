//using EruKampusSpor.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Text.Json.Serialization;

//namespace EruKampusSpor.DTOs
//{
//    public class Boş_Klas
//    {
//        public class Adres
//        {
//            public int Id { get; set; }
//            public string AdresTanımı { get; set; }

//            // Foreign Key
//            public int KullanıcıId { get; set; }

//            // Navigation Property

//            //[JsonIgnore]
//            public Kullanıcı Kullanıcı { get; set; }
//        }
//        public class Brans
//        {

//            public Brans()
//            {
//                Salonlar = new HashSet<SalonBrans>();
//                Seanslar = new HashSet<Seans>();
//            }

//            public int BransId { get; set; }
//            public string BransAdı { get; set; }


//            // Many-to-Many Relationship
//            [JsonIgnore]
//            public ICollection<SalonBrans> Salonlar { get; set; }

//            [JsonIgnore]
//            public ICollection<Seans> Seanslar { get; set; } // Seanslarla ilişki
//        }

//        public class Kullanıcı
//        {
//            public Kullanıcı()
//            {
//                Seanslar = new HashSet<Seans>();
//            }

//            public int KullanıcıId { get; set; }
//            public string TC { get; set; }
//            public string Name { get; set; }


//            [JsonIgnore]
//            public Adres Adres { get; set; }

//            [JsonIgnore]
//            public ICollection<Seans> Seanslar { get; set; } // Seanslarla ilişki
//        }

//        public class Salon
//        {

//            public Salon()
//            {
//                Branslar = new HashSet<SalonBrans>();
//                Seanslar = new HashSet<Seans>();
//            }
//            public int SalonId { get; set; } //Primary Key
//            public string SalonaAdı { get; set; }

//            [JsonIgnore]

//            public Tesis Tesis { get; set; }

//            public int TesisId { get; set; }//Foreign Key


//            // Many-to-Many Relationship
//            [JsonIgnore]
//            public ICollection<SalonBrans> Branslar { get; set; }
//            [JsonIgnore]
//            public ICollection<Seans> Seanslar { get; set; } // Seanslarla ilişki


//        }


//        public class SalonBrans
//        {
//            // Composite Primary Key (SalonId + BransId)
//            public int SalonId { get; set; }
//            public int BransId { get; set; }

//            // Navigation Properties
//            [JsonIgnore]
//            public Salon Salon { get; set; }
//            [JsonIgnore]
//            public Brans Brans { get; set; }
//        }



//        public class Seans
//        {
//            public int TesisId { get; set; } // Foreign Key
//            public int SalonId { get; set; } // Foreign Key
//            public int BransId { get; set; } // Foreign Key
//            public TimeSpan SeansSaati { get; set; } // Saati tutar
//            public DateTime Tarih { get; set; } // Tarihi tutar
//            public bool SeansRezerveEdildiMi { get; set; } // Durum bilgisi
//            public int? KullanıcıId { get; set; } // Nullable Foreign Key

//            // Navigation Properties
//            public Tesis Tesis { get; set; }
//            public Salon Salon { get; set; }
//            public Brans Brans { get; set; }
//            public Kullanıcı? Kullanıcı { get; set; }
//        }


//        public class Tesis
//        {
//            public Tesis()
//            {
//                Salonlar = new HashSet<Salon>();
//                Seanslar = new HashSet<Seans>();
//            }

//            public int TesisId { get; set; } //Primary Key
//            public string TesisAdı { get; set; }
//            [JsonIgnore]
//            public ICollection<Salon> Salonlar { get; set; }

//            [JsonIgnore]
//            public ICollection<Seans> Seanslar { get; set; } // Seanslarla ilişki

//        }

//        public class ApplicationDbContext : DbContext
//        {

//            public static List<EruUser> bilgiIslem = new List<EruUser>
//        {
//            new EruUser{ EruUserId = 1, TC = "11111111111",name="Okan"},
//            new EruUser{ EruUserId = 2, TC = "22222222222",name="Berkay"},
//            new EruUser{ EruUserId = 3, TC = "33333333333",name="Eren"},
//            new EruUser{ EruUserId = 4, TC = "44444444444",name="Furkan"},
//            new EruUser{ EruUserId = 5, TC = "55555555555",name="Melisa"},
//            new EruUser{ EruUserId = 6, TC = "66666666666",name="Mehmet"},
//            new EruUser{ EruUserId = 7, TC = "77777777777",name="Mürşide"},
//            new EruUser{ EruUserId = 8, TC = "88888888888",name="Ahmet"},
//            new EruUser{ EruUserId = 9, TC = "99999999999",name="Berke"},
//            new EruUser{ EruUserId = 9, TC = "99999999900",name="Fehim"},
//            new EruUser{ EruUserId = 9, TC = "99999999901",name="Mete"},
//            new EruUser{ EruUserId = 9, TC = "99999999902",name="Beyza"},
//            new EruUser{ EruUserId = 9, TC = "99999999903",name="Fatma Özge"},
//            new EruUser{ EruUserId = 9, TC = "99999999904",name="Osman"},
//            new EruUser{ EruUserId = 9, TC = "99999999905",name="Bahriye"},
//            new EruUser{ EruUserId = 9, TC = "99999999906",name="Ömür Şahin"},
//            new EruUser{ EruUserId = 9, TC = "99999999907",name="Bilal"},
//            new EruUser{ EruUserId = 9, TC = "99999999908",name="Serkan"},
//            new EruUser{ EruUserId = 9, TC = "99999999909",name="Tayyip"}


//        };


//            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//            : base(options)
//            {
//            }

//            public DbSet<Kullanıcı> Kullanıcılar { get; set; }
//            public DbSet<Adres> Adresler { get; set; }

//            public DbSet<Tesis> Tesisler { get; set; }
//            public DbSet<Brans> Branslar { get; set; }

//            public DbSet<Salon> Salonlar { get; set; }

//            public DbSet<SalonBrans> SalonBrans { get; set; }

//            public DbSet<Seans> Seanslar { get; set; }


//            protected override void OnModelCreating(ModelBuilder modelBuilder)
//            {
//                base.OnModelCreating(modelBuilder);

//                modelBuilder.Entity<Kullanıcı>()
//            .HasKey(k => k.KullanıcıId);

//                modelBuilder.Entity<Adres>()
//                    .HasKey(a => a.Id);

//                modelBuilder.Entity<Kullanıcı>()
//                    .HasOne(a => a.Adres)
//                    .WithOne(k => k.Kullanıcı)
//                    .HasForeignKey<Adres>(a => a.KullanıcıId);



//                modelBuilder.Entity<Tesis>()
//                    .HasKey(t => t.TesisId);

//                modelBuilder.Entity<Brans>()
//                    .HasKey(b => b.BransId);

//                modelBuilder.Entity<Salon>()
//                    .HasKey(s => s.SalonId);

//                // Tesis ve Salon arasında 1'e çok ilişki
//                modelBuilder.Entity<Salon>()
//                    .HasOne(s => s.Tesis)       // Salon, bir Tesis'e bağlı
//                    .WithMany(t => t.Salonlar)  // Tesis, birden fazla Salon'a sahip
//                    .HasForeignKey(s => s.TesisId)  // Salon'daki TesisId, dış anahtar
//                    .OnDelete(DeleteBehavior.Cascade); // Tesis silindiğinde, ilgili salonlar da silinsin


//                //many-to-many between salon and brans

//                // SalonBrans Ara Tablo Konfigürasyonu
//                modelBuilder.Entity<SalonBrans>()
//                    .HasKey(sb => new { sb.SalonId, sb.BransId }); // Composite Primary Key

//                modelBuilder.Entity<SalonBrans>()
//                    .HasOne(sb => sb.Salon) // Ara tablo ile Salon arasında ilişki 1-n
//                    .WithMany(s => s.Branslar)
//                    .HasForeignKey(sb => sb.SalonId);

//                modelBuilder.Entity<SalonBrans>()
//                    .HasOne(sb => sb.Brans) // Ara tablo ile Brans arasında ilişki 1-n
//                    .WithMany(b => b.Salonlar)
//                    .HasForeignKey(sb => sb.BransId);

//                // Seans için Composite Key tanımlama
//                modelBuilder.Entity<Seans>()
//                    .HasKey(s => new { s.TesisId, s.SalonId, s.BransId, s.SeansSaati, s.Tarih });

//                // Tesis ve Seans İlişkisi
//                modelBuilder.Entity<Seans>()
//                    .HasOne(s => s.Tesis)
//                    .WithMany(t => t.Seanslar)
//                    .HasForeignKey(s => s.TesisId)
//                    .OnDelete(DeleteBehavior.Restrict);

//                // Salon ve Seans İlişkisi
//                modelBuilder.Entity<Seans>()
//                    .HasOne(s => s.Salon)
//                    .WithMany(salon => salon.Seanslar)
//                    .HasForeignKey(s => s.SalonId)
//                    .OnDelete(DeleteBehavior.Restrict);

//                // Branş ve Seans İlişkisi
//                modelBuilder.Entity<Seans>()
//                    .HasOne(s => s.Brans)
//                    .WithMany(b => b.Seanslar)
//                    .HasForeignKey(s => s.BransId)
//                    .OnDelete(DeleteBehavior.Restrict);




//                // Kullanıcı ve Seans İlişkisi (Nullable Foreign Key)
//                modelBuilder.Entity<Seans>()
//                    .HasOne(s => s.Kullanıcı)
//                    .WithMany(k => k.Seanslar)
//                    .HasForeignKey(s => s.KullanıcıId)
//                    .OnDelete(DeleteBehavior.SetNull);
//                //.IsRequired(false); bu olmalı mı ?




//                // SeansSaati ve Tarih alanları için tür atamaları
//                modelBuilder.Entity<Seans>()
//                    .Property(s => s.SeansSaati)
//                    .HasColumnType("time"); // Saat bilgisi için uygun bir tür

//                modelBuilder.Entity<Seans>()
//                    .Property(s => s.Tarih)
//                    .HasColumnType("date"); // Tarih bilgisi için uygun bir tür

//                // SeansDurumu alanı için varsayılan değer
//                modelBuilder.Entity<Seans>()
//                    .Property(s => s.SeansRezerveEdildiMi)
//                    .HasDefaultValue(false);

//                modelBuilder.Entity<Seans>()
//                    .Property(s => s.KullanıcıId)
//                    .HasDefaultValue(null); //Oluşturulan seanslar null olarak müsait bir sekilde oluşturul.
//                                            //Seans bir kullanıcı tarafından rezerve edildiği taktirdi o kullanıcının Id değeri
//                                            //KullanıcıId foreignKey kolonuna karşılık gelecektir. 


//            }

//        }

//        public class EruUser
//        {

//            public int EruUserId { get; set; }
//            public string name { get; set; }
//            public string TC { get; set; }
//        }
//    }




//}
