using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Wyvern.Utils.Generators
{
    public static class IdGen
    {
        private const string CrockfordBase32 = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        public static string GenerateId()
        {
            return GenerateId(DateTimeOffset.UtcNow);
        }

        private static string GenerateId(DateTimeOffset timestamp)
        {
            Span<byte> bytes = stackalloc byte[16];

            long timeMs = timestamp.ToUnixTimeMilliseconds();
            for (int i = 0; i < 6; i++)
            {
                bytes[5 - i] = (byte)(timeMs & 0xFF);
                timeMs >>= 8;
            }

            Span<byte> randomBytes = bytes.Slice(6);
            Rng.GetBytes(randomBytes);

            BigInteger value = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);

            char[] chars = new char[26];
            for (int i = 25; i >= 0; i--)
            {
                int index = (int)(value % 32);
                chars[i] = CrockfordBase32[index];
                value /= 32;
            }

            return new string(chars);
        }
    }
}
