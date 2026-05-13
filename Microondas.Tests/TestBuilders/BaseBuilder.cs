using Bogus;

namespace Microondas.Tests.TestBuilders;

public abstract class BaseBuilder<T> where T : class
{
    protected readonly Faker<T> Faker = new("pt_BR");

    public T Build() => Faker.Generate();
}
