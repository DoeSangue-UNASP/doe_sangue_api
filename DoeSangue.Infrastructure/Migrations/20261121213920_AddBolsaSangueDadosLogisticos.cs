using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoeSangue.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBolsaSangueDadosLogisticos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataColeta",
                table: "BolsasSangue",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataValidade",
                table: "BolsasSangue",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Destino",
                table: "BolsasSangue",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalColeta",
                table: "BolsasSangue",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsavelColeta",
                table: "BolsasSangue",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataColeta",
                table: "BolsasSangue");

            migrationBuilder.DropColumn(
                name: "DataValidade",
                table: "BolsasSangue");

            migrationBuilder.DropColumn(
                name: "Destino",
                table: "BolsasSangue");

            migrationBuilder.DropColumn(
                name: "LocalColeta",
                table: "BolsasSangue");

            migrationBuilder.DropColumn(
                name: "ResponsavelColeta",
                table: "BolsasSangue");
        }
    }
}
