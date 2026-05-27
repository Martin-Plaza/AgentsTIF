namespace ServiControl.Domain.Entities;

public class Metrica
{
    public int Id { get; private set; }
    public int UsuarioId { get; private set; }
    public decimal MontoTotalPeriodo { get; private set; }
    public int CantidadTrabajos { get; private set; }
    public int TrabajosPendientes { get; private set; }
    public decimal MontoPromedioTrabajo { get; private set; }
    public DateTime PeriodoInicio { get; private set; }
    public DateTime PeriodoFin { get; private set; }

    public Metrica(
        int usuarioId,
        decimal montoTotalPeriodo,
        int cantidadTrabajos,
        int trabajosPendientes,
        DateTime periodoInicio,
        DateTime periodoFin)
    {
        if (usuarioId <= 0)
        {
            throw new ArgumentException("El usuario asociado es obligatorio.", nameof(usuarioId));
        }

        ValidarPeriodo(periodoInicio, periodoFin);
        ValidarValores(montoTotalPeriodo, cantidadTrabajos, trabajosPendientes);

        UsuarioId = usuarioId;
        MontoTotalPeriodo = montoTotalPeriodo;
        CantidadTrabajos = cantidadTrabajos;
        TrabajosPendientes = trabajosPendientes;
        PeriodoInicio = periodoInicio;
        PeriodoFin = periodoFin;
        MontoPromedioTrabajo = CalcularMontoPromedio(montoTotalPeriodo, cantidadTrabajos);
    }

    public void Actualizar(
        decimal montoTotalPeriodo,
        int cantidadTrabajos,
        int trabajosPendientes,
        DateTime periodoInicio,
        DateTime periodoFin)
    {
        ValidarPeriodo(periodoInicio, periodoFin);
        ValidarValores(montoTotalPeriodo, cantidadTrabajos, trabajosPendientes);

        MontoTotalPeriodo = montoTotalPeriodo;
        CantidadTrabajos = cantidadTrabajos;
        TrabajosPendientes = trabajosPendientes;
        PeriodoInicio = periodoInicio;
        PeriodoFin = periodoFin;
        MontoPromedioTrabajo = CalcularMontoPromedio(montoTotalPeriodo, cantidadTrabajos);
    }

    private static void ValidarPeriodo(DateTime periodoInicio, DateTime periodoFin)
    {
        if (periodoInicio > periodoFin)
        {
            throw new ArgumentException("La fecha de inicio del periodo debe ser menor o igual a la fecha de fin.");
        }
    }

    private static void ValidarValores(decimal montoTotalPeriodo, int cantidadTrabajos, int trabajosPendientes)
    {
        if (montoTotalPeriodo < 0)
        {
            throw new ArgumentException("El monto total del periodo no puede ser menor a cero.", nameof(montoTotalPeriodo));
        }

        if (cantidadTrabajos < 0)
        {
            throw new ArgumentException("La cantidad de trabajos no puede ser menor a cero.", nameof(cantidadTrabajos));
        }

        if (trabajosPendientes < 0)
        {
            throw new ArgumentException("La cantidad de trabajos pendientes no puede ser menor a cero.", nameof(trabajosPendientes));
        }
    }

    private static decimal CalcularMontoPromedio(decimal montoTotalPeriodo, int cantidadTrabajos)
    {
        return cantidadTrabajos == 0 ? 0 : montoTotalPeriodo / cantidadTrabajos;
    }
}
