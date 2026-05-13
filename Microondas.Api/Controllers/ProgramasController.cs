using Microondas.Domain.Dtos;
using Microondas.Domain.Interfaces;
using Microondas.Api.Extensions;
using Microondas.Domain.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microondas.Api.Controllers;

[Route("api/programas")]
[ApiController]
[Authorize]
public class ProgramasController(IProgramaService programaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar()
        => await programaService.ListarAsync().ToActionResultAsync();

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] ProgramaDto dto)
        => await programaService.CadastrarAsync(dto.ParaEntidade()).ToActionResultAsync();
}
