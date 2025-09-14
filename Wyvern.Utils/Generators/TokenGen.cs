using System.Numerics;
using System.Security.Cryptography;

namespace Wyvern.Utils.Generators
{
    public static class TokenGen
    {
        private const string CrockfordBase32 = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        /*
            Refresh: 50
            Auth/Verify: 30
            Bot Tokens: 60
        */
        public static string GenerateToken(int length)
        {
            int totalBits = length * 5;
            int totalBytes = (totalBits + 7) / 8;

            Span<byte> bytes = stackalloc byte[totalBytes];

            long timeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int timestampBytes = Math.Min(6, totalBytes);
            for (int i = 0; i < timestampBytes; i++)
            {
                bytes[timestampBytes - 1 - i] = (byte)(timeMs & 0xFF);
                timeMs >>= 8;
            }

            Span<byte> randomBytes = bytes.Slice(timestampBytes);
            Rng.GetBytes(randomBytes);

            BigInteger value = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);

            char[] chars = new char[length];
            for (int i = length - 1; i >= 0; i--)
            {
                int index = (int)(value % 32);
                chars[i] = CrockfordBase32[index];
                value /= 32;
            }

            Console.WriteLine($"Code String: {new string(chars)}");

            return new string(chars);
        }
    }
}