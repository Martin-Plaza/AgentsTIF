using ServiControl.Application.DTOs;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;

namespace ServiControl.Application.Services;

public class CostoService : ICostoService
{
    private readonly ICostoRepository _costoRepository;
    private readonly ITrabajoRepository _trabajoRepository;

    public CostoService(ICostoRepository costoRepository, ITrabajoRepository trabajoRepository)
    {
        _costoRepository = costoRepository;
        _trabajoRepository = trabajoRepository;
    }

    public async Task<CostoResponse> RegistrarCostoEstimadoAsync(
        RegisterCostoEstimadoRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidarTrabajoExistenteAsync(request.TrabajoId, cancellationToken);

        var costo = await _costoRepository.GetByTrabajoIdAsync(request.TrabajoId, cancellationToken);

        if (costo is null)
        {
            costo = new Costo(request.TrabajoId, request.CostoEstimado);
            var created = await _costoRepository.AddAsync(costo, cancellationToken);

            return MapToResponse(created);
        }

        costo.RegistrarCostoEstimado(request.CostoEstimado);
        await _costoRepository.UpdateAsync(costo, cancellationToken);

        return MapToResponse(costo);
    }

    public async Task<CostoResponse> RegistrarCostoFinalAsync(
        RegisterCostoFinalRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidarTrabajoExistenteAsync(request.TrabajoId, cancellationToken);

        var costo = await _costoRepository.GetByTrabajoIdAsync(request.TrabajoId, cancellationToken);

        if (costo is null)
        {
            costo = new Costo(request.TrabajoId, costoFinal: request.CostoFinal);
            var created = await _costoRepository.AddAsync(costo, cancellationToken);

            return MapToResponse(created);
        }

        costo.RegistrarCostoFinal(request.CostoFinal);
        await _costoRepository.UpdateAsync(costo, cancellationToken);

        return MapToResponse(costo);
    }

    private async Task ValidarTrabajoExistenteAsync(int trabajoId, CancellationToken cancellationToken)
    {
        if (await _trabajoRepository.GetByIdAsync(trabajoId, cancellationToken) is null)
        {
            throw new ArgumentException("El trabajo indicado no existe.", nameof(trabajoId));
        }
    }

    private static CostoResponse MapToResponse(Costo costo)
    {
        return new CostoResponse(
            costo.Id,
            costo.TrabajoId,
            costo.CostoEstimado,
            costo.CostoFinal);
    }
}
