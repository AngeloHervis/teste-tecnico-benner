using System.ComponentModel.DataAnnotations;
using Microondas.Domain.Constants;
using Microondas.Domain.Exceptions;

namespace Microondas.Domain.Entities;

public class ProgramaAquecimento
{
    public int Id { get; }
    public string Nome { get; private set; }
    public string Alimento { get; private set; }
    public int TempoSegundos { get; private set; }
    public int Potencia { get; private set; }
    public char CaractereAquecimento { get; private set; }
    [MaxLength(500)] public string? Instrucoes { get; private set; }
    public bool EhPadrao { get; private set; }

    public ProgramaAquecimento()
    {
    }

    public ProgramaAquecimento(string nome, string alimento, int tempoSegundos, int potencia, char caractereAquecimento, string instrucoes, bool ehPadrao = false)
    {
        Validar(nome, alimento, tempoSegundos, potencia, caractereAquecimento);

        Nome = nome;
        Alimento = alimento;
        TempoSegundos = tempoSegundos;
        Potencia = potencia;
        CaractereAquecimento = caractereAquecimento;
        Instrucoes = instrucoes;
        EhPadrao = ehPadrao;
    }

    private static void Validar(string nome, string alimento, int tempoSegundos, int potencia, char caractere)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ValidacaoMicroondasException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(alimento))
            throw new ValidacaoMicroondasException("Alimento é obrigatório.");

        if (tempoSegundos is < ValoresPadrao.TempoMinimoSegundos or > ValoresPadrao.TempoMaximoProgramaSegundos)
            throw new ValidacaoMicroondasException($"Tempo deve estar entre {ValoresPadrao.TempoMinimoSegundos} e {ValoresPadrao.TempoMaximoProgramaSegundos} segundos.");

        if (potencia is < ValoresPadrao.PotenciaMinima or > ValoresPadrao.PotenciaMaxima)
            throw new ValidacaoMicroondasException($"Potência deve estar entre {ValoresPadrao.PotenciaMinima} e {ValoresPadrao.PotenciaMaxima}.");

        if (caractere == ValoresPadrao.CaracterePadrao)
            throw new ValidacaoMicroondasException($"O caractere '{ValoresPadrao.CaracterePadrao}' é reservado para o aquecimento padrão.");
    }
}
