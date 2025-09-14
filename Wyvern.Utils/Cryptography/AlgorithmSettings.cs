using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Wyvern.ConfigModel;

namespace Wyvern.Utils.Cryptography
{
    public static class AlgorithmSettings
    {
        private static readonly RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        public static Argon2Config GetConfig(string? input = null) {
            byte[] salt = new byte[Config.GetKey<int>("utils", "cryptography", "argon2", "salt_length")]; // >= 8 bytes if not null
            rng.GetBytes(salt);

            Console.WriteLine("[AlgorithmSettings.cs] Salt (hex rep):\t{0}", BitConverter.ToString(salt).Replace("-", " "));

            Argon2Config config = new ()
            {
                Type = Argon2Type.DataDependentAddressing,
                Version = Argon2Version.Nineteen,
                Threads = Environment.ProcessorCount, // higher than "Lanes" doesn't help (or hurt)
                TimeCost = Config.GetKey<int>("utils", "cryptography", "argon2", "time_cost"),
                MemoryCost = Config.GetKey<int>("utils", "cryptography", "argon2", "memory_cost"),
                Lanes = Config.GetKey<int>("utils", "cryptography", "argon2", "lanes"),
                Salt = salt,
                HashLength = Config.GetKey<int>("utils", "cryptography", "argon2", "hash_length"),
            };

            if (input != null && input != "")
            {
                config.Password = Encoding.ASCII.GetBytes(input);
            }

            return config;
        }
    }
}
