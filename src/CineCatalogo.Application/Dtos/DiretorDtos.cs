using System.ComponentModel.DataAnnotations;

namespace CineCatalogo.Application.Dtos;

public record DiretorDto(int Id, string Nome, string Nacionalidade, DateTime DataNascimento, int QuantidadeFilmes);

public class CreateDiretorDto
{
    [Required(ErrorMessage = "O nome do diretor é obrigatório.")]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nacionalidade é obrigatória.")]
    [MaxLength(80)]
    public string Nacionalidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    public DateTime DataNascimento { get; set; }
}

public class UpdateDiretorDto
{
    [Required(ErrorMessage = "O nome do diretor é obrigatório.")]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nacionalidade é obrigatória.")]
    [MaxLength(80)]
    public string Nacionalidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    public DateTime DataNascimento { get; set; }
}
