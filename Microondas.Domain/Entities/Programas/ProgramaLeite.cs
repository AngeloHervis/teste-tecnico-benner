using Microondas.Domain.Constants;

namespace Microondas.Domain.Entities.Programas;

public sealed class ProgramaLeite : ProgramaAquecimento
{
    public ProgramaLeite()
    {
        Nome = "Leite";
        Alimento = "Leite";
        TempoSegundos = ValoresPadrao.TempoLeite;
        Potencia = ValoresPadrao.PotenciaLeite;
        CaractereAquecimento = 'l';
        Instrucoes = "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar ferfura imediata causando risco de queimaduras.";
        EhPreDefinido = true;
    }
}
