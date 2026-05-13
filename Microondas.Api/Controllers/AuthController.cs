using Microondas.Api.Extensions;
using Microondas.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microondas.Api.Controllers;

/// <summary>
/// Controller responsável pela autenticação de usuários.
/// </summary>
[Route("api/auth")]
[ApiController]
public class AuthController(IUsuarioService usuarioService) : ControllerBase
{
    public record LoginRequest(string Username, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
        => await usuarioService.RealizarLoginAsync(request.Username, request.Password).ToActionResultAsync();

    public record ConfigPasswordRequest(string NovaSenha);

    /// <summary>
    /// Altera a senha do usuário autenticado.
    /// </summary>
    [Authorize]
    [HttpPost("configurar")]
    public async Task<IActionResult> Configurar([FromBody] ConfigPasswordRequest request)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        return await usuarioService.AtualizarSenhaAsync(username, request.NovaSenha).ToActionResultAsync();
    }
}



