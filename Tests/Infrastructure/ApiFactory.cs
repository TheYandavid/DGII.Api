using DGII.Api.Data;
using DGII.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.TestHost;

namespace DGII.Api.Tests.Infrastructure;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    public ApiFactory()
    {
        _connection.Open();
        using var client = CreateClient();
        SeedDatabase();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DGIIContext>();
            services.RemoveAll<DbContextOptions<DGIIContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<DGIIContext>>();
            services.AddDbContext<DGIIContext>(options =>
                options.UseSqlite(_connection)
                    .ReplaceService<IModelCustomizer, SqliteTestModelCustomizer>());
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }

    private void SeedDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DGIIContext>();
        context.Database.EnsureCreated();

        var ana = new Contribuyente
        {
            RncCedula = "98754321012",
            Nombre = "ANA PEREZ",
            Tipo = Contribuyente.TipoPersonaFisica,
            Estatus = Contribuyente.EstatusActivo,
            FechaRegistro = new DateTime(2024, 1, 1)
        };
        var carlos = new Contribuyente
        {
            RncCedula = "00101564321",
            Nombre = "CARLOS GOMEZ",
            Tipo = Contribuyente.TipoPersonaFisica,
            Estatus = Contribuyente.EstatusInactivo,
            FechaRegistro = new DateTime(2024, 1, 2)
        };

        context.Contribuyentes.AddRange(ana, carlos);
        context.ComprobantesFiscales.AddRange(
            new ComprobanteFiscal
            {
                RncCedula = ana.RncCedula,
                NCF = "E310000000001",
                FechaEmision = new DateTime(2024, 2, 1),
                Monto = 100m,
                Itbis18 = 18m,
                Estado = ComprobanteFiscal.EstadoVigente,
                FechaRegistro = new DateTime(2024, 2, 1)
            },
            new ComprobanteFiscal
            {
                RncCedula = ana.RncCedula,
                NCF = "E310000000002",
                FechaEmision = new DateTime(2024, 2, 2),
                Monto = 50m,
                Itbis18 = 9m,
                Estado = ComprobanteFiscal.EstadoVencido,
                FechaRegistro = new DateTime(2024, 2, 2)
            },
            new ComprobanteFiscal
            {
                RncCedula = carlos.RncCedula,
                NCF = "B010000000001",
                FechaEmision = new DateTime(2024, 2, 3),
                Monto = 25m,
                Itbis18 = 4.5m,
                Estado = ComprobanteFiscal.EstadoVigente,
                FechaRegistro = new DateTime(2024, 2, 3)
            });

        context.SaveChanges();
    }
}
