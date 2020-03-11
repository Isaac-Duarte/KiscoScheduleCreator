using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace KiscoSchedule.Database.Services
{
    public class CryptoService : ICryptoService
    {
        private AesCryptoServiceProvider aes;

        /// <summary>
        /// Highly reccommended that this is changed!
        /// You can use the SaltGenerator in this solution!
        /// </summary>
        private static readonly byte[] salt =
        {
            0x55, 0xBC, 0x0C, 0xA8, 0xBD, 0x25, 0xBF, 0xD3, 0x6E, 0x49,
            0x7D, 0xD1, 0x35, 0x3B, 0xC0, 0xC0, 0xB5, 0xC6, 0x27, 0x1C,
            0x03, 0xDF, 0xB3, 0xA2, 0x23, 0x94, 0xAB, 0x4A, 0x8F, 0x3D,
            0xD8, 0xE9, 0x62, 0x8C, 0xD8, 0xA7, 0xF3, 0xE0, 0x1D, 0x94,
            0x25, 0xEF, 0xB0, 0xCB, 0xCA, 0xC5, 0xB8, 0x08, 0x30, 0x34,
            0x83, 0xF5, 0xB0, 0x33, 0x95, 0xE2, 0x55, 0xED, 0x7F, 0xF5,
            0xA9, 0xA0, 0xAF, 0x8F
        };

        /// <summary>
        /// Constuctor for the CryptoService
        /// </summary>
        public CryptoService()
        {
            // Initalize variables
            aes = new AesCryptoServiceProvider();
        }

        /// <summary>
        /// Generates the crypto service provider
        /// </summary>
        /// <param name="password">password to derive from</param>
        public void GenerateCryptoProvider(string password)
        {
            // Initalize the RC2 Key & IV
            Rfc2898DeriveBytes passwordGenerator = new Rfc2898DeriveBytes(password, salt, 10000);

            // Get the key & IV
            byte[] key = passwordGenerator.GetBytes(aes.KeySize / 8);
            byte[] iv = passwordGenerator.GetBytes(aes.BlockSize/ 8);

            // Modify current RC2 variable
            aes = new AesCryptoServiceProvider();

            aes.Key = key;
            aes.IV = iv;
        }

        /// <summary>
        /// This will hash a string
        /// </summary>
        /// <param name="input">string wanting to be hashed</param>
        /// <returns>hash</returns>
        public static string Hash(string input)
        {
            using (SHA256Managed sha2 = new SHA256Managed())
            {
                byte[] hash = generateSaltedHash(Encoding.UTF8.GetBytes(input), salt);
                StringBuilder strinBuilder = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    strinBuilder.Append(b.ToString("x2"));
                }

                return strinBuilder.ToString();
            }
        }

        /// <summary>
        /// https://stackoverflow.com/a/2138588
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        private static byte[] generateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        /// <summary>
        /// Encrypts bytes using RC2
        /// </summary>
        /// <param name="rawBytes">The bytes wanting to be encrypted</param>
        /// <returns></returns>
        public byte[] EncryptBytes(byte[] rawBytes)
        {
            byte[] encryptedBytes;

            using (MemoryStream memoryStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(rawBytes, 0, rawBytes.Length);
                cryptoStream.Close();
                encryptedBytes = memoryStream.ToArray();
            };

            return encryptedBytes;
        }

        /// <summary>
        /// Decrypts the byes using RC2
        /// </summary>
        /// <param name="encryptedBytes">The encrypted bytes wanted to be decrypted</param>
        /// <returns></returns>
        public byte[] DecryptBytes(byte[] encryptedBytes)
        {
            byte[] rawBytes;

            using (MemoryStream memoryStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                cryptoStream.Close();
                rawBytes = memoryStream.ToArray();
            };

            return rawBytes;
        }

        /// <summary>
        /// Encrypts bytes using RC2
        /// </summary>
        /// <param name="raw">The string wanting to be encrypted</param>
        /// <returns></returns>
        public byte[] EncryptString(string raw)
        {
            return EncryptBytes(Encoding.UTF8.GetBytes(raw));
        }

        /// <summary>
        /// Decrypts the byes using RC2 into a string
        /// </summary>
        /// <param name="encryptedBytes">The encrypted bytes wanted to be decrypted</param>
        /// <returns></returns>
        public string DecryptBytesToString(byte[] encryptedBytes)
        {
            return Encoding.UTF8.GetString(DecryptBytes(encryptedBytes));
        }
    }
}
