using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EruKampusSpor.Migrations
{
    /// <inheritdoc />
    public partial class mig_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IptalEdilenBrans",
                table: "Rezervasyonlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IptalEdilenBrans",
                table: "Rezervasyonlar");
        }
    }
}
