using CineCatalogo.Application.Dtos;
using CineCatalogo.Application.Services;
using CineCatalogo.Domain.Common;
using CineCatalogo.Domain.Entities;
using CineCatalogo.Domain.Exceptions;
using CineCatalogo.Domain.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CineCatalogo.Tests.Unit.Services;

public class FilmeServiceTests
{
    private readonly Mock<IFilmeRepository> _filmeRepositoryMock = new();
    private readonly Mock<IDiretorRepository> _diretorRepositoryMock = new();
    private readonly FilmeService _service;

    public FilmeServiceTests()
    {
        _service = new FilmeService(_filmeRepositoryMock.Object, _diretorRepositoryMock.Object, NullLogger<FilmeService>.Instance);
    }

    [Fact]
    public async Task GetByIdAsync_QuandoFilmeNaoExiste_LancaNotFoundException()
    {
        _filmeRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Filme?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(1));
    }

    [Fact]
    public async Task CreateAsync_QuandoDiretorNaoExiste_LancaNotFoundException()
    {
        var dto = new CreateFilmeDto { Titulo = "Duna", Genero = "Ficção Científica", AnoLancamento = 2021, DuracaoMinutos = 155, DiretorId = 99 };
        _diretorRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Diretor?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(dto));

        _filmeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Filme>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_QuandoDiretorExiste_CriaFilmeComSucesso()
    {
        var diretor = new Diretor { Id = 1, Nome = "Denis Villeneuve" };
        var dto = new CreateFilmeDto
        {
            Titulo = "Duna",
            Genero = "Ficção Científica",
            AnoLancamento = 2021,
            DuracaoMinutos = 155,
            Sinopse = "Um jovem herdeiro parte em uma jornada em um planeta desértico.",
            NotaMedia = 8.5m,
            DiretorId = 1
        };
        _diretorRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(diretor);

        var resultado = await _service.CreateAsync(dto);

        Assert.Equal(dto.Titulo, resultado.Titulo);
        Assert.Equal(diretor.Nome, resultado.DiretorNome);
        _filmeRepositoryMock.Verify(r => r.AddAsync(It.Is<Filme>(f => f.Titulo == dto.Titulo && f.DiretorId == 1)), Times.Once);
        _filmeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_QuandoFilmeNaoExiste_LancaNotFoundException()
    {
        _filmeRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Filme?)null);
        var dto = new UpdateFilmeDto { Titulo = "X", Genero = "Y", DiretorId = 1 };

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(1, dto));
    }

    [Fact]
    public async Task GetAllAsync_RetornaResultadoPaginadoMapeado()
    {
        var diretor = new Diretor { Id = 1, Nome = "Denis Villeneuve" };
        var filmes = new List<Filme>
        {
            new() { Id = 1, Titulo = "Duna", Genero = "Ficção Científica", DiretorId = 1, Diretor = diretor },
            new() { Id = 2, Titulo = "Duna: Parte 2", Genero = "Ficção Científica", DiretorId = 1, Diretor = diretor }
        };
        var pagedResult = PagedResult<Filme>.Create(filmes, totalCount: 2, pageNumber: 1, pageSize: 10);
        _filmeRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<PaginationParams>(), null, null))
            .ReturnsAsync(pagedResult);

        var resultado = await _service.GetAllAsync(new PaginationParams(), null, null);

        Assert.Equal(2, resultado.TotalCount);
        Assert.All(resultado.Items, f => Assert.Equal("Denis Villeneuve", f.DiretorNome));
    }

    [Fact]
    public async Task DeleteAsync_QuandoFilmeExiste_RemoveComSucesso()
    {
        var filme = new Filme { Id = 5, Titulo = "Duna" };
        _filmeRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(filme);

        await _service.DeleteAsync(5);

        _filmeRepositoryMock.Verify(r => r.Remove(filme), Times.Once);
        _filmeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
