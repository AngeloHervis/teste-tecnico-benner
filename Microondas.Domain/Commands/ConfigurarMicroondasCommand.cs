namespace Microondas.Domain.Commands;

public class ConfigurarMicroondasCommand
{
    public int TempoEmSegundos { get; set; }
    public int? Potencia { get; set; }
}
