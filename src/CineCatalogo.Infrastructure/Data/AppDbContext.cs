using CineCatalogo.Domain.Entities;
using CineCatalogo.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CineCatalogo.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Diretor> Diretores => Set<Diretor>();
    public DbSet<Filme> Filmes => Set<Filme>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DiretorConfiguration());
        modelBuilder.ApplyConfiguration(new FilmeConfiguration());
    }
}
