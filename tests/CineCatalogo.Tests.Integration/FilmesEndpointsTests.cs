using System.Net;
using System.Net.Http.Json;
using CineCatalogo.Application.Dtos;
using Xunit;

namespace CineCatalogo.Tests.Integration;

public class FilmesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FilmesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<DiretorDto> CriarDiretorAsync()
    {
        var dto = new CreateDiretorDto
        {
            Nome = $"Diretor {Guid.NewGuid()}",
            Nacionalidade = "Brasileira",
            DataNascimento = new DateTime(1980, 1, 1)
        };
        var response = await _client.PostAsJsonAsync("/api/diretores", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<DiretorDto>())!;
    }

    [Fact]
    public async Task Create_ComDiretorInexistente_DeveRetornar404()
    {
        var dto = new CreateFilmeDto
        {
            Titulo = "Filme Sem Diretor",
            Genero = "Drama",
            AnoLancamento = 2020,
            DuracaoMinutos = 100,
            DiretorId = 999999
        };

        var response = await _client.PostAsJsonAsync("/api/filmes", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ComDadosValidos_DeveRetornar201()
    {
        var diretor = await CriarDiretorAsync();
        var dto = new CreateFilmeDto
        {
            Titulo = $"Filme {Guid.NewGuid()}",
            Genero = "Ficção Científica",
            AnoLancamento = 2021,
            DuracaoMinutos = 140,
            Sinopse = "Sinopse de teste.",
            NotaMedia = 8.0m,
            DiretorId = diretor.Id
        };

        var response = await _client.PostAsJsonAsync("/api/filmes", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var criado = await response.Content.ReadFromJsonAsync<FilmeDto>();
        Assert.Equal(dto.Titulo, criado!.Titulo);
        Assert.Equal(diretor.Nome, criado.DiretorNome);
    }

    [Fact]
    public async Task GetAll_FiltradoPorGeneroAcentuado_DeveRetornarApenasCorrespondentes()
    {
        var diretor = await CriarDiretorAsync();
        var generoUnico = $"Suspense Psicológico {Guid.NewGuid():N}";

        var dto = new CreateFilmeDto
        {
            Titulo = $"Filme Suspense {Guid.NewGuid()}",
            Genero = generoUnico,
            AnoLancamento = 2019,
            DuracaoMinutos = 110,
            DiretorId = diretor.Id
        };
        await (await _client.PostAsJsonAsync("/api/filmes", dto)).Content.ReadAsStringAsync();

        var response = await _client.GetAsync($"/api/filmes?genero={Uri.EscapeDataString(generoUnico)}&pageSize=50");
        response.EnsureSuccessStatusCode();

        var resultado = await response.Content.ReadFromJsonAsync<PagedResultDto<FilmeDto>>();

        Assert.NotNull(resultado);
        Assert.All(resultado!.Items, f => Assert.Equal(generoUnico, f.Genero, ignoreCase: true));
        Assert.Contains(resultado.Items, f => f.Titulo == dto.Titulo);
    }

    [Fact]
    public async Task GetAll_FiltradoPorDiretor_DeveRetornarApenasFilmesDesseDiretor()
    {
        var diretor = await CriarDiretorAsync();
        var dto = new CreateFilmeDto
        {
            Titulo = $"Filme Filtro Diretor {Guid.NewGuid()}",
            Genero = "Aventura",
            AnoLancamento = 2018,
            DuracaoMinutos = 120,
            DiretorId = diretor.Id
        };
        await _client.PostAsJsonAsync("/api/filmes", dto);

        var response = await _client.GetAsync($"/api/filmes?diretorId={diretor.Id}&pageSize=50");
        response.EnsureSuccessStatusCode();

        var resultado = await response.Content.ReadFromJsonAsync<PagedResultDto<FilmeDto>>();

        Assert.NotNull(resultado);
        Assert.NotEmpty(resultado!.Items);
        Assert.All(resultado.Items, f => Assert.Equal(diretor.Id, f.DiretorId));
    }

    [Fact]
    public async Task Update_ComDadosValidos_DeveAtualizarFilme()
    {
        var diretor = await CriarDiretorAsync();
        var criarDto = new CreateFilmeDto
        {
            Titulo = $"Filme Original {Guid.NewGuid()}",
            Genero = "Drama",
            AnoLancamento = 2015,
            DuracaoMinutos = 100,
            DiretorId = diretor.Id
        };
        var criarResponse = await _client.PostAsJsonAsync("/api/filmes", criarDto);
        var criado = await criarResponse.Content.ReadFromJsonAsync<FilmeDto>();

        var atualizarDto = new UpdateFilmeDto
        {
            Titulo = criado!.Titulo,
            Genero = "Suspense",
            AnoLancamento = 2016,
            DuracaoMinutos = 105,
            DiretorId = diretor.Id,
            NotaMedia = 7.5m
        };
        var atualizarResponse = await _client.PutAsJsonAsync($"/api/filmes/{criado.Id}", atualizarDto);

        Assert.Equal(HttpStatusCode.OK, atualizarResponse.StatusCode);
        var atualizado = await atualizarResponse.Content.ReadFromJsonAsync<FilmeDto>();
        Assert.Equal("Suspense", atualizado!.Genero);
    }

    [Fact]
    public async Task Delete_QuandoFilmeExiste_DeveRemover()
    {
        var diretor = await CriarDiretorAsync();
        var criarDto = new CreateFilmeDto
        {
            Titulo = $"Filme Para Remover {Guid.NewGuid()}",
            Genero = "Terror",
            AnoLancamento = 2012,
            DuracaoMinutos = 95,
            DiretorId = diretor.Id
        };
        var criarResponse = await _client.PostAsJsonAsync("/api/filmes", criarDto);
        var criado = await criarResponse.Content.ReadFromJsonAsync<FilmeDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/filmes/{criado!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/filmes/{criado.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
