using Microondas.Domain.Commands;
using Microondas.Domain.Constants;
using Microondas.Domain.Entities;
using Microondas.Domain.Enums;
using Microondas.Domain.Exceptions;

namespace Microondas.Console;

public class MicroondasInterface
{
    private readonly MaquinaMicroondas _maquina = new();
    private bool _rodando = true;

    public void IniciarLoop()
    {
        while (_rodando)
        {
            System.Console.Clear();
            ExibirVisorCentral();

            if (_maquina.Estado == EstadoMicroondas.EmAndamento)
            {
                TratarLoopEmAndamento();
                continue;
            }

            ExibirMenuPrincipal();
            var opcao = System.Console.ReadLine();

            try
            {
                ProcessarOpcaoMenu(opcao);
            }
            catch (ValidacaoMicroondasException ex)
            {
                ExibirErroValidacao(ex.Message);
            }
            catch (Exception ex)
            {
                ExibirErroGenerico(ex.Message);
            }
        }
    }

    private void ExibirVisorCentral()
    {
        System.Console.WriteLine("=== MICRO-ONDAS DIGITAL ===");
        System.Console.WriteLine($"Status: {_maquina.Estado}");
        
        if (_maquina.Estado == EstadoMicroondas.EmAndamento || _maquina.Estado == EstadoMicroondas.Pausado)
        {
            System.Console.WriteLine($"Tempo Restante: {_maquina.ObterTempoFormatado()}");
            System.Console.WriteLine($"Potência: {_maquina.Potencia}");
        }
        
        if (!string.IsNullOrEmpty(_maquina.VisorAquecimento))
        {
            System.Console.WriteLine("---------------------------");
            System.Console.WriteLine(_maquina.VisorAquecimento);
            System.Console.WriteLine("---------------------------");
        }
    }

    private void TratarLoopEmAndamento()
    {
        System.Console.WriteLine("\n[Aquecimento em andamento... Pressione 'I' (Início) para +30s ou 'P' (Pausar/Cancelar) para Pausar]");
        
        Thread.Sleep(1000);
        
        if (System.Console.KeyAvailable)
        {
            var key = System.Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.P)
                _maquina.PausarOuCancelar();
                
            if (key == ConsoleKey.I)
                _maquina.AdicionarTempo(ValoresPadrao.AcrescimoTempoSegundos);
        }
        
        _maquina.AvancarSegundo();
    }

    private static void ExibirMenuPrincipal()
    {
        System.Console.WriteLine("\nMenu:");
        System.Console.WriteLine("1 - Iniciar / Retomar");
        System.Console.WriteLine("2 - Início Rápido (+30s)");
        System.Console.WriteLine("3 - Pausar / Cancelar / Limpar");
        System.Console.WriteLine("4 - Sair");
        System.Console.Write("Escolha uma opção: ");
    }

    private void ProcessarOpcaoMenu(string? opcao)
    {
        switch (opcao)
        {
            case "1":
                ProcessarAcaoIniciarOuRetomar();
                break;
            case "2":
                _maquina.InicioRapido();
                break;
            case "3":
                _maquina.PausarOuCancelar();
                break;
            case "4":
                _rodando = false;
                break;
        }
    }

    private void ProcessarAcaoIniciarOuRetomar()
    {
        if (_maquina.Estado == EstadoMicroondas.Pausado)
        {
            _maquina.Iniciar();
            return;
        }

        System.Console.Write("Tempo (segundos): ");
        var tempoInput = System.Console.ReadLine();
        var tempo = ConverterTempo(tempoInput);
        
        System.Console.Write("Potência (1 a 10) [vazio para 10]: ");
        var potenciaInput = System.Console.ReadLine();
        var potencia = ConverterPotencia(potenciaInput);

        var command = new ConfigurarMicroondasCommand 
        { 
            TempoEmSegundos = tempo, 
            Potencia = potencia 
        };
        
        _maquina.Configurar(command);
        _maquina.Iniciar();
    }

    private static void ExibirErroValidacao(string mensagem)
    {
        System.Console.WriteLine($"\n[ERRO DE VALIDAÇÃO]: {mensagem}");
        System.Console.WriteLine("Pressione qualquer tecla para continuar...");
        System.Console.ReadKey();
    }

    private static void ExibirErroGenerico(string mensagem)
    {
        System.Console.WriteLine($"\n[ERRO]: {mensagem}");
        System.Console.WriteLine("Pressione qualquer tecla para continuar...");
        System.Console.ReadKey();
    }

    private static int? ConverterPotencia(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        if (input.All(char.IsDigit))
            return int.Parse(input);

        return null;
    }

    private static int ConverterTempo(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return 0;

        if (input.All(char.IsDigit))
            return int.Parse(input);

        return 0;
    }
}
