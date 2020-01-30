using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SaltGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] salt = GenerateSalt(64);
            string hex = BitConverter.ToString(salt);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (string singleHex in hex.Split('-'))
            {
                stringBuilder.Append("0x" + singleHex.Replace("-", string.Empty) + ", ");
            }

            Console.WriteLine(stringBuilder.ToString());
            Console.ReadLine();
        }

        static byte[] GenerateSalt(int length = 64)
        {
            byte[] salt = new byte[length];

            using (RNGCryptoServiceProvider random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
    }
}
