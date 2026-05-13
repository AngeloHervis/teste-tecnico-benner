using Microondas.Domain.Commands;
using Microondas.Domain.Constants;
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
            case "2":
                ProcessarAcaoIniciarOuRetomar();
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
        var resultadoProgramas = await programaService.ListarAsync();
        var programas = resultadoProgramas.Data?.ToList() ?? new List<ProgramaAquecimento>();

        for (var i = 0; i < programas.Count; i++)
        {
            var programa = programas[i];
            var prefixoItalico = programa.EhPadrao ? "" : "\e[3m";
            var sufixoItalico = programa.EhPadrao ? "" : "\e[23m";

            System.Console.WriteLine(
                $"{i + 1} - {prefixoItalico}{programa.Nome} (Alimento: {programa.Alimento}, Tempo: {programa.TempoSegundos}s, Potência: {programa.Potencia}){sufixoItalico}");
        }

        System.Console.WriteLine("0 - Voltar");
        System.Console.Write("Escolha um programa: ");

        var inputOpcao = System.Console.ReadLine();
        if (inputOpcao == "0") return;

        if (!int.TryParse(inputOpcao, out var indice) || indice <= 0 || indice > programas.Count)
            throw new ValidacaoMicroondasException("Opção de programa inválida.");

        var programaSelecionado = programas[indice - 1];
        maquina.ConfigurarPrograma(programaSelecionado);
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

        var tempo = LerIntNoIntervalo("Tempo (segundos): ", ValoresPadrao.TempoMinimoSegundos,
            ValoresPadrao.TempoMaximoProgramaSegundos);
        if (tempo == null) return;

        var potencia = LerIntNoIntervalo($"Potência ({ValoresPadrao.PotenciaMinima}-{ValoresPadrao.PotenciaMaxima}): ",
            ValoresPadrao.PotenciaMinima, ValoresPadrao.PotenciaMaxima);
        if (potencia == null) return;

        var caractere = await LerCaractereUnicoAsync("Caractere de Aquecimento: ");
        if (caractere == null) return;

        System.Console.Write("Instruções (opcional): ");
        var instrucoes = System.Console.ReadLine() ?? "";

        try
        {
            var programaDto = new ProgramaDto(nome, alimento, tempo.Value, potencia.Value, caractere.Value, instrucoes,
                false);
            var novoPrograma = programaDto.ParaEntidade();

            var resultadoCadastro = await programaService.CadastrarAsync(novoPrograma);
            if (resultadoCadastro.IsError)
            {
                MicroondasVisor.ExibirErroValidacao(resultadoCadastro.Errors?.FirstOrDefault()?.Message ?? "Erro desconhecido");
                return;
            }

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

            if (!await programaService.ExisteNomeAsync(input))
                return input;
            
            System.Console.WriteLine($"[ERRO]: Já existe um programa cadastrado com o nome '{input}'.");
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

    private static int? LerIntNoIntervalo(string prompt, int minimo, int maximo)
    {
        while (true)
        {
            System.Console.Write(prompt);
            var input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input == "0")
                return null;

            if (int.TryParse(input, out var valor) && valor >= minimo && valor <= maximo)
                return valor;

            System.Console.WriteLine($"[ERRO]: Por favor, insira um número entre {minimo} e {maximo}.");
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

            if (input.Length != 1)
            {
                System.Console.WriteLine("[ERRO]: Insira exatamente um caractere.");
                continue;
            }

            var caractereInformado = input[0];
            if (caractereInformado == ValoresPadrao.CaracterePadrao)
            {
                System.Console.WriteLine(
                    $"[ERRO]: O caractere '{ValoresPadrao.CaracterePadrao}' é reservado para o aquecimento padrão.");
                continue;
            }

            if (!await programaService.ExisteCaractereAsync(caractereInformado))
                return caractereInformado;

            System.Console.WriteLine(
                $"[ERRO]: O caractere '{caractereInformado}' já está sendo usado por outro programa.");
        }
    }

    private void ProcessarAcaoIniciarOuRetomar()
    {
        if (maquina.Estado is EstadoMicroondas.EmAndamento or EstadoMicroondas.Pausado)
        {
            maquina.Iniciar();
            return;
        }

        var tempoFinal = LerTempoParaInicio();
        if (tempoFinal == -1)
            return;

        var potenciaFinal = LerPotenciaParaInicio();
        if (potenciaFinal == -1)
            return;

        var potenciaEfetiva = potenciaFinal > 0 ? potenciaFinal : ValoresPadrao.PotenciaPadrao;

        var comando = tempoFinal > 0
            ? new ConfigurarMicroondasCommand { TempoEmSegundos = tempoFinal, Potencia = potenciaEfetiva }
            : null;

        maquina.Iniciar(comando);
    }

    private static int LerTempoParaInicio()
    {
        while (true)
        {
            System.Console.Write(
                $"Tempo (segundos) [{ValoresPadrao.TempoMinimoSegundos}-{ValoresPadrao.TempoMaximoSegundos}s, vazio p/ início rápido]: ");
            var input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) return 0;
            if (input == "0") return -1;

            if (int.TryParse(input, out var tempo) &&
                tempo is >= ValoresPadrao.TempoMinimoSegundos and <= ValoresPadrao.TempoMaximoSegundos)
            {
                return tempo;
            }


            MicroondasVisor.ExibirErroValidacao(
                $"Tempo inválido. Use {ValoresPadrao.TempoMinimoSegundos} a {ValoresPadrao.TempoMaximoSegundos}s.");
        }
    }

    private static int LerPotenciaParaInicio()
    {
        ExibirTecladoDigital();
        while (true)
        {
            System.Console.Write(
                $"Potência ({ValoresPadrao.PotenciaMinima}-{ValoresPadrao.PotenciaMaxima}) [vazio p/ {ValoresPadrao.PotenciaPadrao}]: ");
            var input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) return 0;
            if (input == "0") return -1;

            if (int.TryParse(input, out var potencia)
                && potencia is >= ValoresPadrao.PotenciaMinima and <= ValoresPadrao.PotenciaMaxima)
                return potencia;

            MicroondasVisor.ExibirErroValidacao(
                $"Potência inválida. Use {ValoresPadrao.PotenciaMinima} a {ValoresPadrao.PotenciaMaxima}.");
        }
    }

    private static void ExibirTecladoDigital()
    {
        System.Console.WriteLine("\n[TECLADO DIGITAL]");
        System.Console.WriteLine("[1] [2] [3] [4] [5]");
        System.Console.WriteLine("[6] [7] [8] [9] [0]");
        System.Console.WriteLine();
    }
}