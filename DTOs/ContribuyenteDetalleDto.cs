namespace DGII.Api.DTOs;

public sealed record ContribuyenteDetalleDto(
    string RncCedula,
    string Nombre,
    string Tipo,
    string Estatus,
    int CantidadComprobantes,
    decimal TotalItbis);