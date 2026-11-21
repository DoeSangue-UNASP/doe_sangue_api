using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoeSangue.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSexoTipoSanguineoDoador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sexo",
                table: "Doadores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoSanguineo",
                table: "Doadores",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sexo",
                table: "Doadores");

            migrationBuilder.DropColumn(
                name: "TipoSanguineo",
                table: "Doadores");
        }
    }
}
