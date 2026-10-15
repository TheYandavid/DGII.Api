using DGII.Api.Data;
using DGII.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DGII.Api.Tests.Infrastructure;

public sealed class SqliteTestModelCustomizer(ModelCustomizerDependencies dependencies)
    : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);

        modelBuilder.Entity<Contribuyente>().ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_Contribuyente_Tipo",
                "[Tipo] IN ('PERSONA FISICA', 'PERSONA JURIDICA')");
            table.HasCheckConstraint(
                "CK_Contribuyente_Estatus",
                "[Estatus] IN ('ACTIVO', 'INACTIVO')");
        });

        modelBuilder.Entity<ComprobanteFiscal>().ToTable(table =>
            table.HasCheckConstraint(
                "CK_ComprobanteFiscal_Estado",
                "[Estado] IN ('ACTIVO', 'INACTIVO')"));
    }
}
