using DGII.Api.DTOs;

namespace DGII.Api.Interfaces;

public interface IHealthService
{
    Task<HealthStatusDto> GetStatusAsync(CancellationToken cancellationToken = default);
}