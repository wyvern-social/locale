using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using System.Text;
using Wyvern.ConfigModel;
using Wyvern.Utils.Cryptography;

public static class CheckHash
{
    public static bool HashChecker(string input, string hashToCheckAgainst)
    {
        bool verified = false;
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        Argon2Config configOfPasswordToVerify = new() { Password = inputBytes, Threads = 5 };
        SecureArray<byte>? hashedInput = null;
        try
        {
            if (configOfPasswordToVerify.DecodeString(hashToCheckAgainst, out hashedInput) && hashedInput != null)
            {
                var argon2ToVerify = new Argon2(configOfPasswordToVerify);
                using (var hashToVerify = argon2ToVerify.Hash())
                {
                    if (Argon2.FixedTimeEquals(hashedInput, hashToVerify))
                    {
                        Console.WriteLine("HASH MATCH!!!");
                        verified = true;
                    }
                    else
                    {
                        Console.WriteLine("HASH MISMATCH!!!");
                    }
                }
            }
        }
        finally
        {
            hashedInput?.Dispose();
        }
        return verified;
    }
}