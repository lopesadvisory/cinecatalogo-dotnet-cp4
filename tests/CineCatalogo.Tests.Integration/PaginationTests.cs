using System.Net.Http.Json;
using CineCatalogo.Application.Dtos;
using Xunit;

namespace CineCatalogo.Tests.Integration;

public class PaginationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PaginationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ComPageSizeMaiorQueMaximoPermitido_DeveSerLimitadoA50()
    {
        var response = await _client.GetAsync("/api/filmes?pageNumber=1&pageSize=9999");
        response.EnsureSuccessStatusCode();

        var resultado = await response.Content.ReadFromJsonAsync<PagedResultDto<FilmeDto>>();

        Assert.NotNull(resultado);
        Assert.Equal(50, resultado!.PageSize);
    }

    [Fact]
    public async Task GetAll_PaginasDiferentes_NaoDevemRepetirItens()
    {
        var prefixo = $"PagTeste-{Guid.NewGuid():N}-";
        var diretorResponse = await _client.PostAsJsonAsync("/api/diretores", new CreateDiretorDto
        {
            Nome = $"Diretor Paginacao {Guid.NewGuid()}",
            Nacionalidade = "Brasileira",
            DataNascimento = new DateTime(1990, 1, 1)
        });
        var diretor = await diretorResponse.Content.ReadFromJsonAsync<DiretorDto>();

        for (var i = 1; i <= 5; i++)
        {
            await _client.PostAsJsonAsync("/api/filmes", new CreateFilmeDto
            {
                Titulo = $"{prefixo}{i:00}",
                Genero = "Documentário",
                AnoLancamento = 2000 + i,
                DuracaoMinutos = 90,
                DiretorId = diretor!.Id
            });
        }

        var pagina1 = await _client.GetFromJsonAsync<PagedResultDto<FilmeDto>>("/api/filmes?pageNumber=1&pageSize=2");
        var pagina2 = await _client.GetFromJsonAsync<PagedResultDto<FilmeDto>>("/api/filmes?pageNumber=2&pageSize=2");

        var idsPagina1 = pagina1!.Items.Select(f => f.Id).ToHashSet();
        var idsPagina2 = pagina2!.Items.Select(f => f.Id).ToHashSet();

        Assert.Empty(idsPagina1.Intersect(idsPagina2));
        Assert.True(pagina1.HasNextPage);
        Assert.True(pagina2.HasPreviousPage);
    }
}
