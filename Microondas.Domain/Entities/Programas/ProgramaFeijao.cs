using Microondas.Domain.Constants;

namespace Microondas.Domain.Entities.Programas;

public sealed class ProgramaFeijao : ProgramaAquecimento
{
    public ProgramaFeijao()
    {
        Nome = "Feijão";
        Alimento = "Feijão";
        TempoSegundos = ValoresPadrao.TempoFeijao;
        Potencia = ValoresPadrao.PotenciaFeijao;
        CaractereAquecimento = 'j';
        Instrucoes = "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas.";
        EhPreDefinido = true;
    }
}
