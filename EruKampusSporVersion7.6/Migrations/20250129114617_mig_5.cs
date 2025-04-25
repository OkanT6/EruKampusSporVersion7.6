using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EruKampusSpor.Migrations
{
    /// <inheritdoc />
    public partial class mig_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilFotografiUrl",
                table: "KullanıcıDetayları",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilFotografiUrl",
                table: "KullanıcıDetayları");
        }
    }
}
