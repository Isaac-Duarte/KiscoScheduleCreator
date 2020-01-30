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
        byte[] salt =
        {
            0xAC, 0xA7, 0x3D, 0x46, 0xF0, 0xCD, 0x9F, 0x30, 0x2A, 0x4E, 0x0C, 
            0xD1, 0xA7, 0x27, 0xF3, 0x8C, 0x22, 0xC8, 0x5E, 0x7C, 0xFE, 0xDB, 
            0xA3, 0x93, 0xA8, 0x0D, 0x93, 0x7A, 0xCB, 0xF8, 0xAA, 0xB0, 0x60, 
            0x45, 0x4F, 0x9D, 0xF2, 0xF4, 0xCD, 0xB1, 0xE0, 0xEB, 0x39, 0x47, 
            0x7B, 0xBA, 0x91, 0xF4, 0x6E, 0x27, 0x71, 0xBC, 0x8F, 0xC9, 0xC7, 
            0xFF, 0x65, 0x08, 0x62, 0xF1, 0xF8, 0xCE, 0xB4, 0xEB
        };

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
