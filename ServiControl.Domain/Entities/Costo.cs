namespace ServiControl.Domain.Entities;

public class Costo
{
    public int Id { get; private set; }
    public int TrabajoId { get; private set; }
    public decimal? CostoEstimado { get; private set; }
    public decimal? CostoFinal { get; private set; }

    public Costo(int trabajoId, decimal? costoEstimado = null, decimal? costoFinal = null)
    {
        if (trabajoId <= 0)
        {
            throw new ArgumentException("El trabajo asociado es obligatorio.", nameof(trabajoId));
        }

        ValidarCostoNoNegativo(costoEstimado, nameof(costoEstimado));
        ValidarCostoNoNegativo(costoFinal, nameof(costoFinal));

        TrabajoId = trabajoId;
        CostoEstimado = costoEstimado;
        CostoFinal = costoFinal;
    }

    public void RegistrarCostoEstimado(decimal costoEstimado)
    {
        ValidarCostoNoNegativo(costoEstimado, nameof(costoEstimado));
        CostoEstimado = costoEstimado;
    }

    public void RegistrarCostoFinal(decimal costoFinal)
    {
        ValidarCostoNoNegativo(costoFinal, nameof(costoFinal));
        CostoFinal = costoFinal;
    }

    private static void ValidarCostoNoNegativo(decimal? costo, string parameterName)
    {
        if (costo < 0)
        {
            throw new ArgumentException("El costo no puede ser menor a cero.", parameterName);
        }
    }
}
