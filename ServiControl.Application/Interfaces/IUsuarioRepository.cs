namespace ServiControl.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
}
