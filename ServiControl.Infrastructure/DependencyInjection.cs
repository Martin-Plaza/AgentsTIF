using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiControl.Application.Interfaces;
using ServiControl.Infrastructure.Authentication;
using ServiControl.Infrastructure.Persistence.Context;
using ServiControl.Infrastructure.Persistence.Repositories;
using ServiControl.Infrastructure.Security;

namespace ServiControl.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("La connection string 'DefaultConnection' no esta configurada.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ITrabajoRepository, TrabajoRepository>();
        services.AddScoped<ICostoRepository, CostoRepository>();
        services.AddScoped<IMetricaRepository, MetricaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
