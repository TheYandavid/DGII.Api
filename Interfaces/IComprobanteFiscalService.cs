using DGII.Api.DTOs;

namespace DGII.Api.Interfaces;

public interface IComprobanteFiscalService
{
    Task<(IReadOnlyList<ComprobanteFiscalDto> Data, int Total)> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? rncCedula = null,
        string? ncf = null,
        string? estado = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ComprobanteFiscalDto>> GetByRncCedulaAsync(string rncCedula, CancellationToken cancellationToken = default);
}