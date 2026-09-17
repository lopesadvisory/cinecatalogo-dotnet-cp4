using CineCatalogo.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CineCatalogo.Infrastructure.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;

    public DatabaseHealthCheck(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var podeConectar = await _context.Database.CanConnectAsync(cancellationToken);

            return podeConectar
                ? HealthCheckResult.Healthy("Banco de dados acessível.")
                : HealthCheckResult.Unhealthy("Não foi possível conectar ao banco de dados.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Falha ao verificar a conexão com o banco de dados.", ex);
        }
    }
}
