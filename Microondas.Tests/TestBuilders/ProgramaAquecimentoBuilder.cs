using Microondas.Domain.Entities;
using Bogus;

namespace Microondas.Tests.TestBuilders;

public class ProgramaAquecimentoBuilder : BaseBuilder<ProgramaAquecimento>
{
    public ProgramaAquecimentoBuilder()
    {
        Faker.RuleFor(e => e.Nome, f => f.Commerce.ProductName());
        Faker.RuleFor(e => e.Alimento, f => f.Commerce.Product());
        Faker.RuleFor(e => e.TempoSegundos, f => f.Random.Int(10, 300));
        Faker.RuleFor(e => e.Potencia, f => f.Random.Int(1, 10));
        Faker.RuleFor(e => e.CaractereAquecimento, f => f.Random.Char('A', 'Z').ToString().Replace(".", "#")[0]);
        Faker.RuleFor(e => e.Instrucoes, f => f.Lorem.Sentence());
        Faker.RuleFor(e => e.EhPadrao, _ => false);
    }

    public ProgramaAquecimentoBuilder ComNome(string nome)
    {
        Faker.RuleFor(e => e.Nome, nome);
        return this;
    }

    public ProgramaAquecimentoBuilder ComAlimento(string alimento)
    {
        Faker.RuleFor(e => e.Alimento, alimento);
        return this;
    }

    public ProgramaAquecimentoBuilder ComTempo(int tempo)
    {
        Faker.RuleFor(e => e.TempoSegundos, tempo);
        return this;
    }

    public ProgramaAquecimentoBuilder ComPotencia(int potencia)
    {
        Faker.RuleFor(e => e.Potencia, potencia);
        return this;
    }

    public ProgramaAquecimentoBuilder ComCaractere(char caractere)
    {
        Faker.RuleFor(e => e.CaractereAquecimento, caractere);
        return this;
    }

    public ProgramaAquecimentoBuilder EhPadrao(bool ehPadrao = true)
    {
        Faker.RuleFor(e => e.EhPadrao, ehPadrao);
        return this;
    }
}
