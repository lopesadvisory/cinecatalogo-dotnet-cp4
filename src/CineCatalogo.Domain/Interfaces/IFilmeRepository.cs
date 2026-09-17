using CineCatalogo.Domain.Common;
using CineCatalogo.Domain.Entities;

namespace CineCatalogo.Domain.Interfaces;

public interface IFilmeRepository
{
    Task<Filme?> GetByIdAsync(int id);
    Task<PagedResult<Filme>> GetAllAsync(PaginationParams pagination, string? genero, int? diretorId);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Filme filme);
    void Update(Filme filme);
    void Remove(Filme filme);
    Task<int> SaveChangesAsync();
}
