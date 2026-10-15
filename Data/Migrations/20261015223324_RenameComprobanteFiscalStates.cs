using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DGII.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameComprobanteFiscalStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.check_constraints
                    WHERE [name] = N'CK_ComprobanteFiscal_Estado'
                      AND [parent_object_id] = OBJECT_ID(N'[dbo].[ComprobantesFiscales]'))
                BEGIN
                    ALTER TABLE [dbo].[ComprobantesFiscales]
                    DROP CONSTRAINT [CK_ComprobanteFiscal_Estado];
                END
                """);

            migrationBuilder.Sql(
                "UPDATE [ComprobantesFiscales] SET [Estado] = CASE [Estado] WHEN N'ACTIVO' THEN N'VIGENTE' WHEN N'INACTIVO' THEN N'VENCIDO' ELSE [Estado] END");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ComprobanteFiscal_Estado",
                table: "ComprobantesFiscales",
                sql: "[Estado] IN (N'VIGENTE', N'VENCIDO')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.check_constraints
                    WHERE [name] = N'CK_ComprobanteFiscal_Estado'
                      AND [parent_object_id] = OBJECT_ID(N'[dbo].[ComprobantesFiscales]'))
                BEGIN
                    ALTER TABLE [dbo].[ComprobantesFiscales]
                    DROP CONSTRAINT [CK_ComprobanteFiscal_Estado];
                END
                """);

            migrationBuilder.Sql(
                "UPDATE [ComprobantesFiscales] SET [Estado] = CASE [Estado] WHEN N'VIGENTE' THEN N'ACTIVO' WHEN N'VENCIDO' THEN N'INACTIVO' ELSE [Estado] END");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ComprobanteFiscal_Estado",
                table: "ComprobantesFiscales",
                sql: "[Estado] IN (N'ACTIVO', N'INACTIVO')");
        }
    }
}
