using Microondas.Domain.Commands;
using Microondas.Domain.Constants;
using Microondas.Domain.Enums;
using Microondas.Domain.Exceptions;

namespace Microondas.Domain.Entities;

public sealed class MaquinaMicroondas
{
    public int TempoRestanteSegundos { get; private set; }
    public int Potencia { get; private set; }
    public EstadoMicroondas Estado { get; private set; } = EstadoMicroondas.Inativo;
    public string VisorAquecimento { get; private set; } = string.Empty;
    public ProgramaAquecimento? ProgramaAtual { get; private set; }

    public void Configurar(ConfigurarMicroondasCommand comando)
    {
        if (comando.TempoEmSegundos is < ValoresPadrao.TempoMinimoSegundos or > ValoresPadrao.TempoMaximoSegundos)
            throw new ValidacaoMicroondasException($"Tempo deve estar entre {ValoresPadrao.TempoMinimoSegundos} segundo e {ValoresPadrao.TempoMaximoSegundos / 60} minutos");

        if (comando.Potencia is < ValoresPadrao.PotenciaMinima or > ValoresPadrao.PotenciaMaxima)
            throw new ValidacaoMicroondasException($"Potência deve ser um valor de {ValoresPadrao.PotenciaMinima} a {ValoresPadrao.PotenciaMaxima}");

        var tempoReal = comando.TempoEmSegundos;
        if (comando.TempoEmSegundos is >= 60 and <= 100)
            tempoReal = comando.TempoEmSegundos;

        TempoRestanteSegundos = tempoReal;
        Potencia = comando.Potencia ?? ValoresPadrao.PotenciaPadrao;
        Estado = EstadoMicroondas.Inativo;
        VisorAquecimento = string.Empty;
        ProgramaAtual = null;
    }

    public void ConfigurarPrograma(ProgramaAquecimento programa)
    {
        TempoRestanteSegundos = programa.TempoSegundos;
        Potencia = programa.Potencia;
        ProgramaAtual = programa;
        Estado = EstadoMicroondas.Inativo;
        VisorAquecimento = string.Empty;
    }

    public void Iniciar()
    {
        if (TempoRestanteSegundos == 0 && Estado != EstadoMicroondas.Pausado)
            throw new ValidacaoMicroondasException("Nenhum tempo configurado.");

        Estado = EstadoMicroondas.EmAndamento;
    }

    public void InicioRapido()
    {
        TempoRestanteSegundos = ValoresPadrao.AcrescimoTempoSegundos;
        Potencia = ValoresPadrao.PotenciaPadrao;
        VisorAquecimento = string.Empty;
        ProgramaAtual = null;
        Iniciar();
    }

    public void AdicionarTempo(int segundos)
    {
        if (Estado != EstadoMicroondas.EmAndamento) 
            return;

        if (ProgramaAtual != null)
            return;
        
        TempoRestanteSegundos += segundos;
        if (TempoRestanteSegundos > ValoresPadrao.TempoMaximoSegundos)
            TempoRestanteSegundos = ValoresPadrao.TempoMaximoSegundos;
    }

    public void PausarOuCancelar()
    {
        switch (Estado)
        {
            case EstadoMicroondas.EmAndamento:
                Estado = EstadoMicroondas.Pausado;
                break;
            case EstadoMicroondas.Inativo:
                TempoRestanteSegundos = 0;
                Potencia = 0;
                VisorAquecimento = string.Empty;
                ProgramaAtual = null;
                break;
            case EstadoMicroondas.Concluido or EstadoMicroondas.Pausado:
                Estado = EstadoMicroondas.Inativo;
                TempoRestanteSegundos = 0;
                Potencia = 0;
                VisorAquecimento = string.Empty;
                ProgramaAtual = null;
                break;
        }
    }

    public void AvancarSegundo()
    {
        if (Estado != EstadoMicroondas.EmAndamento)
            return;

        if (TempoRestanteSegundos > 0)
        {
            TempoRestanteSegundos--;
            var caractere = ProgramaAtual?.CaractereAquecimento ?? '.';
            VisorAquecimento += new string(caractere, Potencia) + " ";
        }

        if (TempoRestanteSegundos != 0) 
            return;
        
        Estado = EstadoMicroondas.Concluido;
        VisorAquecimento += "Aquecimento concluído";
    }

    public string ObterTempoFormatado()
    {
        var minutos = TempoRestanteSegundos / 60;
        var segundos = TempoRestanteSegundos % 60;

        return minutos > 0 ? $"{minutos}:{segundos:D2}" : $"{segundos}s";
    }
}
