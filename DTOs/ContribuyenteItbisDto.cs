namespace DGII.Api.DTOs;

public sealed record ContribuyenteItbisDto(
    string RncCedula,
    string Nombre,
    string Tipo,
    string Estatus,
    int CantidadComprobantesActivos,
    decimal TotalItbis);
