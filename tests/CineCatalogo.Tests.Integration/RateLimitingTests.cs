using System.Net;
using Xunit;

namespace CineCatalogo.Tests.Integration;

public class RateLimitingTests : IClassFixture<LowRateLimitWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RateLimitingTests(LowRateLimitWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AposExcederOLimitePermitido_DeveRetornar429()
    {
        var statusCodes = new List<HttpStatusCode>();

        for (var i = 0; i < 5; i++)
        {
            var response = await _client.GetAsync("/api/diretores?pageSize=1");
            statusCodes.Add(response.StatusCode);
        }

        Assert.Contains(HttpStatusCode.OK, statusCodes);
        Assert.Contains(HttpStatusCode.TooManyRequests, statusCodes);
    }
}
