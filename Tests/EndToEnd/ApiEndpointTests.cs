using System.Net;
using System.Text.Json;
using DGII.Api.Tests.Infrastructure;
using Xunit;

namespace DGII.Api.Tests.EndToEnd;

public class ApiEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public ApiEndpointTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    public async Task GetHealth_ReturnsHealthyStatus()
    {
        using var response = await _client.GetAsync("/api/health");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", body.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    public async Task GetContribuyentes_ReturnsPagedData()
    {
        using var response = await _client.GetAsync("/api/contribuyentes?page=1&pageSize=1");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, body.RootElement.GetProperty("total").GetInt32());
        Assert.Single(body.RootElement.GetProperty("data").EnumerateArray());
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    public async Task SearchContribuyentes_FiltersByName()
    {
        using var response = await _client.GetAsync("/api/contribuyentes/buscar?nombre=ANA");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var taxpayer = Assert.Single(body.RootElement.EnumerateArray());
        Assert.Equal("ANA PEREZ", taxpayer.GetProperty("nombre").GetString());
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    public async Task GetContributor_ReturnsActiveReceiptTotals()
    {
        using var response = await _client.GetAsync("/api/contribuyentes/98754321012");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, body.RootElement.GetProperty("cantidadComprobantes").GetInt32());
        Assert.Equal(18m, body.RootElement.GetProperty("totalItbis").GetDecimal());
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    public async Task GetContributorReceipts_ReturnsVigenteAndVencidoDocuments()
    {
        using var response = await _client.GetAsync("/api/contribuyentes/98754321012/comprobantes");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var receipts = body.RootElement.EnumerateArray().ToArray();
        Assert.Equal(2, receipts.Length);
        Assert.Contains(receipts, receipt => receipt.GetProperty("estado").GetString() == "VENCIDO");
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    public async Task GetComprobantes_AppliesRncAndStatusFilters()
    {
        using var response = await _client.GetAsync(
            "/api/comprobantes?page=1&pageSize=10&rncCedula=98754321012&estado=VIGENTE");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, body.RootElement.GetProperty("total").GetInt32());
        var receipt = Assert.Single(body.RootElement.GetProperty("data").EnumerateArray());
        Assert.Equal("E310000000001", receipt.GetProperty("ncf").GetString());
        Assert.Equal("ANA PEREZ", receipt.GetProperty("nombreContribuyente").GetString());
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    public async Task GetMissingContributor_ReturnsProblemDetailsNotFound()
    {
        using var response = await _client.GetAsync("/api/contribuyentes/99999999999");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(404, body.RootElement.GetProperty("status").GetInt32());
        Assert.True(body.RootElement.TryGetProperty("traceId", out _));
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    public async Task GetContributorWithInvalidRnc_ReturnsProblemDetailsBadRequest()
    {
        using var response = await _client.GetAsync("/api/contribuyentes/ABC");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(400, body.RootElement.GetProperty("status").GetInt32());
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    public async Task GetComprobantesWithInvalidPage_ReturnsBadRequest()
    {
        using var response = await _client.GetAsync("/api/comprobantes?page=0");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Paginación inválida", body.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    [Trait("Category", "EndToEnd")]
    public async Task GetDashboardReport_ReturnsTotalsForActiveDocuments()
    {
        using var response = await _client.GetAsync("/api/reportes/resumen");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, body.RootElement.GetProperty("totalContribuyentes").GetInt32());
        Assert.Equal(2, body.RootElement.GetProperty("totalComprobantesActivos").GetInt32());
        Assert.Equal(22.5m, body.RootElement.GetProperty("totalItbis").GetDecimal());
    }
}
