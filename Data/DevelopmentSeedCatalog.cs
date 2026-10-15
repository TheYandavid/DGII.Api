using DGII.Api.Models;

namespace DGII.Api.Data;

internal static class DevelopmentSeedCatalog
{
    public static IReadOnlyList<Contribuyente> Contribuyentes { get; } =
    [
        new()
        {
            RncCedula = "98754321012",
            Nombre = "JUAN PEREZ",
            Tipo = Contribuyente.TipoPersonaFisica,
            Estatus = Contribuyente.EstatusActivo,
            FechaRegistro = new DateTime(2023, 1, 10, 8, 0, 0, DateTimeKind.Unspecified)
        },
        new()
        {
            RncCedula = "123456789",
            Nombre = "FARMACIA TU SALUD",
            Tipo = Contribuyente.TipoPersonaJuridica,
            Estatus = Contribuyente.EstatusInactivo,
            FechaRegistro = new DateTime(2023, 2, 5, 9, 0, 0, DateTimeKind.Unspecified)
        },
        new()
        {
            RncCedula = "40212345678",
            Nombre = "MARIA RODRIGUEZ",
            Tipo = Contribuyente.TipoPersonaFisica,
            Estatus = Contribuyente.EstatusActivo,
            FechaRegistro = new DateTime(2023, 3, 12, 10, 0, 0, DateTimeKind.Unspecified)
        },
        new()
        {
            RncCedula = "00101564321",
            Nombre = "CARLOS GOMEZ",
            Tipo = Contribuyente.TipoPersonaFisica,
            Estatus = Contribuyente.EstatusInactivo,
            FechaRegistro = new DateTime(2023, 4, 8, 11, 0, 0, DateTimeKind.Unspecified)
        },
        new()
        {
            RncCedula = "131098765",
            Nombre = "SUPERMERCADO EL SOL",
            Tipo = Contribuyente.TipoPersonaJuridica,
            Estatus = Contribuyente.EstatusActivo,
            FechaRegistro = new DateTime(2023, 5, 20, 8, 30, 0, DateTimeKind.Unspecified)
        },
        new()
        {
            RncCedula = "101234567",
            Nombre = "CONSTRUCTORA ATLANTICO",
            Tipo = Contribuyente.TipoPersonaJuridica,
            Estatus = Contribuyente.EstatusActivo,
            FechaRegistro = new DateTime(2023, 6, 1, 14, 0, 0, DateTimeKind.Unspecified)
        },
        new()
        {
            RncCedula = "40298765432",
            Nombre = "ANA MARTINEZ",
            Tipo = Contribuyente.TipoPersonaFisica,
            Estatus = Contribuyente.EstatusActivo,
            FechaRegistro = new DateTime(2023, 7, 18, 9, 15, 0, DateTimeKind.Unspecified)
        },
        new()
        {
            RncCedula = "223456789",
            Nombre = "HOTELES DEL CARIBE",
            Tipo = Contribuyente.TipoPersonaJuridica,
            Estatus = Contribuyente.EstatusInactivo,
            FechaRegistro = new DateTime(2023, 8, 9, 16, 0, 0, DateTimeKind.Unspecified)
        },
        new()
        {
            RncCedula = "00100876543",
            Nombre = "PEDRO SANTOS",
            Tipo = Contribuyente.TipoPersonaFisica,
            Estatus = Contribuyente.EstatusActivo,
            FechaRegistro = new DateTime(2023, 9, 2, 12, 0, 0, DateTimeKind.Unspecified)
        },
        new()
        {
            RncCedula = "130987654",
            Nombre = "DISTRIBUIDORA NORTE SRL",
            Tipo = Contribuyente.TipoPersonaJuridica,
            Estatus = Contribuyente.EstatusActivo,
            FechaRegistro = new DateTime(2023, 10, 14, 10, 45, 0, DateTimeKind.Unspecified)
        },
        new()
        {
            RncCedula = "40211122233",
            Nombre = "LUISA FERNANDEZ",
            Tipo = Contribuyente.TipoPersonaFisica,
            Estatus = Contribuyente.EstatusInactivo,
            FechaRegistro = new DateTime(2023, 11, 21, 8, 0, 0, DateTimeKind.Unspecified)
        }
    ];

    public static IReadOnlyList<ComprobanteFiscal> ComprobantesFiscales { get; } =
    [
        CreateReceipt("98754321012", "E310000000001", new DateTime(2024, 1, 15), 200.00m, 36.00m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("98754321012", "E310000000002", new DateTime(2024, 2, 20), 1000.00m, 180.00m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("98754321012", "E310000000003", new DateTime(2024, 3, 5), 500.00m, 90.00m, ComprobanteFiscal.EstadoVencido),
        CreateReceipt("98754321012", "E310000000004", new DateTime(2024, 4, 11), 150.50m, 27.09m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("40212345678", "E310000000101", new DateTime(2024, 1, 22), 320.00m, 57.60m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("40212345678", "E310000000102", new DateTime(2024, 2, 8), 80.00m, 14.40m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("40212345678", "E310000000103", new DateTime(2024, 3, 19), 450.75m, 81.14m, ComprobanteFiscal.EstadoVencido),
        CreateReceipt("00101564321", "B0100000001", new DateTime(2023, 12, 1), 210.00m, 37.80m, ComprobanteFiscal.EstadoVencido),
        CreateReceipt("00101564321", "B0100000002", new DateTime(2023, 12, 15), 95.00m, 17.10m, ComprobanteFiscal.EstadoVencido),
        CreateReceipt("131098765", "E310000000201", new DateTime(2024, 5, 2), 2400.00m, 432.00m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("131098765", "E310000000202", new DateTime(2024, 5, 18), 175.25m, 31.55m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("131098765", "E310000000203", new DateTime(2024, 6, 3), 890.00m, 160.20m, ComprobanteFiscal.EstadoVencido),
        CreateReceipt("101234567", "E310000000301", new DateTime(2024, 6, 21), 15000.00m, 2700.00m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("101234567", "E310000000302", new DateTime(2024, 7, 4), 8750.00m, 1575.00m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("101234567", "E310000000303", new DateTime(2024, 7, 19), 430.00m, 77.40m, ComprobanteFiscal.EstadoVencido),
        CreateReceipt("40298765432", "E310000000401", new DateTime(2024, 8, 1), 60.00m, 10.80m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("40298765432", "E310000000402", new DateTime(2024, 8, 12), 275.00m, 49.50m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("223456789", "B0200000001", new DateTime(2023, 9, 30), 5600.00m, 1008.00m, ComprobanteFiscal.EstadoVencido),
        CreateReceipt("00100876543", "E310000000501", new DateTime(2024, 9, 7), 125.00m, 22.50m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("00100876543", "E310000000502", new DateTime(2024, 9, 21), 340.00m, 61.20m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("130987654", "E310000000601", new DateTime(2024, 10, 5), 980.00m, 176.40m, ComprobanteFiscal.EstadoVigente),
        CreateReceipt("130987654", "E310000000602", new DateTime(2024, 10, 28), 215.80m, 38.84m, ComprobanteFiscal.EstadoVencido)
    ];

    private static ComprobanteFiscal CreateReceipt(
        string rncCedula,
        string ncf,
        DateTime fechaEmision,
        decimal monto,
        decimal itbis18,
        string estado) =>
        new()
        {
            RncCedula = rncCedula,
            NCF = ncf,
            FechaEmision = fechaEmision,
            Monto = monto,
            Itbis18 = itbis18,
            Estado = estado,
            FechaRegistro = fechaEmision.AddHours(2)
        };
}
