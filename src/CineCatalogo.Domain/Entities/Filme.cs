namespace CineCatalogo.Domain.Entities;

public class Filme
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int AnoLancamento { get; set; }
    public int DuracaoMinutos { get; set; }
    public string Sinopse { get; set; } = string.Empty;
    public decimal NotaMedia { get; set; }

    public int DiretorId { get; set; }
    public Diretor? Diretor { get; set; }
}
