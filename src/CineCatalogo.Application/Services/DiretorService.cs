using CineCatalogo.Application.Dtos;
using CineCatalogo.Application.Interfaces;
using CineCatalogo.Application.Mappings;
using CineCatalogo.Domain.Common;
using CineCatalogo.Domain.Exceptions;
using CineCatalogo.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CineCatalogo.Application.Services;

public class DiretorService : IDiretorService
{
    private readonly IDiretorRepository _repository;
    private readonly ILogger<DiretorService> _logger;

    public DiretorService(IDiretorRepository repository, ILogger<DiretorService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<DiretorDto> GetByIdAsync(int id)
    {
        var diretor = await _repository.GetByIdAsync(id)
            ?? throw NotFoundException.For("Diretor", id);

        return diretor.ToDto();
    }

    public async Task<PagedResultDto<DiretorDto>> GetAllAsync(PaginationParams pagination)
    {
        var result = await _repository.GetAllAsync(pagination);
        return result.ToDto(d => d.ToDto());
    }

    public async Task<DiretorDto> CreateAsync(CreateDiretorDto dto)
    {
        if (await _repository.ExistsByNomeAsync(dto.Nome))
        {
            throw new ConflictException($"Já existe um diretor cadastrado com o nome '{dto.Nome}'.");
        }

        var diretor = dto.ToEntity();
        await _repository.AddAsync(diretor);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Diretor {DiretorId} ({Nome}) criado com sucesso.", diretor.Id, diretor.Nome);

        return diretor.ToDto();
    }

    public async Task<DiretorDto> UpdateAsync(int id, UpdateDiretorDto dto)
    {
        var diretor = await _repository.GetByIdAsync(id)
            ?? throw NotFoundException.For("Diretor", id);

        if (await _repository.ExistsByNomeAsync(dto.Nome, excludeId: id))
        {
            throw new ConflictException($"Já existe um diretor cadastrado com o nome '{dto.Nome}'.");
        }

        dto.ApplyTo(diretor);
        _repository.Update(diretor);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Diretor {DiretorId} atualizado com sucesso.", diretor.Id);

        return diretor.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        var diretor = await _repository.GetByIdAsync(id)
            ?? throw NotFoundException.For("Diretor", id);

        if (await _repository.PossuiFilmesAsync(id))
        {
            throw new ConflictException("Não é possível remover um diretor que possui filmes cadastrados.");
        }

        _repository.Remove(diretor);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Diretor {DiretorId} removido com sucesso.", id);
    }
}
