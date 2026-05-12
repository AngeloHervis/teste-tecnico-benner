using Microondas.Domain.Entities;
using Microondas.Domain.Enums;
using Microondas.Domain.Exceptions;
using Microondas.Domain.Interfaces;

namespace Microondas.Console;

public class MicroondasInterface
{
    private readonly MaquinaMicroondas _maquina = new();
    private readonly MicroondasMenu _menu;
    private readonly MicroondasVisor _visor;
    private bool _rodando = true;

    public MicroondasInterface(IProgramaService programaService)
    {
        _menu = new MicroondasMenu(_maquina, programaService);
        _visor = new MicroondasVisor(_maquina);
    }

    public async Task IniciarLoopAsync()
    {
        while (_rodando)
        {
            System.Console.Clear();
            _visor.ExibirVisorCentral();

            if (_maquina.Estado == EstadoMicroondas.EmAndamento)
            {
                _visor.TratarLoopEmAndamento();
                continue;
            }

            MicroondasMenu.ExibirMenuPrincipal();
            var opcao = System.Console.ReadLine();

            try
            {
                _rodando = await _menu.ProcessarOpcaoAsync(opcao);
            }
            catch (ValidacaoMicroondasException ex)
            {
                MicroondasVisor.ExibirErroValidacao(ex.Message);
            }
            catch (Exception ex)
            {
                MicroondasVisor.ExibirErroGenerico(ex.Message);
            }
        }
    }
}
