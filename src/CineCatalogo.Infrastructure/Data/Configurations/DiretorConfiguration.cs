using CineCatalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineCatalogo.Infrastructure.Data.Configurations;

public class DiretorConfiguration : IEntityTypeConfiguration<Diretor>
{
    public void Configure(EntityTypeBuilder<Diretor> builder)
    {
        builder.ToTable("Diretores");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.Nacionalidade)
            .IsRequired()
            .HasMaxLength(80);

        builder.HasIndex(d => d.Nome)
            .HasDatabaseName("IX_Diretores_Nome");

        builder.HasMany(d => d.Filmes)
            .WithOne(f => f.Diretor)
            .HasForeignKey(f => f.DiretorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
