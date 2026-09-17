using CineCatalogo.Application.Dtos;
using CineCatalogo.Application.Interfaces;
using CineCatalogo.Application.Mappings;
using CineCatalogo.Domain.Common;
using CineCatalogo.Domain.Exceptions;
using CineCatalogo.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CineCatalogo.Application.Services;

public class FilmeService : IFilmeService
{
    private readonly IFilmeRepository _filmeRepository;
    private readonly IDiretorRepository _diretorRepository;
    private readonly ILogger<FilmeService> _logger;

    public FilmeService(IFilmeRepository filmeRepository, IDiretorRepository diretorRepository, ILogger<FilmeService> logger)
    {
        _filmeRepository = filmeRepository;
        _diretorRepository = diretorRepository;
        _logger = logger;
    }

    public async Task<FilmeDto> GetByIdAsync(int id)
    {
        var filme = await _filmeRepository.GetByIdAsync(id)
            ?? throw NotFoundException.For("Filme", id);

        return filme.ToDto();
    }

    public async Task<PagedResultDto<FilmeDto>> GetAllAsync(PaginationParams pagination, string? genero, int? diretorId)
    {
        var result = await _filmeRepository.GetAllAsync(pagination, genero, diretorId);
        return result.ToDto(f => f.ToDto());
    }

    public async Task<FilmeDto> CreateAsync(CreateFilmeDto dto)
    {
        var diretor = await _diretorRepository.GetByIdAsync(dto.DiretorId)
            ?? throw NotFoundException.For("Diretor", dto.DiretorId);

        var filme = dto.ToEntity();
        await _filmeRepository.AddAsync(filme);
        await _filmeRepository.SaveChangesAsync();

        _logger.LogInformation("Filme {FilmeId} ({Titulo}) criado com sucesso para o diretor {DiretorId}.",
            filme.Id, filme.Titulo, diretor.Id);

        filme.Diretor = diretor;
        return filme.ToDto();
    }

    public async Task<FilmeDto> UpdateAsync(int id, UpdateFilmeDto dto)
    {
        var filme = await _filmeRepository.GetByIdAsync(id)
            ?? throw NotFoundException.For("Filme", id);

        var diretor = await _diretorRepository.GetByIdAsync(dto.DiretorId)
            ?? throw NotFoundException.For("Diretor", dto.DiretorId);

        dto.ApplyTo(filme);
        _filmeRepository.Update(filme);
        await _filmeRepository.SaveChangesAsync();

        _logger.LogInformation("Filme {FilmeId} atualizado com sucesso.", filme.Id);

        filme.Diretor = diretor;
        return filme.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        var filme = await _filmeRepository.GetByIdAsync(id)
            ?? throw NotFoundException.For("Filme", id);

        _filmeRepository.Remove(filme);
        await _filmeRepository.SaveChangesAsync();

        _logger.LogInformation("Filme {FilmeId} removido com sucesso.", id);
    }
}
