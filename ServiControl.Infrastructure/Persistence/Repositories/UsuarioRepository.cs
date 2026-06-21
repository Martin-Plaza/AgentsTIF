using Microsoft.EntityFrameworkCore;
using ServiControl.Application.Interfaces;
using ServiControl.Domain.Entities;
using ServiControl.Infrastructure.Persistence.Context;

namespace ServiControl.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(usuario => usuario.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(usuario => usuario.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(usuario => usuario.Email == email, cancellationToken);
    }

    public async Task<bool> HasRelatedRecordsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var tieneTrabajos = await Context.Set<Trabajo>()
            .AnyAsync(trabajo => trabajo.UsuarioId == id, cancellationToken);

        if (tieneTrabajos)
        {
            return true;
        }

        return await Context.Set<Metrica>()
            .AnyAsync(metrica => metrica.UsuarioId == id, cancellationToken);
    }
}
