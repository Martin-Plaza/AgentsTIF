using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiControl.Application.Interfaces;
using ServiControl.Infrastructure.Authentication;
using ServiControl.Infrastructure.ExternalServices;
using ServiControl.Infrastructure.Persistence.Context;
using ServiControl.Infrastructure.Persistence.Repositories;
using ServiControl.Infrastructure.Security;

namespace ServiControl.Infrastructure;

// Modulo: Composicion de Infrastructure
// Capa: Infrastructure
// Responsabilidad: Registra implementaciones concretas de interfaces definidas en Application.
// Nota: Presentation llama esta extension sin conocer detalles de EF Core, JWT o servicios externos.
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // En Azure App Service esta connection string debe venir de Configuration/Connection strings.
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("La connection string 'DefaultConnection' no esta configurada.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ITrabajoRepository, TrabajoRepository>();
        services.AddScoped<ICostoRepository, CostoRepository>();
        services.AddScoped<IMetricaRepository, MetricaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        // HttpClientFactory evita crear HttpClient manualmente y administra conexiones de forma segura.
        services.AddHttpClient<IFeriadoService, FeriadoService>(client =>
        {
            client.BaseAddress = new Uri("https://api.argentinadatos.com/v1/");
        });

        return services;
    }
}
