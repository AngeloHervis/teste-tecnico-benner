using Microondas.Domain.Constants;

namespace Microondas.Domain.Entities.Programas;

public sealed class ProgramaCarneDeBoi : ProgramaAquecimento
{
    public ProgramaCarneDeBoi()
    {
        Nome = "Carnes de boi";
        Alimento = "Carne em pedaço ou fatias";
        TempoSegundos = ValoresPadrao.TempoCarneDeBoi;
        Potencia = ValoresPadrao.PotenciaCarneDeBoi;
        CaractereAquecimento = 'c';
        Instrucoes = "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme";
        EhPreDefinido = true;
    }
}
