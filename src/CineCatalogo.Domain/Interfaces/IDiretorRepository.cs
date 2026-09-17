using CineCatalogo.Domain.Common;
using CineCatalogo.Domain.Entities;

namespace CineCatalogo.Domain.Interfaces;

public interface IDiretorRepository
{
    Task<Diretor?> GetByIdAsync(int id);
    Task<PagedResult<Diretor>> GetAllAsync(PaginationParams pagination);
    Task<bool> ExistsByNomeAsync(string nome, int? excludeId = null);
    Task AddAsync(Diretor diretor);
    void Update(Diretor diretor);
    void Remove(Diretor diretor);
    Task<bool> PossuiFilmesAsync(int diretorId);
    Task<int> SaveChangesAsync();
}
