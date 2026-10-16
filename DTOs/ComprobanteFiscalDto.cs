namespace DGII.Api.DTOs;

public sealed record ComprobanteFiscalDto(
    string RncCedula,
    string NombreContribuyente,
    string NCF,
    DateTime FechaEmision,
    decimal Monto,
    decimal Itbis18,
    string Estado);
