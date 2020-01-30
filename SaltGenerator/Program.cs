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
            Console.Write("How much salt do you want?: ");

            try
            {
                int saltLength = Int32.Parse(Console.ReadLine());
                outputSalt(saltLength);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stupid, you fukced it up!\n{ex.Message}");
            }

            Console.ReadLine();
        }

        private static void outputSalt(int saltLength)
        {
            byte[] salt = GenerateSalt(saltLength);
            string hex = BitConverter.ToString(salt);
            string[] hexes = hex.Split('-');
            StringBuilder stringBuilder = new StringBuilder();
            int maxLength = 10;
            int currentLength = 0;

            stringBuilder.Append("private readonly byte[] salt =\n{\n    ");

            for (int i = 0; i < hexes.Length; i++)
            {
                string singleHex = hexes[i];

                if (currentLength >= maxLength)
                {
                    currentLength = 0;
                    stringBuilder.Append("\n    ");
                }

                if (i == hexes.Length - 1)
                {
                    stringBuilder.Append("0x" + singleHex);
                    continue;
                }

                stringBuilder.Append("0x" + singleHex + ", ");
                currentLength++;
            }

            stringBuilder.Append("\n};");

            Console.WriteLine(stringBuilder.ToString());
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
