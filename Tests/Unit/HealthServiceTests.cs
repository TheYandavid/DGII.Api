using DGII.Api.Services;
using Xunit;

namespace DGII.Api.Tests.Unit;

public class HealthServiceTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetStatusAsync_ReturnsHealthyStatusWithUtcTimestamp()
    {
        var service = new HealthService();

        var result = await service.GetStatusAsync();

        Assert.Equal("Healthy", result.Status);
        Assert.Equal(TimeSpan.Zero, result.Timestamp.Offset);
        Assert.True(result.Timestamp <= DateTimeOffset.UtcNow);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetStatusAsync_ObservesCancellation()
    {
        var service = new HealthService();
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => service.GetStatusAsync(cancellation.Token));
    }
}
