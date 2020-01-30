using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace KiscoSchedule.Database
{
    class CryptoService
    {
        RC2CryptoServiceProvider rc2;

        /// <summary>
        /// Constuctor for the CryptoService
        /// </summary>
        public CryptoService()
        {
            // Initalize variables
            rc2 = new RC2CryptoServiceProvider();
        }

        /// <summary>
        /// This will hash a string
        /// </summary>
        /// <param name="input">string wanting to be hashed</param>
        /// <returns>hash</returns>
        public string Hash(string input)
        {
            using (SHA256Managed sha2 = new SHA256Managed())
            {
                var hash = sha2.ComputeHash(Encoding.UTF8.GetBytes(input));
                var strinBuilder = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    strinBuilder.Append(b.ToString("x2"));
                }

                return strinBuilder.ToString();
            }
        }

        /// <summary>
        /// Encrypts bytes using RC2
        /// </summary>
        /// <param name="rawBytes">The bytes wanting to be encrypted</param>
        /// <returns></returns>
        private byte[] encryptBytes(byte[] rawBytes)
        {
            byte[] encryptedBytes;

            using (MemoryStream memoryStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rc2.CreateEncryptor(), CryptoStreamMode.Write))
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
        private byte[] decryptBytes(byte[] encryptedBytes)
        {
            byte[] rawBytes;

            using (MemoryStream memoryStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rc2.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                cryptoStream.Close();
                rawBytes = memoryStream.ToArray();
            };

            return rawBytes;
        }
    }
}
