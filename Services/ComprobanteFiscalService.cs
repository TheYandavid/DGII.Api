using DGII.Api.Data;
using DGII.Api.DTOs;
using DGII.Api.Exceptions;
using DGII.Api.Interfaces;
using DGII.Api.Validators;
using Microsoft.EntityFrameworkCore;

namespace DGII.Api.Services;

public class ComprobanteFiscalService : IComprobanteFiscalService
{
    private readonly DGIIContext _dbContext;

    public ComprobanteFiscalService(DGIIContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<ComprobanteFiscalDto> Data, int Total)> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? rncCedula = null,
        string? ncf = null,
        string? estado = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ComprobantesFiscales
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(rncCedula))
        {
            query = query.Where(c => c.RncCedula == rncCedula);
        }

        if (!string.IsNullOrWhiteSpace(ncf))
        {
            query = query.Where(c => c.NCF.Contains(ncf));
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = query.Where(c => c.Estado == estado);
        }

        var total = await query.CountAsync(cancellationToken);
        var data = await MapComprobantes(query)
            .OrderByDescending(c => c.FechaEmision)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (data, total);
    }

    public async Task<IReadOnlyList<ComprobanteFiscalDto>> GetByRncCedulaAsync(string rncCedula, CancellationToken cancellationToken = default)
    {
        var normalizedRncCedula = RncCedulaValidator.Normalize(rncCedula);

        var taxpayerExists = await _dbContext.Contribuyentes
            .AsNoTracking()
            .AnyAsync(c => c.RncCedula == normalizedRncCedula, cancellationToken);

        if (!taxpayerExists)
        {
            throw new NotFoundException($"Taxpayer with RNC/Cédula '{normalizedRncCedula}' was not found.");
        }

        return await MapComprobantes(_dbContext.ComprobantesFiscales.Where(c => c.RncCedula == normalizedRncCedula))
            .OrderBy(c => c.NCF)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<ComprobanteFiscalDto> MapComprobantes(IQueryable<Models.ComprobanteFiscal> query)
    {
        return query
            .AsNoTracking()
            .Select(c => new ComprobanteFiscalDto(
                c.RncCedula,
                c.NCF,
                c.FechaEmision,
                c.Monto,
                c.Itbis18,
                c.Estado));
    }
}
