using Microondas.Domain.Commands;
using Microondas.Domain.Dtos;
using Microondas.Domain._Base;

namespace Microondas.Domain.Interfaces;

public interface IAquecimentoService
{
    Task<ServiceResult<string>> IniciarAquecimentoAsync(ConfigurarMicroondasCommand comando);
    Task<ServiceResult<string>> IniciarRapidoAsync();
    Task<ServiceResult<string>> IniciarProgramaAsync(int programaId);
    Task<ServiceResult<string>> AdicionarTempoAsync(int segundos = -1);
    Task<ServiceResult<string>> PausarOuCancelarAsync();
    IAsyncEnumerable<MicroondasStatusDto> ObterStatusStreamAsync(CancellationToken ct);
}

