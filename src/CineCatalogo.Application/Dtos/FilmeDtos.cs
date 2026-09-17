using System.ComponentModel.DataAnnotations;

namespace CineCatalogo.Application.Dtos;

public record FilmeDto(
    int Id,
    string Titulo,
    string Genero,
    int AnoLancamento,
    int DuracaoMinutos,
    string Sinopse,
    decimal NotaMedia,
    int DiretorId,
    string DiretorNome);

public class CreateFilmeDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O gênero é obrigatório.")]
    [MaxLength(60)]
    public string Genero { get; set; } = string.Empty;

    [Range(1888, 2100, ErrorMessage = "Ano de lançamento inválido.")]
    public int AnoLancamento { get; set; }

    [Range(1, 1000, ErrorMessage = "Duração deve ser maior que zero.")]
    public int DuracaoMinutos { get; set; }

    [MaxLength(2000)]
    public string Sinopse { get; set; } = string.Empty;

    [Range(0, 10, ErrorMessage = "Nota média deve estar entre 0 e 10.")]
    public decimal NotaMedia { get; set; }

    [Required(ErrorMessage = "O diretor é obrigatório.")]
    public int DiretorId { get; set; }
}

public class UpdateFilmeDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O gênero é obrigatório.")]
    [MaxLength(60)]
    public string Genero { get; set; } = string.Empty;

    [Range(1888, 2100, ErrorMessage = "Ano de lançamento inválido.")]
    public int AnoLancamento { get; set; }

    [Range(1, 1000, ErrorMessage = "Duração deve ser maior que zero.")]
    public int DuracaoMinutos { get; set; }

    [MaxLength(2000)]
    public string Sinopse { get; set; } = string.Empty;

    [Range(0, 10, ErrorMessage = "Nota média deve estar entre 0 e 10.")]
    public decimal NotaMedia { get; set; }

    [Required(ErrorMessage = "O diretor é obrigatório.")]
    public int DiretorId { get; set; }
}
