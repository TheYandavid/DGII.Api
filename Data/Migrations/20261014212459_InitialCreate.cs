using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DGII.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contribuyentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RncCedula = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estatus = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contribuyentes", x => x.Id);
                    table.UniqueConstraint("AK_Contribuyente_RncCedula", x => x.RncCedula);
                    table.CheckConstraint("CK_Contribuyente_Estatus", "[Estatus] IN (N'ACTIVO', N'INACTIVO')");
                    table.CheckConstraint("CK_Contribuyente_Tipo", "[Tipo] IN (N'PERSONA FISICA', N'PERSONA JURIDICA')");
                });

            migrationBuilder.CreateTable(
                name: "ComprobantesFiscales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RncCedula = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    NCF = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Itbis18 = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobantesFiscales", x => x.Id);
                    table.CheckConstraint("CK_ComprobanteFiscal_Estado", "[Estado] IN (N'ACTIVO', N'INACTIVO')");
                    table.ForeignKey(
                        name: "FK_ComprobantesFiscales_Contribuyentes_RncCedula",
                        column: x => x.RncCedula,
                        principalTable: "Contribuyentes",
                        principalColumn: "RncCedula",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesFiscales_RncCedula",
                table: "ComprobantesFiscales",
                column: "RncCedula");

            migrationBuilder.CreateIndex(
                name: "UX_ComprobantesFiscales_NCF",
                table: "ComprobantesFiscales",
                column: "NCF",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComprobantesFiscales");

            migrationBuilder.DropTable(
                name: "Contribuyentes");
        }
    }
}
