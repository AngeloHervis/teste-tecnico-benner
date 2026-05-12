using Microondas.Domain.Constants;
using Microondas.Domain.Entities;
using Microondas.Domain.Enums;
using Microondas.Domain.Exceptions;

namespace Microondas.Console;

public class MicroondasVisor(MaquinaMicroondas maquina)
{
    public void ExibirVisorCentral()
    {
        System.Console.WriteLine("=== MICRO-ONDAS DIGITAL ===");
        System.Console.WriteLine($"Status: {maquina.Estado}");
        
        if (maquina.Estado is EstadoMicroondas.EmAndamento or EstadoMicroondas.Pausado)
        {
            var p = maquina.ProgramaAtual;
            if (p != null)
            {
                System.Console.WriteLine($"Programa: {p.Nome} | {(p is { EhPadrao: false } ? "Customizado" : "Padrão")}");
                System.Console.WriteLine($"Instruções: {(string.IsNullOrWhiteSpace(p.Instrucoes) ? "Nenhuma" : p.Instrucoes)}");
            }
            System.Console.WriteLine($"Tempo Restante: {maquina.ObterTempoFormatado()}");
            System.Console.WriteLine($"Potência: {maquina.Potencia}");
        }

        if (string.IsNullOrEmpty(maquina.VisorAquecimento)) 
            return;
        
        System.Console.WriteLine("---------------------------");
        System.Console.WriteLine(maquina.VisorAquecimento);
        System.Console.WriteLine("---------------------------");
    }

    public void TratarLoopEmAndamento()
    {
        var permiteAcrescimo = maquina.ProgramaAtual == null || !maquina.ProgramaAtual.EhPadrao;

        System.Console.WriteLine(permiteAcrescimo
            ? "\n[Aquecimento em andamento... Pressione '2' para +30s ou '3' para Pausar/Cancelar]"
            : "\n[Aquecimento em andamento... Pressione '3' para Pausar/Cancelar]");

        Thread.Sleep(1000);
        
        while (System.Console.KeyAvailable)
        {
            var key = System.Console.ReadKey(intercept: true).Key;
            if (key is ConsoleKey.D3 or ConsoleKey.NumPad3)
                maquina.PausarOuCancelar();

            if (!permiteAcrescimo || key is not (ConsoleKey.D2 or ConsoleKey.NumPad2))
                continue;
            
            try
            {
                maquina.AdicionarTempo(ValoresPadrao.AcrescimoTempoSegundos);
            }
            catch (ValidacaoMicroondasException ex)
            {
                ExibirErroValidacao(ex.Message);
            }
        }
        
        maquina.AvancarSegundo();
    }

    public static void ExibirErroValidacao(string mensagem)
    {
        System.Console.WriteLine($"\n[ERRO DE VALIDAÇÃO]: {mensagem}");
        System.Console.WriteLine("Pressione qualquer tecla para continuar...");
        System.Console.ReadKey();
    }

    public static void ExibirErroGenerico(string mensagem)
    {
        System.Console.WriteLine($"\n[ERRO]: {mensagem}");
        System.Console.WriteLine("Pressione qualquer tecla para continuar...");
        System.Console.ReadKey();
    }
}
