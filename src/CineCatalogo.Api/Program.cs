using System.IO.Compression;
using System.Threading.RateLimiting;
using CineCatalogo.Api;
using CineCatalogo.Api.Middleware;
using CineCatalogo.Application.Interfaces;
using CineCatalogo.Application.Services;
using CineCatalogo.Infrastructure;
using CineCatalogo.Infrastructure.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Plataformas como o Render fornecem a porta via variável de ambiente PORT
// e terminam o HTTPS na borda, repassando a requisição em HTTP para o container.
var cloudPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(cloudPort))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{cloudPort}");
}

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "logs", "log-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CineCatálogo API",
        Version = "v1",
        Description = "API para gerenciamento de um catálogo de filmes e diretores, desenvolvida em Clean Architecture com .NET 8 (Checkpoint 4)."
    });
    options.EnableAnnotations();
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IDiretorService, DiretorService>();
builder.Services.AddScoped<IFilmeService, FilmeService>();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // As configurações são lidas em tempo de requisição (via IConfiguration do próprio
    // request) em vez de capturadas em variáveis antes do Build(), para que overrides de
    // configuração aplicados depois (ex.: testes de integração) tenham efeito.
    options.AddPolicy("padrao", httpContext =>
    {
        var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();
        var permitLimit = configuration.GetValue<int?>("RateLimiting:PermitLimit") ?? 30;
        var windowSeconds = configuration.GetValue<int?>("RateLimiting:WindowSeconds") ?? 10;
        var queueLimit = configuration.GetValue<int?>("RateLimiting:QueueLimit") ?? 0;

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "sem-ip",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromSeconds(windowSeconds),
                QueueLimit = queueLimit,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });
});

var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = appInsightsConnectionString;
    });
}
else
{
    builder.Services.AddApplicationInsightsTelemetry();
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await DbSeeder.SeedAsync(db);
}

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    // Plataformas como o Render têm múltiplos saltos internos antes do container; o padrão
    // (ForwardLimit = 1) só desembrulha um salto do X-Forwarded-For e deixa o RemoteIpAddress
    // com um IP interno em vez do IP real do cliente. Sem limite, ele percorre a cadeia inteira.
    ForwardLimit = null
};
// O proxy do Render não tem IP fixo conhecido antecipadamente, então confiamos em
// qualquer origem para esses cabeçalhos (o container só é alcançável através dele).
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

app.UseResponseCompression();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

try
{
    Log.Information("Iniciando a CineCatálogo API");
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{
}
