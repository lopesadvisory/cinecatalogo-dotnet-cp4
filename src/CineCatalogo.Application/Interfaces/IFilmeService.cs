using CineCatalogo.Application.Dtos;
using CineCatalogo.Domain.Common;

namespace CineCatalogo.Application.Interfaces;

public interface IFilmeService
{
    Task<FilmeDto> GetByIdAsync(int id);
    Task<PagedResultDto<FilmeDto>> GetAllAsync(PaginationParams pagination, string? genero, int? diretorId);
    Task<FilmeDto> CreateAsync(CreateFilmeDto dto);
    Task<FilmeDto> UpdateAsync(int id, UpdateFilmeDto dto);
    Task DeleteAsync(int id);
}
