using Microondas.Domain.Commands;

namespace Microondas.Tests.TestBuilders;

public class ConfigurarMicroondasCommandBuilder : BaseBuilder<ConfigurarMicroondasCommand>
{
    public ConfigurarMicroondasCommandBuilder()
    {
        Faker.RuleFor(r => r.TempoEmSegundos, f => 30);
        Faker.RuleFor(r => r.Potencia, f => 10);
    }

    public ConfigurarMicroondasCommandBuilder ComTempo(int tempoEmSegundos)
    {
        Faker.RuleFor(r => r.TempoEmSegundos, tempoEmSegundos);
        return this;
    }

    public ConfigurarMicroondasCommandBuilder ComPotencia(int? potencia)
    {
        Faker.RuleFor(r => r.Potencia, potencia);
        return this;
    }
}
