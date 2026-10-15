
using DGII.Api.Data;
using DGII.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DGII.Api.Controllers;

[ApiController]
[Route("api/reportes")]
public class ReportesController : ControllerBase
{
    private readonly DGIIContext _context;

    public ReportesController(DGIIContext context)
    {
        _context = context;
    }

    // GET: api/reportes/itbis-por-contribuyente
    [HttpGet("itbis-por-contribuyente")]
    public async Task<IActionResult> GetItbisPorContribuyente(
        [FromQuery] string? tipo = null,
        [FromQuery] string? estatus = null)
    {
        var query = _context.Contribuyentes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(c => c.Tipo == tipo);

        if (!string.IsNullOrWhiteSpace(estatus))
            query = query.Where(c => c.Estatus == estatus);

        var data = await query
            .OrderBy(c => c.Nombre)
            .Select(c => new
            {
                c.RncCedula,
                c.Nombre,
                c.Tipo,
                c.Estatus,

                CantidadComprobantes = _context.ComprobantesFiscales
                    .Count(f => f.RncCedula == c.RncCedula
                             && f.Estado == ComprobanteFiscal.EstadoVigente),

                TotalMonto = _context.ComprobantesFiscales
                    .Where(f => f.RncCedula == c.RncCedula
                             && f.Estado == ComprobanteFiscal.EstadoVigente)
                    .Select(f => (decimal?)f.Monto)
                    .Sum() ?? 0m,

                TotalItbis = _context.ComprobantesFiscales
                    .Where(f => f.RncCedula == c.RncCedula
                             && f.Estado == ComprobanteFiscal.EstadoVigente)
                    .Select(f => (decimal?)f.Itbis18)
                    .Sum() ?? 0m
            })
            .ToListAsync();

        return Ok(data);
    }

    // GET: api/reportes/resumen
    [HttpGet("resumen")]
    public async Task<IActionResult> GetResumen()
    {
        var comprobantesActivos = _context.ComprobantesFiscales
            .AsNoTracking()
            .Where(f => f.Estado == ComprobanteFiscal.EstadoVigente);

        var totalContribuyentes =
            await _context.Contribuyentes.CountAsync();

        var contribuyentesActivos =
            await _context.Contribuyentes
                .CountAsync(c => c.Estatus == "ACTIVO");

        var contribuyentesInactivos =
            await _context.Contribuyentes
                .CountAsync(c => c.Estatus == "INACTIVO");

        var totalComprobantes = await comprobantesActivos.CountAsync();

        var totalMonto = await comprobantesActivos
            .Select(f => (decimal?)f.Monto)
            .SumAsync() ?? 0m;

        var totalItbis = await comprobantesActivos
            .Select(f => (decimal?)f.Itbis18)
            .SumAsync() ?? 0m;

        return Ok(new
        {
            TotalContribuyentes = totalContribuyentes,
            ContribuyentesActivos = contribuyentesActivos,
            ContribuyentesInactivos = contribuyentesInactivos,
            TotalComprobantesActivos = totalComprobantes,
            TotalMonto = totalMonto,
            TotalItbis = totalItbis
        });
    }
}
