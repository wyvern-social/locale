using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using System.Security.Cryptography;
using System.Text;

public class GenerateHash
{
    private static readonly RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();
    private static Argon2Config config = new Argon2Config();

    public GenerateHash()
    {
        GenerateHash.config.Type = Argon2Type.DataDependentAddressing;
        GenerateHash.config.Version = Argon2Version.Nineteen;
        GenerateHash.config.TimeCost = 10;
        GenerateHash.config.MemoryCost = 32768;
        GenerateHash.config.Lanes = 5;
        GenerateHash.config.Threads = Environment.ProcessorCount; // higher than "Lanes" doesn't help (or hurt)
        GenerateHash.config.Password = null;
        GenerateHash.config.Salt = new byte[8]; // >= 8 bytes if not null
        GenerateHash.config.HashLength = 32;
    }
    // I'm leaving the values from the example here if they're not defined in the function parameters ^^

    public string HashGenerator(string input)
    {
        if (null != GenerateHash.config.Salt) // stupid compiler warning if remove this statement
        {
            //new Random().NextBytes(GenerateHash.config.Salt);
            rng.GetBytes(GenerateHash.config.Salt);
        }

        if (null != GenerateHash.config.Password) // stupid compiler warning if remove this statement
        {
            GenerateHash.config.Password = Encoding.ASCII.GetBytes(input);
        }

        var argon2A = new Argon2(config);
        string hashString;
        using (SecureArray<byte> hashA = argon2A.Hash())
        {
            hashString = config.EncodeString(hashA.Buffer);
        }
        return hashString;
    }
}