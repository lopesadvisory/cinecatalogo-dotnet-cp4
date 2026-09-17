using CineCatalogo.Application.Dtos;
using CineCatalogo.Application.Mappings;
using CineCatalogo.Domain.Common;
using CineCatalogo.Domain.Entities;
using Xunit;

namespace CineCatalogo.Tests.Unit.Mappings;

public class MappingTests
{
    [Fact]
    public void DiretorToDto_MapeiaTodosOsCampos()
    {
        var diretor = new Diretor
        {
            Id = 1,
            Nome = "Christopher Nolan",
            Nacionalidade = "Britânico",
            DataNascimento = new DateTime(1970, 7, 30),
            Filmes = new List<Filme> { new(), new() }
        };

        var dto = diretor.ToDto();

        Assert.Equal(diretor.Id, dto.Id);
        Assert.Equal(diretor.Nome, dto.Nome);
        Assert.Equal(diretor.Nacionalidade, dto.Nacionalidade);
        Assert.Equal(2, dto.QuantidadeFilmes);
    }

    [Fact]
    public void CreateDiretorDto_ToEntity_MapeiaCorretamente()
    {
        var dto = new CreateDiretorDto { Nome = "Greta Gerwig", Nacionalidade = "Americana", DataNascimento = new DateTime(1983, 8, 4) };

        var entity = dto.ToEntity();

        Assert.Equal(dto.Nome, entity.Nome);
        Assert.Equal(dto.Nacionalidade, entity.Nacionalidade);
        Assert.Equal(dto.DataNascimento, entity.DataNascimento);
    }

    [Fact]
    public void FilmeToDto_QuandoDiretorPresente_IncluiNomeDoDiretor()
    {
        var diretor = new Diretor { Id = 1, Nome = "Denis Villeneuve" };
        var filme = new Filme { Id = 1, Titulo = "Duna", Genero = "Ficção Científica", DiretorId = 1, Diretor = diretor, NotaMedia = 8.5m };

        var dto = filme.ToDto();

        Assert.Equal(filme.Titulo, dto.Titulo);
        Assert.Equal("Denis Villeneuve", dto.DiretorNome);
    }

    [Fact]
    public void UpdateFilmeDto_ApplyTo_AtualizaEntidadeExistente()
    {
        var filme = new Filme { Id = 1, Titulo = "Original", Genero = "Drama", DiretorId = 1 };
        var dto = new UpdateFilmeDto { Titulo = "Atualizado", Genero = "Comédia", AnoLancamento = 2024, DuracaoMinutos = 100, DiretorId = 2, NotaMedia = 9 };

        dto.ApplyTo(filme);

        Assert.Equal("Atualizado", filme.Titulo);
        Assert.Equal("Comédia", filme.Genero);
        Assert.Equal(2, filme.DiretorId);
    }

    [Fact]
    public void PagedResult_ToDto_MapeiaItensEMetadados()
    {
        var pagedResult = PagedResult<int>.Create(new List<int> { 1, 2, 3 }, totalCount: 10, pageNumber: 2, pageSize: 3);

        var dto = pagedResult.ToDto(i => i.ToString());

        Assert.Equal(new[] { "1", "2", "3" }, dto.Items);
        Assert.Equal(10, dto.TotalCount);
        Assert.Equal(4, dto.TotalPages);
        Assert.True(dto.HasPreviousPage);
        Assert.True(dto.HasNextPage);
    }
}
