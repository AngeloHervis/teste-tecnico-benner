using Microondas.Domain.Constants;

namespace Microondas.Domain.Entities.Programas;

public sealed class ProgramaPipoca : ProgramaAquecimento
{
    public ProgramaPipoca()
    {
        Nome = "Pipoca";
        Alimento = "Pipoca (de micro-ondas)";
        TempoSegundos = ValoresPadrao.TempoPipoca;
        Potencia = ValoresPadrao.PotenciaPipoca;
        CaractereAquecimento = 'p';
        Instrucoes = "Observar o barulho do estouro do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento.";
        EhPreDefinido = true;
    }
}
