using CineCatalogo.Domain.Interfaces;
using CineCatalogo.Infrastructure.Data;
using CineCatalogo.Infrastructure.HealthChecks;
using CineCatalogo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CineCatalogo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=cinecatalogo.db";

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IDiretorRepository, DiretorRepository>();
        services.AddScoped<IFilmeRepository, FilmeRepository>();

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready" });

        return services;
    }
}
