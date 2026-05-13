using Microondas.Domain.Commands;
using Microondas.Domain.Entities;
using Microondas.Domain.Enums;
using Microondas.Domain.Exceptions;
using Microondas.Domain.Interfaces;
using Microondas.Domain.Mappers;
using Microondas.Domain.Dtos;

namespace Microondas.Console;

public class MicroondasMenu(MaquinaMicroondas maquina, IProgramaService programaService)
{
    public static void ExibirMenuPrincipal()
    {
        System.Console.WriteLine("\nMenu:");
        System.Console.WriteLine("1 - Iniciar / Retomar");
        System.Console.WriteLine("2 - Início Rápido (+30s)");
        System.Console.WriteLine("3 - Pausar / Cancelar / Limpar");
        System.Console.WriteLine("4 - Programas de Aquecimento");
        System.Console.WriteLine("5 - Cadastrar Programa Customizado");
        System.Console.WriteLine("6 - Sair");
        System.Console.Write("Escolha uma opção: ");
    }

    public async Task<bool> ProcessarOpcaoAsync(string? opcao)
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
                await ProcessarMenuProgramasAsync();
                break;
            case "5":
                await ProcessarCadastroProgramaAsync();
                break;
            case "6":
                return false;
        }
        return true;
    }

    private async Task ProcessarMenuProgramasAsync()
    {
        System.Console.WriteLine("\n=== Programas de Aquecimento ===");
        var programasResult = await programaService.ListarAsync();
        var programas = programasResult.ToList();
        for (var i = 0; i < programas.Count; i++)
        {
            var p = programas[i];
            var prefix = p.EhPadrao ? "" : "\e[3m";
            var suffix = p.EhPadrao ? "" : "\e[23m";
            
            System.Console.WriteLine($"{i + 1} - {prefix}{p.Nome} (Alimento: {p.Alimento}, Tempo: {p.TempoSegundos}s, Potência: {p.Potencia}){suffix}");
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

    private async Task ProcessarCadastroProgramaAsync()
    {
        System.Console.WriteLine("\n=== Novo Programa Customizado ===");
        System.Console.WriteLine("(Deixe vazio ou digite '0' em qualquer campo para cancelar)");
        
        var nome = await LerNomeUnicoAsync("Nome: ");
        if (nome == null) return;

        var alimento = LerTextoObrigatorio("Alimento: ");
        if (alimento == null) return;

        var tempo = LerIntNoIntervalo("Tempo (segundos): ", 1, 3600);
        if (tempo == null) return;

        var potencia = LerIntNoIntervalo("Potência (1-10): ", 1, 10);
        if (potencia == null) return;

        var caractere = await LerCaractereUnicoAsync("Caractere de Aquecimento: ");
        if (caractere == null) return;
        
        System.Console.Write("Instruções (opcional): ");
        var instrucoes = System.Console.ReadLine() ?? "";

        try
        {
            var dto = new ProgramaDto(nome, alimento, tempo.Value, potencia.Value, caractere.Value, instrucoes, false);
            var novoPrograma = dto.ParaEntidade();
            
            await programaService.CadastrarAsync(novoPrograma);
            System.Console.WriteLine("\n[SUCESSO]: Programa cadastrado com sucesso!");
        }
        catch (ValidacaoMicroondasException ex)
        {
            MicroondasVisor.ExibirErroValidacao(ex.Message);
            return;
        }

        System.Console.WriteLine("Pressione qualquer tecla para continuar...");
        System.Console.ReadKey();
    }

    private async Task<string?> LerNomeUnicoAsync(string prompt)
    {
        while (true)
        {
            System.Console.Write(prompt);
            var input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input == "0")
                return null;

            if (await programaService.ExisteNomeAsync(input))
            {
                System.Console.WriteLine($"[ERRO]: Já existe um programa cadastrado com o nome '{input}'.");
                continue;
            }

            return input;
        }
    }

    private static string? LerTextoObrigatorio(string prompt)
    {
        System.Console.Write(prompt);
        var input = System.Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(input) || input == "0") 
            return null;
        
        return input;
    }

    private static int? LerIntNoIntervalo(string prompt, int min, int max)
    {
        while (true)
        {
            System.Console.Write(prompt);
            var input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input == "0")
                return null;
            
            if (int.TryParse(input, out var result) && result >= min && result <= max)
                return result;
            
            System.Console.WriteLine($"[ERRO]: Por favor, insira um número entre {min} e {max}.");
        }
    }

    private async Task<char?> LerCaractereUnicoAsync(string prompt)
    {
        while (true)
        {
            System.Console.Write(prompt);
            var input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input == "0")
                return null;

            if (input.Length == 1)
            {
                var c = input[0];
                if (c == '.')
                {
                    System.Console.WriteLine("[ERRO]: O caractere '.' é reservado para o aquecimento padrão.");
                    continue;
                }

                if (await programaService.ExisteCaractereAsync(c))
                {
                    System.Console.WriteLine($"[ERRO]: O caractere '{c}' já está sendo usado por outro programa.");
                    continue;
                }
                
                return c;
            }
            System.Console.WriteLine("[ERRO]: Insira exatamente um caractere.");
        }
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
