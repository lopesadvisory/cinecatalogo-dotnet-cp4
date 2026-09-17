using CineCatalogo.Application.Dtos;
using CineCatalogo.Domain.Entities;

namespace CineCatalogo.Application.Mappings;

public static class FilmeMappings
{
    public static FilmeDto ToDto(this Filme filme) =>
        new(
            filme.Id,
            filme.Titulo,
            filme.Genero,
            filme.AnoLancamento,
            filme.DuracaoMinutos,
            filme.Sinopse,
            filme.NotaMedia,
            filme.DiretorId,
            filme.Diretor?.Nome ?? string.Empty);

    public static Filme ToEntity(this CreateFilmeDto dto) =>
        new()
        {
            Titulo = dto.Titulo,
            Genero = dto.Genero,
            AnoLancamento = dto.AnoLancamento,
            DuracaoMinutos = dto.DuracaoMinutos,
            Sinopse = dto.Sinopse,
            NotaMedia = dto.NotaMedia,
            DiretorId = dto.DiretorId
        };

    public static void ApplyTo(this UpdateFilmeDto dto, Filme filme)
    {
        filme.Titulo = dto.Titulo;
        filme.Genero = dto.Genero;
        filme.AnoLancamento = dto.AnoLancamento;
        filme.DuracaoMinutos = dto.DuracaoMinutos;
        filme.Sinopse = dto.Sinopse;
        filme.NotaMedia = dto.NotaMedia;
        filme.DiretorId = dto.DiretorId;
    }
}
