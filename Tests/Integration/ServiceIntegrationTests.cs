using DGII.Api.Exceptions;
using DGII.Api.Services;
using DGII.Api.Tests.Infrastructure;
using Xunit;

namespace DGII.Api.Tests.Integration;

public class ServiceIntegrationTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task ContribuyenteService_GetAllAsync_PaginatesAndReturnsActiveReceiptTotals()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var service = new ContribuyenteService(context);

        var (data, total) = await service.GetAllAsync(page: 1, pageSize: 1);

        Assert.Equal(2, total);
        var taxpayer = Assert.Single(data);
        Assert.Equal("ANA PEREZ", taxpayer.Nombre);
        Assert.Equal(1, taxpayer.CantidadComprobantesActivos);
        Assert.Equal(18m, taxpayer.TotalItbis);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ContribuyenteService_SearchAsync_AppliesNameAndStatusFilters()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var service = new ContribuyenteService(context);

        var results = await service.SearchAsync(nombre: "CARLOS", estatus: "INACTIVO");

        var taxpayer = Assert.Single(results);
        Assert.Equal("00101564321", taxpayer.RncCedula);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ContribuyenteService_GetByRncCedulaAsync_ReturnsOnlyActiveReceiptTotals()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var service = new ContribuyenteService(context);

        var result = await service.GetByRncCedulaAsync(" 98754321012 ");

        Assert.Equal(1, result.CantidadComprobantes);
        Assert.Equal(18m, result.TotalItbis);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ContribuyenteService_GetByRncCedulaAsync_ThrowsWhenMissing()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var service = new ContribuyenteService(context);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetByRncCedulaAsync("99999999999"));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ComprobanteFiscalService_GetAllAsync_FiltersAndPaginates()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var service = new ComprobanteFiscalService(context);

        var (data, total) = await service.GetAllAsync(
            page: 1,
            pageSize: 1,
            rncCedula: "98754321012");

        Assert.Equal(2, total);
        var receipt = Assert.Single(data);
        Assert.Equal("E310000000002", receipt.NCF);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ComprobanteFiscalService_GetByRncCedulaAsyncReturnsEveryStatus()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var service = new ComprobanteFiscalService(context);

        var receipts = await service.GetByRncCedulaAsync("98754321012");

        Assert.Equal(2, receipts.Count);
        Assert.Contains(receipts, receipt => receipt.Estado == "ACTIVO");
        Assert.Contains(receipts, receipt => receipt.Estado == "INACTIVO");
    }
}
