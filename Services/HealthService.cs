using DGII.Api.DTOs;
using DGII.Api.Interfaces;

namespace DGII.Api.Services;

public class HealthService : IHealthService
{
    public Task<HealthStatusDto> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var status = new HealthStatusDto("Healthy", DateTimeOffset.UtcNow);
        return Task.FromResult(status);
    }
}
