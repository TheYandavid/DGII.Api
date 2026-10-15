using DGII.Api.Services;
using DGII.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DGII.Api.Controllers;

[ApiController]
[Route("api/comprobantes")]
public class ComprobantesController : ControllerBase
{
    private readonly IComprobanteFiscalService _comprobanteFiscalService;

    public ComprobantesController(IComprobanteFiscalService comprobanteFiscalService)
    {
        _comprobanteFiscalService = comprobanteFiscalService;
    }

    // GET: api/comprobantes
    // GET: api/comprobantes?rncCedula=98754321012
    // GET: api/comprobantes?ncf=B01
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? rncCedula = null,
        [FromQuery] string? ncf = null,
        [FromQuery] string? estado = null,
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

        var (data, total) = await _comprobanteFiscalService.GetAllAsync(
            page,
            pageSize,
            rncCedula,
            ncf,
            estado,
            cancellationToken);

        return Ok(new { page, pageSize, total, data });
    }
}
