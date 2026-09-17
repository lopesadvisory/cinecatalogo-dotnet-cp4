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
public class DiretoresController : ControllerBase
{
    private readonly IDiretorService _service;

    public DiretoresController(IDiretorService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Lista diretores", Description = "Retorna os diretores cadastrados de forma paginada.")]
    [SwaggerResponse(200, "Lista paginada de diretores.", typeof(PagedResultDto<DiretorDto>))]
    public async Task<ActionResult<PagedResultDto<DiretorDto>>> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _service.GetAllAsync(pagination);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Busca um diretor pelo id")]
    [SwaggerResponse(200, "Diretor encontrado.", typeof(DiretorDto))]
    [SwaggerResponse(404, "Diretor não encontrado.")]
    public async Task<ActionResult<DiretorDto>> GetById(int id)
    {
        var diretor = await _service.GetByIdAsync(id);
        return Ok(diretor);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastra um novo diretor")]
    [SwaggerResponse(201, "Diretor criado.", typeof(DiretorDto))]
    [SwaggerResponse(400, "Dados inválidos.")]
    [SwaggerResponse(409, "Já existe um diretor com o mesmo nome.")]
    public async Task<ActionResult<DiretorDto>> Create([FromBody] CreateDiretorDto dto)
    {
        var diretor = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = diretor.Id }, diretor);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza um diretor existente")]
    [SwaggerResponse(200, "Diretor atualizado.", typeof(DiretorDto))]
    [SwaggerResponse(404, "Diretor não encontrado.")]
    [SwaggerResponse(409, "Já existe um diretor com o mesmo nome.")]
    public async Task<ActionResult<DiretorDto>> Update(int id, [FromBody] UpdateDiretorDto dto)
    {
        var diretor = await _service.UpdateAsync(id, dto);
        return Ok(diretor);
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Remove um diretor")]
    [SwaggerResponse(204, "Diretor removido.")]
    [SwaggerResponse(404, "Diretor não encontrado.")]
    [SwaggerResponse(409, "Diretor possui filmes vinculados.")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
