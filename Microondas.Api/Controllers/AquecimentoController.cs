using System.Text.Json;
using Microondas.Api.Extensions;
using Microondas.Domain.Commands;
using Microondas.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microondas.Api.Controllers;

/// <summary>
/// Controller para gerenciar operações de aquecimento.
/// </summary>
[Route("api/aquecimento")]
[ApiController]
[Authorize]
public class AquecimentoController(IAquecimentoService aquecimentoService) : ControllerBase
{
    /// <summary>
    /// Inicia um aquecimento com tempo e potência especificados.
    /// </summary>
    [HttpPost("iniciar")]
    public async Task<IActionResult> Iniciar([FromBody] ConfigurarMicroondasCommand command)
    {
        return await aquecimentoService.IniciarAquecimentoAsync(command).ToActionResultAsync();
    }

    /// <summary>
    /// Inicia aquecimento rápido com padrões pré-definidos.
    /// </summary>
    [HttpPost("rapido")]
    public async Task<IActionResult> InicioRapido()
    {
        return await aquecimentoService.IniciarRapidoAsync().ToActionResultAsync();
    }

    /// <summary>
    /// Inicia aquecimento usando um programa pré-definido.
    /// </summary>
    [HttpPost("programa/{id:int}")]
    public async Task<IActionResult> IniciarPrograma(int id)
    {
        return await aquecimentoService.IniciarProgramaAsync(id).ToActionResultAsync();
    }

    /// <summary>
    /// Adiciona 30 segundos ao tempo de aquecimento em andamento.
    /// </summary>
    [HttpPost("adicionar-tempo")]
    public async Task<IActionResult> AdicionarTempo()
    {
        return await aquecimentoService.AdicionarTempoAsync().ToActionResultAsync();
    }

    /// <summary>
    /// Pausa ou cancela o aquecimento em andamento.
    /// </summary>
    [HttpPost("pausar-cancelar")]
    public async Task<IActionResult> PausarOuCancelar()
    {
        return await aquecimentoService.PausarOuCancelarAsync().ToActionResultAsync();
    }

    /// <summary>
    /// Stream de eventos do status atual do aquecimento.
    /// </summary>
    [HttpGet("stream")]
    public async Task GetStream(CancellationToken ct)
    {
        Response.ContentType = "text/event-stream";

        await foreach (var status in aquecimentoService.ObterStatusStreamAsync(ct))
        {
            var json = JsonSerializer.Serialize(status);
            await Response.WriteAsync($"data: {json}\n\n", ct);
            await Response.Body.FlushAsync(ct);
        }
    }
}



