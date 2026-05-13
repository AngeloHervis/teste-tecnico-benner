using FluentAssertions;
using Microondas.Infrastructure.Security;

namespace Microondas.Tests.Security;

public class CryptoHelperTest
{
    [Fact]
    public void ComputeSha256Hash_Sempre_DeveGerarHashConsistente()
    {
        // Arrange
        const string texto = "123456";
        
        // Act
        var hash1 = CryptoHelper.ComputeSha256Hash(texto);
        var hash2 = CryptoHelper.ComputeSha256Hash(texto);

        // Assert
        hash1.Should().Be(hash2);
        hash1.Should().HaveLength(64);
    }

    [Fact]
    public void EncryptDecrypt_DeveRetornarTextoOriginal()
    {
        // Arrange
        const string original = "Server=localhost;Database=Test;";

        // Act
        var encrypted = CryptoHelper.EncryptConnectionString(original);
        var decrypted = CryptoHelper.DecryptConnectionString(encrypted);

        // Assert
        encrypted.Should().NotBe(original);
        decrypted.Should().Be(original);
    }
}
