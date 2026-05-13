using System.Security.Cryptography;
using System.Text;

namespace Microondas.Infrastructure.Security;

public static class CryptoHelper
{
    private static readonly byte[] Key = "b14ca5898a4e4133bbce2ea2315a1916"u8.ToArray();
    private static readonly byte[] Iv = "813c9a6e1d2e4f5a"u8.ToArray();

    public static string ComputeSha256Hash(string rawData)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }

    public static string EncryptConnectionString(string connectionString)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = Iv;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(connectionString);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string DecryptConnectionString(string encryptedString)
    {
        if (string.IsNullOrWhiteSpace(encryptedString)) return encryptedString;

        try
        {
            var buffer = Convert.FromBase64String(encryptedString);
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = Iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(buffer);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            
            return sr.ReadToEnd();
        }
        catch
        {
            return encryptedString;
        }
    }
}
