using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CineCatalogo.Tests.Integration;

public class LowRateLimitWebApplicationFactory : CustomWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RateLimiting:PermitLimit"] = "3",
                ["RateLimiting:WindowSeconds"] = "10",
                ["RateLimiting:QueueLimit"] = "0"
            });
        });
    }
}
