using CineCatalogo.Application.Dtos;
using CineCatalogo.Domain.Common;

namespace CineCatalogo.Application.Interfaces;

public interface IDiretorService
{
    Task<DiretorDto> GetByIdAsync(int id);
    Task<PagedResultDto<DiretorDto>> GetAllAsync(PaginationParams pagination);
    Task<DiretorDto> CreateAsync(CreateDiretorDto dto);
    Task<DiretorDto> UpdateAsync(int id, UpdateDiretorDto dto);
    Task DeleteAsync(int id);
}
