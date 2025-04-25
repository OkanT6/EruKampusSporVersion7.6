using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EruKampusSpor.Migrations
{
    /// <inheritdoc />
    public partial class mig_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branslar",
                columns: table => new
                {
                    BransId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BransAdı = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branslar", x => x.BransId);
                });

            migrationBuilder.CreateTable(
                name: "Kullanıcılar",
                columns: table => new
                {
                    KullanıcıId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanıcılar", x => x.KullanıcıId);
                });

            migrationBuilder.CreateTable(
                name: "Tesisler",
                columns: table => new
                {
                    TesisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TesisAdı = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tesisler", x => x.TesisId);
                });

            migrationBuilder.CreateTable(
                name: "KullanıcıDetayları",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EMail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cinsiyet = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullanıcıDetayları", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullanıcıDetayları_Kullanıcılar_Id",
                        column: x => x.Id,
                        principalTable: "Kullanıcılar",
                        principalColumn: "KullanıcıId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Salonlar",
                columns: table => new
                {
                    SalonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonaAdı = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TesisId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salonlar", x => x.SalonId);
                    table.ForeignKey(
                        name: "FK_Salonlar_Tesisler_TesisId",
                        column: x => x.TesisId,
                        principalTable: "Tesisler",
                        principalColumn: "TesisId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalonBrans",
                columns: table => new
                {
                    SalonId = table.Column<int>(type: "int", nullable: false),
                    BransId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalonBrans", x => new { x.SalonId, x.BransId });
                    table.ForeignKey(
                        name: "FK_SalonBrans_Branslar_BransId",
                        column: x => x.BransId,
                        principalTable: "Branslar",
                        principalColumn: "BransId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalonBrans_Salonlar_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salonlar",
                        principalColumn: "SalonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seanslar",
                columns: table => new
                {
                    TesisId = table.Column<int>(type: "int", nullable: false),
                    SalonId = table.Column<int>(type: "int", nullable: false),
                    SeansSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    Tarih = table.Column<DateTime>(type: "date", nullable: false),
                    SeansId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kontenjan = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    yapılanBrans = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeansBaslangicZamani = table.Column<DateTime>(type: "datetime2", nullable: false, computedColumnSql: "CAST([Tarih] AS DATETIME) + CAST([SeansSaati] AS DATETIME)"),
                    Dolu = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SeansCinsiyet = table.Column<int>(type: "int", nullable: false),
                    RezerveEdenKisiSayisi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seanslar", x => new { x.TesisId, x.SalonId, x.SeansSaati, x.Tarih });
                    table.UniqueConstraint("AK_Seanslar_SeansId", x => x.SeansId);
                    table.ForeignKey(
                        name: "FK_Seanslar_Salonlar_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salonlar",
                        principalColumn: "SalonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seanslar_Tesisler_TesisId",
                        column: x => x.TesisId,
                        principalTable: "Tesisler",
                        principalColumn: "TesisId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rezervasyonlar",
                columns: table => new
                {
                    KullanıcıId = table.Column<int>(type: "int", nullable: false),
                    SeansId = table.Column<int>(type: "int", nullable: false),
                    RezerveEdilmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IptalEdildi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervasyonlar", x => new { x.SeansId, x.KullanıcıId });
                    table.ForeignKey(
                        name: "FK_Rezervasyonlar_Kullanıcılar_KullanıcıId",
                        column: x => x.KullanıcıId,
                        principalTable: "Kullanıcılar",
                        principalColumn: "KullanıcıId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rezervasyonlar_Seanslar_SeansId",
                        column: x => x.SeansId,
                        principalTable: "Seanslar",
                        principalColumn: "SeansId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_KullanıcıId",
                table: "Rezervasyonlar",
                column: "KullanıcıId");

            migrationBuilder.CreateIndex(
                name: "IX_SalonBrans_BransId",
                table: "SalonBrans",
                column: "BransId");

            migrationBuilder.CreateIndex(
                name: "IX_Salonlar_TesisId",
                table: "Salonlar",
                column: "TesisId");

            migrationBuilder.CreateIndex(
                name: "IX_Seanslar_SalonId",
                table: "Seanslar",
                column: "SalonId");

            migrationBuilder.CreateIndex(
                name: "IX_Seanslar_SeansId",
                table: "Seanslar",
                column: "SeansId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KullanıcıDetayları");

            migrationBuilder.DropTable(
                name: "Rezervasyonlar");

            migrationBuilder.DropTable(
                name: "SalonBrans");

            migrationBuilder.DropTable(
                name: "Kullanıcılar");

            migrationBuilder.DropTable(
                name: "Seanslar");

            migrationBuilder.DropTable(
                name: "Branslar");

            migrationBuilder.DropTable(
                name: "Salonlar");

            migrationBuilder.DropTable(
                name: "Tesisler");
        }
    }
}
