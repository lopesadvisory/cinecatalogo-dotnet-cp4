namespace CineCatalogo.Domain.Entities;

public class Diretor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Nacionalidade { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }

    public ICollection<Filme> Filmes { get; set; } = new List<Filme>();
}
