using CineCatalogo.Domain.Entities;
using CineCatalogo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineCatalogo.Api;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Diretores.AnyAsync())
        {
            return;
        }

        var diretores = new List<Diretor>
        {
            new() { Nome = "Christopher Nolan", Nacionalidade = "Britânico", DataNascimento = new DateTime(1970, 7, 30) },
            new() { Nome = "Fernando Meirelles", Nacionalidade = "Brasileiro", DataNascimento = new DateTime(1955, 11, 9) },
            new() { Nome = "Greta Gerwig", Nacionalidade = "Americana", DataNascimento = new DateTime(1983, 8, 4) }
        };

        await context.Diretores.AddRangeAsync(diretores);
        await context.SaveChangesAsync();

        var filmes = new List<Filme>
        {
            new()
            {
                Titulo = "A Origem",
                Genero = "Ficção Científica",
                AnoLancamento = 2010,
                DuracaoMinutos = 148,
                Sinopse = "Um ladrão que invade sonhos recebe a missão de plantar uma ideia na mente de um executivo.",
                NotaMedia = 8.8m,
                DiretorId = diretores[0].Id
            },
            new()
            {
                Titulo = "Interestelar",
                Genero = "Ficção Científica",
                AnoLancamento = 2014,
                DuracaoMinutos = 169,
                Sinopse = "Um grupo de exploradores viaja através de um buraco de minhoca em busca de um novo lar para a humanidade.",
                NotaMedia = 8.9m,
                DiretorId = diretores[0].Id
            },
            new()
            {
                Titulo = "Cidade de Deus",
                Genero = "Drama",
                AnoLancamento = 2002,
                DuracaoMinutos = 130,
                Sinopse = "A história de duas crianças que crescem em meio à violência de uma favela carioca.",
                NotaMedia = 9.0m,
                DiretorId = diretores[1].Id
            },
            new()
            {
                Titulo = "Barbie",
                Genero = "Comédia",
                AnoLancamento = 2023,
                DuracaoMinutos = 114,
                Sinopse = "Barbie vive uma crise existencial e parte para o mundo real em busca de respostas.",
                NotaMedia = 7.2m,
                DiretorId = diretores[2].Id
            }
        };

        await context.Filmes.AddRangeAsync(filmes);
        await context.SaveChangesAsync();
    }
}
