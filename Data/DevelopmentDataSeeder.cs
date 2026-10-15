using DGII.Api.Models;
using DGII.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DGII.Api.Data;

public class DevelopmentDataSeeder : IDevelopmentDataSeeder
{
    private readonly DGIIContext _dbContext;
    private readonly ILogger<DevelopmentDataSeeder> _logger;

    public DevelopmentDataSeeder(DGIIContext dbContext, ILogger<DevelopmentDataSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var seedRncs = DevelopmentSeedCatalog.Contribuyentes
            .Select(c => c.RncCedula)
            .ToList();

        var existingRncs = (await _dbContext.Contribuyentes
                .AsNoTracking()
                .Where(c => seedRncs.Contains(c.RncCedula))
                .Select(c => c.RncCedula)
                .ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.Ordinal);

        var newContribuyentes = DevelopmentSeedCatalog.Contribuyentes
            .Where(c => !existingRncs.Contains(c.RncCedula))
            .Select(CloneContribuyente)
            .ToList();

        if (newContribuyentes.Count > 0)
        {
            await _dbContext.Contribuyentes.AddRangeAsync(newContribuyentes, cancellationToken);
        }

        var availableRncs = existingRncs
            .Concat(newContribuyentes.Select(c => c.RncCedula))
            .ToHashSet(StringComparer.Ordinal);

        var seedNcfs = DevelopmentSeedCatalog.ComprobantesFiscales
            .Select(c => c.NCF)
            .ToList();

        var existingNcfs = (await _dbContext.ComprobantesFiscales
                .AsNoTracking()
                .Where(c => seedNcfs.Contains(c.NCF))
                .Select(c => c.NCF)
                .ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.Ordinal);

        var newComprobantes = DevelopmentSeedCatalog.ComprobantesFiscales
            .Where(c => !existingNcfs.Contains(c.NCF) && availableRncs.Contains(c.RncCedula))
            .Select(CloneComprobante)
            .ToList();

        if (newComprobantes.Count > 0)
        {
            await _dbContext.ComprobantesFiscales.AddRangeAsync(newComprobantes, cancellationToken);
        }

        if (!_dbContext.ChangeTracker.HasChanges())
        {
            _logger.LogInformation("Development seed skipped: all catalog records already exist.");
            return;
        }

        var inserted = await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Development seed inserted {ContribuyenteCount} contribuyentes and {ComprobanteCount} comprobantes ({Rows} rows).",
            newContribuyentes.Count,
            newComprobantes.Count,
            inserted);
    }

    private static Contribuyente CloneContribuyente(Contribuyente source) =>
        new()
        {
            RncCedula = source.RncCedula,
            Nombre = source.Nombre,
            Tipo = source.Tipo,
            Estatus = source.Estatus,
            FechaRegistro = source.FechaRegistro
        };

    private static ComprobanteFiscal CloneComprobante(ComprobanteFiscal source) =>
        new()
        {
            RncCedula = source.RncCedula,
            NCF = source.NCF,
            FechaEmision = source.FechaEmision,
            Monto = source.Monto,
            Itbis18 = source.Itbis18,
            Estado = source.Estado,
            FechaRegistro = source.FechaRegistro
        };
}
