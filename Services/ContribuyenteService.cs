using DGII.Api.Data;
using DGII.Api.DTOs;
using DGII.Api.Exceptions;
using DGII.Api.Interfaces;
using DGII.Api.Models;
using DGII.Api.Validators;
using Microsoft.EntityFrameworkCore;

namespace DGII.Api.Services;

public class ContribuyenteService : IContribuyenteService
{
    private readonly DGIIContext _dbContext;

    public ContribuyenteService(DGIIContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<ContribuyenteItbisDto> Data, int Total)> GetAllAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ContribuyenteItbisQuery().OrderBy(c => c.Nombre);
        var total = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (data, total);
    }

    public async Task<IReadOnlyList<ContribuyenteItbisDto>> SearchAsync(
        string? nombre = null,
        string? rncCedula = null,
        string? tipo = null,
        string? estatus = null,
        CancellationToken cancellationToken = default)
    {
        var query = ContribuyenteItbisQuery();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(c => c.Nombre.Contains(nombre));
        }

        if (!string.IsNullOrWhiteSpace(rncCedula))
        {
            query = query.Where(c => c.RncCedula.Contains(rncCedula));
        }

        if (!string.IsNullOrWhiteSpace(tipo))
        {
            query = query.Where(c => c.Tipo == tipo);
        }

        if (!string.IsNullOrWhiteSpace(estatus))
        {
            query = query.Where(c => c.Estatus == estatus);
        }

        return await query
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<ContribuyenteDetalleDto> GetByRncCedulaAsync(
        string rncCedula,
        CancellationToken cancellationToken = default)
    {
        var normalizedRncCedula = RncCedulaValidator.Normalize(rncCedula);
        var contribuyente = await _dbContext.Contribuyentes
            .AsNoTracking()
            .Where(c => c.RncCedula == normalizedRncCedula)
            .Select(c => new ContribuyenteDetalleDto(
                c.RncCedula,
                c.Nombre,
                c.Tipo,
                c.Estatus,
                c.ComprobantesFiscales.Count(f => f.Estado == ComprobanteFiscal.EstadoActivo),
                c.ComprobantesFiscales
                    .Where(f => f.Estado == ComprobanteFiscal.EstadoActivo)
                    .Sum(f => (decimal?)f.Itbis18) ?? 0m))
            .FirstOrDefaultAsync(cancellationToken);

        return contribuyente ?? throw new NotFoundException(
            $"Taxpayer with RNC/Cédula '{normalizedRncCedula}' was not found.");
    }

    private IQueryable<ContribuyenteItbisDto> ContribuyenteItbisQuery()
    {
        return _dbContext.Contribuyentes
            .AsNoTracking()
            .Select(c => new ContribuyenteItbisDto(
                c.RncCedula,
                c.Nombre,
                c.Tipo,
                c.Estatus,
                c.ComprobantesFiscales.Count(f => f.Estado == ComprobanteFiscal.EstadoActivo),
                c.ComprobantesFiscales
                    .Where(f => f.Estado == ComprobanteFiscal.EstadoActivo)
                    .Sum(f => (decimal?)f.Itbis18) ?? 0m));
    }
}