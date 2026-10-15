using DGII.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DGII.Api.Controllers;

[ApiController]
[Route("api/contribuyentes")]
public class ContribuyentesController : ControllerBase
{
    private readonly IContribuyenteService _contribuyenteService;
    private readonly IComprobanteFiscalService _comprobanteFiscalService;

    public ContribuyentesController(
        IContribuyenteService contribuyenteService,
        IComprobanteFiscalService comprobanteFiscalService)
    {
        _contribuyenteService = contribuyenteService;
        _comprobanteFiscalService = comprobanteFiscalService;
    }

    // GET: api/contribuyentes
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize < 1)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Paginación inválida",
                detail: "La página y el tamaño deben ser mayores que cero.");
        }

        pageSize = Math.Min(pageSize, 100);

        var (data, total) = await _contribuyenteService.GetAllAsync(
            page,
            pageSize,
            cancellationToken);

        return Ok(new { page, pageSize, total, data });
    }

    // GET: api/contribuyentes/buscar?nombre=Juan
    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar(
        [FromQuery] string? nombre,
        [FromQuery] string? rncCedula,
        [FromQuery] string? tipo,
        [FromQuery] string? estatus,
        CancellationToken cancellationToken = default)
    {
        var data = await _contribuyenteService.SearchAsync(
            nombre,
            rncCedula,
            tipo,
            estatus,
            cancellationToken);

        return Ok(data);
    }

    // GET: api/contribuyentes/98754321012
    [HttpGet("{rncCedula}")]
    public async Task<IActionResult> GetByRncCedula(
        string rncCedula,
        CancellationToken cancellationToken)
    {
        var data = await _contribuyenteService.GetByRncCedulaAsync(
            rncCedula,
            cancellationToken);

        return Ok(data);
    }

    // GET: api/contribuyentes/98754321012/comprobantes
    [HttpGet("{rncCedula}/comprobantes")]
    public async Task<IActionResult> GetComprobantes(
        string rncCedula,
        CancellationToken cancellationToken = default)
    {
        var data = await _comprobanteFiscalService.GetByRncCedulaAsync(
            rncCedula,
            cancellationToken);

        return Ok(data);
    }
}   

