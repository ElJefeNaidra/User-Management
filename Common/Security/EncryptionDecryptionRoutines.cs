using System.Security.Cryptography;
using System.Text;

namespace UserManagement.Common.Security
{
    public static class EncryptionDecryptionRoutines
    {
        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("EDA2D680067E45C78B7E543ECC877781");
        private static string Password = "Astrolite$2024";

        public static string Encrypt(string plainText, string password)
        {


            using var deriveBytes = new Rfc2898DeriveBytes(Password, Salt);
            using var aes = Aes.Create();
            aes.Key = deriveBytes.GetBytes(32);  // 32 bytes = 256 bits
            aes.IV = deriveBytes.GetBytes(16);   // 16 bytes = 128 bits

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(plainText);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static string Decrypt(string cipherText, string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(Password, Salt);
            using var aes = Aes.Create();
            aes.Key = deriveBytes.GetBytes(32);
            aes.IV = deriveBytes.GetBytes(16);

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using (var reader = new StreamReader(cryptoStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

