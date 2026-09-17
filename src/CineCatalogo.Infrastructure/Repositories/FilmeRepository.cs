using CineCatalogo.Domain.Common;
using CineCatalogo.Domain.Entities;
using CineCatalogo.Domain.Interfaces;
using CineCatalogo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineCatalogo.Infrastructure.Repositories;

public class FilmeRepository : IFilmeRepository
{
    private readonly AppDbContext _context;

    public FilmeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Filme?> GetByIdAsync(int id) =>
        await _context.Filmes
            .Include(f => f.Diretor)
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task<PagedResult<Filme>> GetAllAsync(PaginationParams pagination, string? genero, int? diretorId)
    {
        var query = _context.Filmes
            .Include(f => f.Diretor)
            .AsNoTracking()
            .AsQueryable();

        if (diretorId.HasValue)
        {
            query = query.Where(f => f.DiretorId == diretorId.Value);
        }

        query = query.OrderBy(f => f.Titulo);

        // O SQLite só faz case-folding nativo para ASCII (LOWER/UPPER não cobrem acentos),
        // então a comparação de gênero é feita em memória com StringComparison.OrdinalIgnoreCase
        // para funcionar corretamente com valores acentuados (ex.: "Comédia", "Ficção Científica").
        if (!string.IsNullOrWhiteSpace(genero))
        {
            var todos = await query.ToListAsync();
            var filtrados = todos
                .Where(f => string.Equals(f.Genero, genero, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var totalFiltrado = filtrados.Count;
            var itemsFiltrados = filtrados
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            return PagedResult<Filme>.Create(itemsFiltrados, totalFiltrado, pagination.PageNumber, pagination.PageSize);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return PagedResult<Filme>.Create(items, totalCount, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<bool> ExistsAsync(int id) => await _context.Filmes.AnyAsync(f => f.Id == id);

    public async Task AddAsync(Filme filme) => await _context.Filmes.AddAsync(filme);

    public void Update(Filme filme) => _context.Filmes.Update(filme);

    public void Remove(Filme filme) => _context.Filmes.Remove(filme);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
