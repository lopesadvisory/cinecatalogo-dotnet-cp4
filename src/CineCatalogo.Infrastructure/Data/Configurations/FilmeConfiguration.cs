using CineCatalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineCatalogo.Infrastructure.Data.Configurations;

public class FilmeConfiguration : IEntityTypeConfiguration<Filme>
{
    public void Configure(EntityTypeBuilder<Filme> builder)
    {
        builder.ToTable("Filmes");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Genero)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(f => f.Sinopse)
            .HasMaxLength(2000);

        builder.Property(f => f.NotaMedia)
            .HasColumnType("decimal(3,1)");

        builder.HasIndex(f => f.Titulo)
            .HasDatabaseName("IX_Filmes_Titulo");

        builder.HasIndex(f => f.Genero)
            .HasDatabaseName("IX_Filmes_Genero");

        builder.HasIndex(f => f.DiretorId)
            .HasDatabaseName("IX_Filmes_DiretorId");
    }
}
