using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Database.Services
{
    interface ICryptoService
    {
        /// <summary>
        /// Generates the crypto service provider
        /// </summary>
        /// <param name="password">password to derive from</param>
        void GenerateCryptoProvider(string password);

        /// <summary>
        /// Encrypts bytes using RC2
        /// </summary>
        /// <param name="rawBytes">The bytes wanting to be encrypted</param>
        /// <returns></returns>
        byte[] EncryptBytes(byte[] rawBytes);

        /// <summary>
        /// Decrypts the byes using RC2
        /// </summary>
        /// <param name="encryptedBytes">The encrypted bytes wanted to be decrypted</param>
        /// <returns></returns>
        byte[] DecryptBytes(byte[] encryptedBytes);

        /// <summary>
        /// Encrypts bytes using RC2
        /// </summary>
        /// <param name="raw">The string wanting to be encrypted</param>
        /// <returns></returns>
        byte[] EncryptString(string raw);

        /// <summary>
        /// Decrypts the byes using RC2 into a string
        /// </summary>
        /// <param name="encryptedBytes">The encrypted bytes wanted to be decrypted</param>
        /// <returns></returns>
        string DecryptBytesToString(byte[] encryptedBytes);
    }
}
