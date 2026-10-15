using DGII.Api.DTOs;
using DGII.Api.Interfaces;
using DGII.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DGII.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IHealthService _healthService;

    public HealthController(IHealthService healthService)
    {
        _healthService = healthService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HealthStatusDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HealthStatusDto>> Get(CancellationToken cancellationToken)
    {
        var status = await _healthService.GetStatusAsync(cancellationToken);
        return Ok(status);
    }
}
