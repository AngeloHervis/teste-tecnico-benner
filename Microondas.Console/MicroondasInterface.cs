using Microondas.Domain.Entities;
using Microondas.Domain.Exceptions;
using Microondas.Domain.Interfaces;
using Microondas.Domain.Services;
using Microondas.Domain.Enums;

namespace Microondas.Console;

public class MicroondasInterface
{
    private readonly MaquinaMicroondas _maquina = new();
    private readonly IProgramaRepository _programaRepository = new ProgramaRepository();
    private readonly MicroondasMenu _menu;
    private readonly MicroondasVisor _visor;
    private bool _rodando = true;

    public MicroondasInterface()
    {
        _menu = new MicroondasMenu(_maquina, _programaRepository);
        _visor = new MicroondasVisor(_maquina);
    }

    public void IniciarLoop()
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
                _rodando = _menu.ProcessarOpcao(opcao);
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
