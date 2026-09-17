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

public class DiretorServiceTests
{
    private readonly Mock<IDiretorRepository> _repositoryMock = new();
    private readonly DiretorService _service;

    public DiretorServiceTests()
    {
        _service = new DiretorService(_repositoryMock.Object, NullLogger<DiretorService>.Instance);
    }

    [Fact]
    public async Task GetByIdAsync_QuandoDiretorExiste_RetornaDto()
    {
        var diretor = new Diretor { Id = 1, Nome = "Christopher Nolan", Nacionalidade = "Britânico", DataNascimento = new DateTime(1970, 7, 30) };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(diretor);

        var resultado = await _service.GetByIdAsync(1);

        Assert.Equal(diretor.Id, resultado.Id);
        Assert.Equal(diretor.Nome, resultado.Nome);
    }

    [Fact]
    public async Task GetByIdAsync_QuandoDiretorNaoExiste_LancaNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Diretor?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
    }

    [Fact]
    public async Task CreateAsync_QuandoNomeJaExiste_LancaConflictException()
    {
        var dto = new CreateDiretorDto { Nome = "Christopher Nolan", Nacionalidade = "Britânico", DataNascimento = new DateTime(1970, 7, 30) };
        _repositoryMock.Setup(r => r.ExistsByNomeAsync(dto.Nome, null)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(dto));

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Diretor>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_QuandoNomeNaoExiste_CriaEPersisteDiretor()
    {
        var dto = new CreateDiretorDto { Nome = "Greta Gerwig", Nacionalidade = "Americana", DataNascimento = new DateTime(1983, 8, 4) };
        _repositoryMock.Setup(r => r.ExistsByNomeAsync(dto.Nome, null)).ReturnsAsync(false);

        var resultado = await _service.CreateAsync(dto);

        Assert.Equal(dto.Nome, resultado.Nome);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Diretor>(d => d.Nome == dto.Nome)), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_QuandoDiretorPossuiFilmes_LancaConflictException()
    {
        var diretor = new Diretor { Id = 1, Nome = "Christopher Nolan" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(diretor);
        _repositoryMock.Setup(r => r.PossuiFilmesAsync(1)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => _service.DeleteAsync(1));

        _repositoryMock.Verify(r => r.Remove(It.IsAny<Diretor>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_QuandoDiretorNaoPossuiFilmes_RemoveComSucesso()
    {
        var diretor = new Diretor { Id = 2, Nome = "Greta Gerwig" };
        _repositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(diretor);
        _repositoryMock.Setup(r => r.PossuiFilmesAsync(2)).ReturnsAsync(false);

        await _service.DeleteAsync(2);

        _repositoryMock.Verify(r => r.Remove(diretor), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_RetornaResultadoPaginadoMapeado()
    {
        var diretores = new List<Diretor>
        {
            new() { Id = 1, Nome = "A", Nacionalidade = "BR", DataNascimento = DateTime.UnixEpoch },
            new() { Id = 2, Nome = "B", Nacionalidade = "US", DataNascimento = DateTime.UnixEpoch }
        };
        var pagedResult = PagedResult<Diretor>.Create(diretores, totalCount: 2, pageNumber: 1, pageSize: 10);
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<PaginationParams>())).ReturnsAsync(pagedResult);

        var resultado = await _service.GetAllAsync(new PaginationParams());

        Assert.Equal(2, resultado.TotalCount);
        Assert.Equal(2, resultado.Items.Count);
    }
}
