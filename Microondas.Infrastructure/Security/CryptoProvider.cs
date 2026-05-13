using System.Security.Cryptography;
using System.Text;
using Microondas.Domain.Interfaces;

namespace Microondas.Infrastructure.Security;

public class CryptoProvider : ICryptoProvider
{
    public string ComputeHash(string text)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }
}
