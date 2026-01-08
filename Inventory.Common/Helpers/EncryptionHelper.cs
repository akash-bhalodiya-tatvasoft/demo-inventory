using System.Security.Cryptography;
using System.Text;

namespace Inventory.Common.Helpers;

public static class EncryptionHelper
{
    private static readonly string SecretKey = "fsdhfiushdfiusbnvmlkewo123456785";

    private static readonly string SecretIV = "gds984dfgdjgbnj1";

    public static string EncryptId(string id)
    {
        if (string.IsNullOrEmpty(id))
            return string.Empty;

        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(SecretKey);
        aes.IV = Encoding.UTF8.GetBytes(SecretIV);

        byte[] inputBytes = Encoding.UTF8.GetBytes(id);
        byte[] encrypted = aes.CreateEncryptor()
                              .TransformFinalBlock(inputBytes, 0, inputBytes.Length);

        return Convert.ToBase64String(encrypted).Replace("+", "-").Replace("/", "_");
    }

    public static string DecryptId(string encryptedId)
    {
        encryptedId = encryptedId.Replace("-", "+").Replace("_", "/");

        if (string.IsNullOrEmpty(encryptedId))
            return string.Empty;

        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(SecretKey);
        aes.IV = Encoding.UTF8.GetBytes(SecretIV);

        byte[] cipherBytes = Convert.FromBase64String(encryptedId);
        byte[] decrypted = aes.CreateDecryptor()
                              .TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(decrypted);
    }
}
