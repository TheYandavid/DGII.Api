namespace DGII.Api.Interfaces;

public interface IDevelopmentDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}