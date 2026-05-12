using Microondas.Domain.Constants;

namespace Microondas.Domain.Entities.Programas;

public sealed class ProgramaFrango : ProgramaAquecimento
{
    public ProgramaFrango()
    {
        Nome = "Frango";
        Alimento = "Frango";
        TempoSegundos = ValoresPadrao.TempoFrango;
        Potencia = ValoresPadrao.PotenciaFrango;
        CaractereAquecimento = 'f';
        Instrucoes = "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme";
        EhPreDefinido = true;
    }
}
