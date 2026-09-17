using CineCatalogo.Application.Dtos;
using CineCatalogo.Application.Interfaces;
using CineCatalogo.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace CineCatalogo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("padrao")]
[Produces("application/json")]
public class FilmesController : ControllerBase
{
    private readonly IFilmeService _service;

    public FilmesController(IFilmeService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista filmes",
        Description = "Retorna os filmes cadastrados de forma paginada, com filtros opcionais por gênero e diretor.")]
    [SwaggerResponse(200, "Lista paginada de filmes.", typeof(PagedResultDto<FilmeDto>))]
    public async Task<ActionResult<PagedResultDto<FilmeDto>>> GetAll(
        [FromQuery] PaginationParams pagination,
        [FromQuery] string? genero,
        [FromQuery] int? diretorId)
    {
        var result = await _service.GetAllAsync(pagination, genero, diretorId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Busca um filme pelo id")]
    [SwaggerResponse(200, "Filme encontrado.", typeof(FilmeDto))]
    [SwaggerResponse(404, "Filme não encontrado.")]
    public async Task<ActionResult<FilmeDto>> GetById(int id)
    {
        var filme = await _service.GetByIdAsync(id);
        return Ok(filme);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastra um novo filme")]
    [SwaggerResponse(201, "Filme criado.", typeof(FilmeDto))]
    [SwaggerResponse(400, "Dados inválidos.")]
    [SwaggerResponse(404, "Diretor informado não existe.")]
    public async Task<ActionResult<FilmeDto>> Create([FromBody] CreateFilmeDto dto)
    {
        var filme = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = filme.Id }, filme);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza um filme existente")]
    [SwaggerResponse(200, "Filme atualizado.", typeof(FilmeDto))]
    [SwaggerResponse(404, "Filme ou diretor não encontrado.")]
    public async Task<ActionResult<FilmeDto>> Update(int id, [FromBody] UpdateFilmeDto dto)
    {
        var filme = await _service.UpdateAsync(id, dto);
        return Ok(filme);
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Remove um filme")]
    [SwaggerResponse(204, "Filme removido.")]
    [SwaggerResponse(404, "Filme não encontrado.")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
