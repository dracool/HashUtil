using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.IO;
using HashUtil.Algorithms;

namespace MicroBenchmark
{
    static class HashChecker
    {
        private static Dictionary<string, HashAlgorithm> HashAlgorithms => new Dictionary<string, HashAlgorithm>
        {
            { "CRC-32", Crc32.Create() },
            { "CRC-64 ISO", Crc64Iso.Create() },
            { "CRC-64 Ecma", Crc64Ecma.Create() },
            { "MD5", MD5.Create() },
            { "SHA-1", SHA1.Create() },
            { "SHA-2 256", SHA256.Create() },
            { "SHA-2 384", SHA384.Create() },
            { "SHA-2 512", SHA512.Create() },
            { "SHA-3 224", new SHA3Managed(224) },
            { "SHA-3 256", new SHA3Managed(256) },
            { "SHA-3 384", new SHA3Managed(384) },
            { "SHA-3 512", new SHA3Managed(512) },
        };
        
        private static void CalculateManySinglePass(IList<HashAlgorithm> algorithms, Stream stream)
        {
            var buffer = new byte[16 * 1024];
            var read = 0;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                foreach (var h in algorithms)
                {
                    h.TransformBlock(buffer, 0, read, null, 0);
                }
            }
            foreach (var h in algorithms)
            {
                h.TransformFinalBlock(buffer, 0, read);
            }
        }

        public static IList<string> FindMatch(byte[] hash, Stream stream)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var selected = HashAlgorithms
                .Where(kv => kv.Value.HashSize == (hash.Length * 8));

            CalculateManySinglePass(selected.Select(kv => kv.Value).ToList(), stream);

            return selected
                .Where(kv => kv.Value.Hash.SequenceEqual(hash))
                .Select(kv => kv.Key)
                .ToList();
        }
        
        public static Dictionary<string, byte[]> CalculateHashes(Stream stream)
        {
            CalculateManySinglePass(HashAlgorithms.Select(kv => kv.Value).ToList(), stream);

            return HashAlgorithms.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Hash
            );
        }
    }

    class Program
    {
        public static string HashToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        private static byte[] HexToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        static void Main(string[] args)
        {
            var path = @"C:\Users\NeXtDracool\Downloads\vs_community_ENU.exe";
            using (var mem = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var matches = HashChecker.CalculateHashes(mem);
                matches = HashChecker.CalculateHashes(mem);
                foreach (var match in matches)
                {
                    Console.WriteLine($"{match.Key}:\t{HashToString(match.Value)}");
                }
            }

            Console.ReadLine();
        }
    }
}
