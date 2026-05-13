using Microondas.Domain.Enums;
using Microondas.Domain.Constants;
using Microondas.Domain.Interfaces;

namespace Microondas.Api.BackgroundServices;

public class MicroondasWorker(IMicroondasStateProvider stateProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var maquina in stateProvider.ObterTodasAsMaquinas())
            {
                if (maquina.Estado == EstadoMicroondas.EmAndamento)
                    maquina.AvancarSegundo();
            }

            await Task.Delay(ValoresPadrao.UmSegundoEmMilissegundos, stoppingToken);
        }
    }
}


