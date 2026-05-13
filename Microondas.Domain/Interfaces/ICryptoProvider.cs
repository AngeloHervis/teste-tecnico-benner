namespace Microondas.Domain.Interfaces;

public interface ICryptoProvider
{
    string ComputeHash(string text);
}
