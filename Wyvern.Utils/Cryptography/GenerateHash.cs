using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using System.Security.Cryptography;
using System.Text;
using Wyvern.ConfigModel;

namespace Wyvern.Utils.Cryptography
{
    public static class GenerateHash
    {
        public static string HashGenerator(string input)
        {
            Argon2Config config = AlgorithmSettings.GetConfig();
            config.Password = Encoding.ASCII.GetBytes(input);
            var argon2A = new Argon2(config);
            string hashString;
            using (SecureArray<byte> hashA = argon2A.Hash())
            {
                hashString = config.EncodeString(hashA.Buffer);
            }
            Console.WriteLine("[GenerateHash.cs] Plaintext:\t\t<REDACTED>");
            Console.WriteLine("[GenerateHash.cs] Ciphertext:\t\t{0}", hashString);
            return hashString;
        }
    }
}