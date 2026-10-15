using DGII.Api.DTOs;

namespace DGII.Api.Interfaces;

public interface IContribuyenteService
{
    Task<(IReadOnlyList<ContribuyenteItbisDto> Data, int Total)> GetAllAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContribuyenteItbisDto>> SearchAsync(
        string? nombre = null,
        string? rncCedula = null,
        string? tipo = null,
        string? estatus = null,
        CancellationToken cancellationToken = default);

    Task<ContribuyenteDetalleDto> GetByRncCedulaAsync(
        string rncCedula,
        CancellationToken cancellationToken = default);
}