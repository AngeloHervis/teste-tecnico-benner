namespace Microondas.Domain.Interfaces;

public interface ITokenService
{
    string GenerateToken(string username);
}
