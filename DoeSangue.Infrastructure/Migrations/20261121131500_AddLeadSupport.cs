using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoeSangue.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "Doadores",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "HemocentroId",
                table: "Doadores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doadores_HemocentroId",
                table: "Doadores",
                column: "HemocentroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doadores_Hemocentros_HemocentroId",
                table: "Doadores",
                column: "HemocentroId",
                principalTable: "Hemocentros",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doadores_Hemocentros_HemocentroId",
                table: "Doadores");

            migrationBuilder.DropIndex(
                name: "IX_Doadores_HemocentroId",
                table: "Doadores");

            migrationBuilder.DropColumn(
                name: "HemocentroId",
                table: "Doadores");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "Doadores",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
