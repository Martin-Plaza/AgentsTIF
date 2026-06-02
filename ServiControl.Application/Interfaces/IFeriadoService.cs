using ServiControl.Application.DTOs;

namespace ServiControl.Application.Interfaces;

public interface IFeriadoService
{
    Task<IReadOnlyList<FeriadoDto>> GetFeriadosPorAnioAsync(
        int anio,
        CancellationToken cancellationToken = default);
}
