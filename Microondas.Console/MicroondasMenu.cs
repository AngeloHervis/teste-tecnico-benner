using Microondas.Domain.Commands;
using Microondas.Domain.Entities;
using Microondas.Domain.Enums;
using Microondas.Domain.Exceptions;
using Microondas.Domain.Interfaces;

namespace Microondas.Console;

public class MicroondasMenu(MaquinaMicroondas maquina, IProgramaRepository programaRepository)
{
    public static void ExibirMenuPrincipal()
    {
        System.Console.WriteLine("\nMenu:");
        System.Console.WriteLine("1 - Iniciar / Retomar");
        System.Console.WriteLine("2 - Início Rápido (+30s)");
        System.Console.WriteLine("3 - Pausar / Cancelar / Limpar");
        System.Console.WriteLine("4 - Programas de Aquecimento");
        System.Console.WriteLine("5 - Sair");
        System.Console.Write("Escolha uma opção: ");
    }

    public bool ProcessarOpcao(string? opcao)
    {
        switch (opcao)
        {
            case "1":
                ProcessarAcaoIniciarOuRetomar();
                break;
            case "2":
                maquina.InicioRapido();
                break;
            case "3":
                maquina.PausarOuCancelar();
                break;
            case "4":
                ProcessarMenuProgramas();
                break;
            case "5":
                return false;
        }
        return true;
    }

    private void ProcessarMenuProgramas()
    {
        System.Console.WriteLine("\n=== Programas de Aquecimento ===");
        var programas = programaRepository.ObterTodos().ToList();
        for (var i = 0; i < programas.Count; i++)
        {
            var p = programas[i];
            System.Console.WriteLine($"{i + 1} - {p.Nome} (Alimento: {p.Alimento}, Tempo: {p.TempoSegundos}s, Potência: {p.Potencia})");
        }
        System.Console.WriteLine("0 - Voltar");
        System.Console.Write("Escolha um programa: ");
        
        var opcao = System.Console.ReadLine();
        if (opcao == "0") return;

        if (!int.TryParse(opcao, out var index) || index <= 0 || index > programas.Count)
            throw new ValidacaoMicroondasException("Opção de programa inválida.");
        
        var programa = programas[index - 1];
        maquina.ConfigurarPrograma(programa);
        maquina.Iniciar();
    }

    private void ProcessarAcaoIniciarOuRetomar()
    {
        if (maquina.Estado == EstadoMicroondas.Pausado)
        {
            maquina.Iniciar();
            return;
        }

        System.Console.Write("Tempo (segundos) [vazio ou 0 para cancelar]: ");
        var tempoInput = System.Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(tempoInput) || tempoInput == "0")
        {
            maquina.PausarOuCancelar();
            return;
        }
        
        var tempo = ConverterTempo(tempoInput);
        
        System.Console.Write("Potência (1 a 10) [vazio para 10, 0 para cancelar]: ");
        var potenciaInput = System.Console.ReadLine();
        
        if (potenciaInput == "0")
        {
            maquina.PausarOuCancelar();
            return;
        }
        
        var potencia = ConverterPotencia(potenciaInput);

        var command = new ConfigurarMicroondasCommand 
        { 
            TempoEmSegundos = tempo, 
            Potencia = potencia 
        };
        
        maquina.Configurar(command);
        maquina.Iniciar();
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

        return input.All(char.IsDigit) ? int.Parse(input) : 0;
    }
}
