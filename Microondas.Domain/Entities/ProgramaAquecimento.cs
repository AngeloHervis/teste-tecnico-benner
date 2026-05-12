namespace Microondas.Domain.Entities;

public abstract class ProgramaAquecimento
{
    public string Nome { get; protected set; } = string.Empty;
    public string Alimento { get; protected set; } = string.Empty;
    public int TempoSegundos { get; protected set; }
    public int Potencia { get; protected set; }
    public char CaractereAquecimento { get; protected set; }
    public string Instrucoes { get; protected set; } = string.Empty;
    public bool EhPreDefinido { get; protected set; } = true;
}
