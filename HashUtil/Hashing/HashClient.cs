using HashUtil.Hashing.Algorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HashUtil.Hashing
{
    public abstract class HashClient
    {
        static HashClient()
        {
            var list = HashAlgorithms.Select((kv) => kv.Key).ToList();
            list.Sort();
            AlgorithmNames = list.AsReadOnly();
        }

        protected static Dictionary<string, HashAlgorithm> HashAlgorithms => new Dictionary<string, HashAlgorithm>
        {
            { "CRC-32", Crc32.Create() },
            { "CRC-64 ISO", Crc64Iso.Create() },
            { "CRC-64 Ecma", Crc64Ecma.Create() },
            { "MD5", MD5.Create() },
            { "SHA-1", SHA1.Create() },
            { "SHA-2 256", SHA256.Create() },
            { "SHA-2 384", SHA384.Create() },
            { "SHA-2 512", SHA512.Create() },
            { "SHA-3 224", new Sha3Managed(224) },
            { "SHA-3 256", new Sha3Managed(256) },
            { "SHA-3 384", new Sha3Managed(384) },
            { "SHA-3 512", new Sha3Managed(512) },
        };

        public static IReadOnlyList<string> AlgorithmNames { get; }

        protected HashClient()
        {
            Status = HashStatus.Start;
        }

        protected void DoProgressChanged(ProgressChangedEventArgs args)
        {
            ProgressChanged?.Invoke(this, args);
        }

        protected void DoStatusChanged(HashStatus args)
        {
            Status = args;
            StatusChanged?.Invoke(this, new StatusChangedEventArgs(args));
        }

        protected void CalculateManySinglePass(Stream data, IList<HashAlgorithm> algorithms)
        {
            var buffer = new byte[16 * 1024];
            var read = 0;
            var current = 0;
            int maximum;
            try
            {
                maximum = (int)data.Length;
            }
            catch (NotSupportedException)
            {
                maximum = -1;
            }
            DoStatusChanged(HashStatus.Calculating);
            while ((read = data.Read(buffer, 0, buffer.Length)) > 0)
            {
                Parallel.ForEach(algorithms, (h) =>
                {
                    h.TransformBlock(buffer, 0, read, null, 0);
                });

                current += read;
                
                DoProgressChanged(maximum < 0 ? ProgressChangedEventArgs.Unknown : new ProgressChangedEventArgs(current, maximum));
            }
            foreach (var h in algorithms)
            {
                h.TransformFinalBlock(buffer, 0, read);
            }
        }

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public HashStatus Status { get; private set; }
    }
}
