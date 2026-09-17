using CineCatalogo.Domain.Common;
using CineCatalogo.Domain.Entities;
using CineCatalogo.Domain.Interfaces;
using CineCatalogo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineCatalogo.Infrastructure.Repositories;

public class DiretorRepository : IDiretorRepository
{
    private readonly AppDbContext _context;

    public DiretorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Diretor?> GetByIdAsync(int id) =>
        await _context.Diretores
            .Include(d => d.Filmes)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<PagedResult<Diretor>> GetAllAsync(PaginationParams pagination)
    {
        var query = _context.Diretores
            .Include(d => d.Filmes)
            .AsNoTracking()
            .OrderBy(d => d.Nome);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return PagedResult<Diretor>.Create(items, totalCount, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<bool> ExistsByNomeAsync(string nome, int? excludeId = null)
    {
        // Comparação feita em memória (ver FilmeRepository.GetAllAsync) porque o SQLite
        // só faz case-folding nativo para ASCII, o que quebraria nomes acentuados.
        var candidatos = await _context.Diretores
            .Where(d => excludeId == null || d.Id != excludeId)
            .Select(d => d.Nome)
            .ToListAsync();

        return candidatos.Any(n => string.Equals(n, nome, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> PossuiFilmesAsync(int diretorId) =>
        await _context.Filmes.AnyAsync(f => f.DiretorId == diretorId);

    public async Task AddAsync(Diretor diretor) => await _context.Diretores.AddAsync(diretor);

    public void Update(Diretor diretor) => _context.Diretores.Update(diretor);

    public void Remove(Diretor diretor) => _context.Diretores.Remove(diretor);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
