using DGII.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DGII.Api.Data;

public class DGIIContext : DbContext
{
    public DGIIContext(DbContextOptions<DGIIContext> options)
        : base(options)
    {
    }

    public DbSet<Contribuyente> Contribuyentes => Set<Contribuyente>();
    public DbSet<ComprobanteFiscal> ComprobantesFiscales => Set<ComprobanteFiscal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contribuyente>(entity =>
        {
            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Contribuyente_Tipo",
                    $"[Tipo] IN (N'{Contribuyente.TipoPersonaFisica}', N'{Contribuyente.TipoPersonaJuridica}')");
                table.HasCheckConstraint(
                    "CK_Contribuyente_Estatus",
                    $"[Estatus] IN (N'{Contribuyente.EstatusActivo}', N'{Contribuyente.EstatusInactivo}')");
            });

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.RncCedula)
                .IsRequired()
                .HasMaxLength(11);

            entity.HasAlternateKey(e => e.RncCedula)
                .HasName("AK_Contribuyente_RncCedula");

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Tipo)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Estatus)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.HasMany(e => e.ComprobantesFiscales)
                .WithOne(e => e.Contribuyente)
                .HasForeignKey(e => e.RncCedula)
                .HasPrincipalKey(e => e.RncCedula)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ComprobantesFiscales_Contribuyentes_RncCedula");
        });

        modelBuilder.Entity<ComprobanteFiscal>(entity =>
        {
            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_ComprobanteFiscal_Estado",
                    $"[Estado] IN (N'{ComprobanteFiscal.EstadoActivo}', N'{ComprobanteFiscal.EstadoInactivo}')");
            });

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.RncCedula)
                .IsRequired()
                .HasMaxLength(11);

            entity.Property(e => e.NCF)
                .IsRequired()
                .HasMaxLength(19);

            entity.HasIndex(e => e.NCF)
                .IsUnique()
                .HasDatabaseName("UX_ComprobantesFiscales_NCF");

            entity.HasIndex(e => e.RncCedula)
                .HasDatabaseName("IX_ComprobantesFiscales_RncCedula");

            entity.Property(e => e.FechaEmision)
                .HasColumnType("datetime2")
                .IsRequired();

            entity.Property(e => e.Monto)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.Itbis18)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.Estado)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime2")
                .IsRequired();
        });
    }
}
