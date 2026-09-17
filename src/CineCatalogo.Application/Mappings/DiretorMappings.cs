using CineCatalogo.Application.Dtos;
using CineCatalogo.Domain.Entities;

namespace CineCatalogo.Application.Mappings;

public static class DiretorMappings
{
    public static DiretorDto ToDto(this Diretor diretor) =>
        new(diretor.Id, diretor.Nome, diretor.Nacionalidade, diretor.DataNascimento, diretor.Filmes.Count);

    public static Diretor ToEntity(this CreateDiretorDto dto) =>
        new()
        {
            Nome = dto.Nome,
            Nacionalidade = dto.Nacionalidade,
            DataNascimento = dto.DataNascimento
        };

    public static void ApplyTo(this UpdateDiretorDto dto, Diretor diretor)
    {
        diretor.Nome = dto.Nome;
        diretor.Nacionalidade = dto.Nacionalidade;
        diretor.DataNascimento = dto.DataNascimento;
    }
}
