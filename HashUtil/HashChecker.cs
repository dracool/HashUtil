using HashUtil.Algorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HashUtil
{
    [Serializable]
    class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(int current, int upperBound)
        {
            IsBoundUnknown = upperBound < current || upperBound < 0;
            Current = current;
            UpperBound = upperBound;
        }

        public static ProgressEventArgs Unknown { get; } = new ProgressEventArgs(-1, -1);

        public bool IsBoundUnknown { get; }
        public int Current { get; }
        public int UpperBound { get; }
    }

    class HashChecker
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

        private void CalculateManySinglePass(IList<HashAlgorithm> algorithms)
        {
            var buffer = new byte[16 * 1024];
            var read = 0;
            var current = 0;
            int maximum;
            try
            {
                maximum = (int)Data.Length;
            }
            catch(NotSupportedException)
            {
                maximum = -1;
            }
            while ((read = Data.Read(buffer, 0, buffer.Length)) > 0)
            {
                Parallel.ForEach(algorithms, (h) =>
                {
                    h.TransformBlock(buffer, 0, read, null, 0);
                });
                
                current += read;
                DoProgress(current, maximum);
            }
            foreach (var h in algorithms)
            {
                h.TransformFinalBlock(buffer, 0, read);
            }
        }

        public Stream Data { get; set; }
        public event EventHandler<ProgressEventArgs> Progress;

        protected void DoProgress(int current, int upper)
        {
            Progress?.Invoke(this, new ProgressEventArgs(current, upper));
        }

        public IList<string> FindMatch(byte[] hash)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            if (Data == null) throw new InvalidOperationException("Cannot find a match when Data is null");

            var selected = HashAlgorithms
                .Where(kv => kv.Value.HashSize == (hash.Length * 8));


            CalculateManySinglePass(selected.Select(kv => kv.Value).ToList());

            return selected
                .Where(kv => kv.Value.Hash.SequenceEqual(hash))
                .Select(kv => kv.Key)
                .ToList();
        }

        public Dictionary<string, byte[]> CalculateHashes()
        {
            if (Data == null) throw new InvalidOperationException("Cannot calculate hashes when Data is null");
            var algs = HashAlgorithms;
            CalculateManySinglePass(algs.Select(kv => kv.Value).ToList());

            return algs.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Hash
            );
        }
    }
}
