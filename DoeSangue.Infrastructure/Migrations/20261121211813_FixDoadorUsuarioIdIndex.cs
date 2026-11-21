using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoeSangue.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDoadorUsuarioIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Doadores_UsuarioId",
                table: "Doadores");

            migrationBuilder.CreateIndex(
                name: "IX_Doadores_UsuarioId",
                table: "Doadores",
                column: "UsuarioId",
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Doadores_UsuarioId",
                table: "Doadores");

            migrationBuilder.CreateIndex(
                name: "IX_Doadores_UsuarioId",
                table: "Doadores",
                column: "UsuarioId",
                unique: true);
        }
    }
}
