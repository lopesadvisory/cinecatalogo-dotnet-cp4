using System.Net;
using System.Net.Http.Json;
using CineCatalogo.Application.Dtos;
using Xunit;

namespace CineCatalogo.Tests.Integration;

public class DiretoresEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DiretoresEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_DeveRetornarListaPaginada()
    {
        var response = await _client.GetAsync("/api/diretores?pageNumber=1&pageSize=2");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<DiretorDto>>();

        Assert.NotNull(result);
        Assert.Equal(1, result!.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.True(result.Items.Count <= 2);
        Assert.True(result.TotalCount >= result.Items.Count);
    }

    [Fact]
    public async Task Create_ComDadosValidos_DeveRetornar201EPersistir()
    {
        var novoDiretor = new CreateDiretorDto
        {
            Nome = $"Diretor Teste {Guid.NewGuid()}",
            Nacionalidade = "Brasileira",
            DataNascimento = new DateTime(1980, 1, 1)
        };

        var response = await _client.PostAsJsonAsync("/api/diretores", novoDiretor);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var criado = await response.Content.ReadFromJsonAsync<DiretorDto>();
        Assert.NotNull(criado);
        Assert.Equal(novoDiretor.Nome, criado!.Nome);

        var getResponse = await _client.GetAsync(response.Headers.Location);
        getResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Create_ComNomeDuplicado_DeveRetornar409()
    {
        var diretor = new CreateDiretorDto
        {
            Nome = $"Diretor Duplicado {Guid.NewGuid()}",
            Nacionalidade = "Brasileira",
            DataNascimento = new DateTime(1980, 1, 1)
        };

        var primeiraResposta = await _client.PostAsJsonAsync("/api/diretores", diretor);
        Assert.Equal(HttpStatusCode.Created, primeiraResposta.StatusCode);

        var segundaResposta = await _client.PostAsJsonAsync("/api/diretores", diretor);
        Assert.Equal(HttpStatusCode.Conflict, segundaResposta.StatusCode);
    }

    [Fact]
    public async Task Create_ComDadosInvalidos_DeveRetornar400()
    {
        var diretorInvalido = new CreateDiretorDto { Nome = "", Nacionalidade = "", DataNascimento = default };

        var response = await _client.PostAsJsonAsync("/api/diretores", diretorInvalido);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_QuandoNaoExiste_DeveRetornar404()
    {
        var response = await _client.GetAsync("/api/diretores/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task FluxoCompleto_CriarAtualizarERemover_DeveFuncionar()
    {
        var criarDto = new CreateDiretorDto
        {
            Nome = $"Diretor Fluxo {Guid.NewGuid()}",
            Nacionalidade = "Portuguesa",
            DataNascimento = new DateTime(1975, 5, 5)
        };
        var criarResponse = await _client.PostAsJsonAsync("/api/diretores", criarDto);
        var criado = await criarResponse.Content.ReadFromJsonAsync<DiretorDto>();

        var atualizarDto = new UpdateDiretorDto
        {
            Nome = criado!.Nome,
            Nacionalidade = "Espanhola",
            DataNascimento = criarDto.DataNascimento
        };
        var atualizarResponse = await _client.PutAsJsonAsync($"/api/diretores/{criado.Id}", atualizarDto);
        Assert.Equal(HttpStatusCode.OK, atualizarResponse.StatusCode);

        var atualizado = await atualizarResponse.Content.ReadFromJsonAsync<DiretorDto>();
        Assert.Equal("Espanhola", atualizado!.Nacionalidade);

        var removerResponse = await _client.DeleteAsync($"/api/diretores/{criado.Id}");
        Assert.Equal(HttpStatusCode.NoContent, removerResponse.StatusCode);

        var buscarResponse = await _client.GetAsync($"/api/diretores/{criado.Id}");
        Assert.Equal(HttpStatusCode.NotFound, buscarResponse.StatusCode);
    }
}
