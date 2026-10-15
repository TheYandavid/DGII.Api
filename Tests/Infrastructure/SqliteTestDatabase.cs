using DGII.Api.Data;
using DGII.Api.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DGII.Api.Tests.Infrastructure;

public sealed class SqliteTestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<DGIIContext> _options;

    private SqliteTestDatabase(SqliteConnection connection)
    {
        _connection = connection;
        _options = new DbContextOptionsBuilder<DGIIContext>()
            .UseSqlite(_connection)
            .ReplaceService<IModelCustomizer, SqliteTestModelCustomizer>()
            .Options;
    }

    public static async Task<SqliteTestDatabase> CreateAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var database = new SqliteTestDatabase(connection);

        await using var context = database.CreateContext();
        await context.Database.EnsureCreatedAsync();
        await SeedAsync(context);

        return database;
    }

    public DGIIContext CreateContext() => new(_options);

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    private static async Task SeedAsync(DGIIContext context)
    {
        var activeTaxpayer = new Contribuyente
        {
            RncCedula = "98754321012",
            Nombre = "ANA PEREZ",
            Tipo = Contribuyente.TipoPersonaFisica,
            Estatus = Contribuyente.EstatusActivo,
            FechaRegistro = new DateTime(2024, 1, 1)
        };
        var inactiveTaxpayer = new Contribuyente
        {
            RncCedula = "00101564321",
            Nombre = "CARLOS GOMEZ",
            Tipo = Contribuyente.TipoPersonaFisica,
            Estatus = Contribuyente.EstatusInactivo,
            FechaRegistro = new DateTime(2024, 1, 2)
        };

        context.Contribuyentes.AddRange(activeTaxpayer, inactiveTaxpayer);
        context.ComprobantesFiscales.AddRange(
            new ComprobanteFiscal
            {
                RncCedula = activeTaxpayer.RncCedula,
                NCF = "E310000000001",
                FechaEmision = new DateTime(2024, 2, 1),
                Monto = 100m,
                Itbis18 = 18m,
                Estado = ComprobanteFiscal.EstadoVigente,
                FechaRegistro = new DateTime(2024, 2, 1)
            },
            new ComprobanteFiscal
            {
                RncCedula = activeTaxpayer.RncCedula,
                NCF = "E310000000002",
                FechaEmision = new DateTime(2024, 2, 2),
                Monto = 50m,
                Itbis18 = 9m,
                Estado = ComprobanteFiscal.EstadoVencido,
                FechaRegistro = new DateTime(2024, 2, 2)
            },
            new ComprobanteFiscal
            {
                RncCedula = inactiveTaxpayer.RncCedula,
                NCF = "B010000000001",
                FechaEmision = new DateTime(2024, 2, 3),
                Monto = 25m,
                Itbis18 = 4.5m,
                Estado = ComprobanteFiscal.EstadoVigente,
                FechaRegistro = new DateTime(2024, 2, 3)
            });

        await context.SaveChangesAsync();
    }
}
