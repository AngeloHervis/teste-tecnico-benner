using System.Runtime.CompilerServices;
using Microondas.Domain._Base;
using Microondas.Domain.Commands;
using Microondas.Domain.Constants;
using Microondas.Domain.Dtos;
using Microondas.Domain.Interfaces;

namespace Microondas.Domain.Services;

public class AquecimentoService(IProgramaService programaService, IMicroondasStateProvider stateProvider, IUserContext userContext) : IAquecimentoService
{
    public Task<ServiceResult<string>> IniciarAquecimentoAsync(ConfigurarMicroondasCommand comando)
    {
        var maquina = stateProvider.ObterMaquinaDoUsuario(userContext.ObterUsuarioAtual());
        maquina.Iniciar(comando);
        return Task.FromResult(ServiceResult<string>.Success("Aquecimento iniciado."));
    }

    public Task<ServiceResult<string>> IniciarRapidoAsync()
    {
        var maquina = stateProvider.ObterMaquinaDoUsuario(userContext.ObterUsuarioAtual());
        maquina.Iniciar();
        return Task.FromResult(ServiceResult<string>.Success("Aquecimento iniciado/acrescentado."));
    }

    public async Task<ServiceResult<string>> IniciarProgramaAsync(int programaId)
    {
        var programasResult = await programaService.ListarAsync();
        var programa = programasResult.Data?.FirstOrDefault(p => p.Id == programaId);

        if (programa == null)
            return ServiceResult<string>.NotFound("Programa não encontrado.");

        var maquina = stateProvider.ObterMaquinaDoUsuario(userContext.ObterUsuarioAtual());
        maquina.ConfigurarPrograma(programa);
        maquina.Iniciar();

        return ServiceResult<string>.Success("Aquecimento por programa iniciado.");
    }

    public Task<ServiceResult<string>> AdicionarTempoAsync(int segundos = -1)
    {
        var segundosEfetivos = segundos == -1 ? ValoresPadrao.AcrescimoTempoSegundos : segundos;
        var maquina = stateProvider.ObterMaquinaDoUsuario(userContext.ObterUsuarioAtual());
        maquina.AdicionarTempo(segundosEfetivos);
        return Task.FromResult(ServiceResult<string>.Success("Tempo adicionado."));
    }

    public Task<ServiceResult<string>> PausarOuCancelarAsync()
    {
        var maquina = stateProvider.ObterMaquinaDoUsuario(userContext.ObterUsuarioAtual());
        maquina.PausarOuCancelar();
        return Task.FromResult(ServiceResult<string>.Success("Comando de pausa/cancelamento executado."));
    }

    public async IAsyncEnumerable<MicroondasStatusDto> ObterStatusStreamAsync([EnumeratorCancellation] CancellationToken ct)
    {
        var maquina = stateProvider.ObterMaquinaDoUsuario(userContext.ObterUsuarioAtual());

        while (!ct.IsCancellationRequested)
        {
            yield return new MicroondasStatusDto(
                maquina.TempoRestanteSegundos,
                maquina.ObterTempoFormatado(),
                maquina.VisorAquecimento,
                maquina.Estado.ToString()
            );

            await Task.Delay(ValoresPadrao.IntervaloStreamStatusMs, ct);
        }
    }
}






