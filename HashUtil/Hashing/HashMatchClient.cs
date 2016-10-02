using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace HashUtil.Hashing
{
    public sealed class HashMatchClient<T> : HashClient
    {
        public HashMatchClient()
        {
            _result = new Dictionary<T, string>();
            Result = new ReadOnlyDictionary<T, string>(_result);
        }

        private void DoMatch(IEnumerable<T> hashes, Func<T, Hash> get, Stream stream)
        {
            DoStatusChanged(HashStatus.Start);
            var algs = HashAlgorithms;
            var selected = hashes
                .Select(kv => new KeyValuePair<T, List<KeyValuePair<string, HashAlgorithm>>>(
                   kv,
                   algs.Where(alg => alg.Value.HashSize == get(kv).Length).ToList()
                ))
                .ToList();

            var calc = selected
                .SelectMany(val => val.Value.Select(kv => kv.Value))
                .Distinct()
                .ToList();

            CalculateManySinglePass(stream, calc);

            var r = selected
                .Where(val => val.Value.Any(kv => kv.Value.Hash == get(val.Key)))
                .ToDictionary(
                  kv => kv.Key,
                  kv => kv.Value.First(h => h.Value.Hash== get(kv.Key)).Key
                );
            
            _result.Clear();

            if (r.Count > 0)
            {
                foreach(var kv in r)
                {
                    _result.Add(kv.Key, kv.Value);
                }
                DoStatusChanged(HashStatus.Success);
            }
            else
            {
                DoStatusChanged(HashStatus.Failure);
            }
        }

        private readonly Dictionary<T, string> _result;
        public IReadOnlyDictionary<T, string> Result { get; }

        public void Match(Stream stream, IEnumerable<T> hashes, Func<T, Hash> selector)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (hashes == null) throw new ArgumentNullException(nameof(hashes));
            DoMatch(hashes, selector, stream);
        }

        public void Match(string fileName, IEnumerable<T> hashes, Func<T, Hash> selector)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (hashes == null) throw new ArgumentNullException(nameof(hashes));

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                DoMatch(hashes, selector, stream);
            }
        }
    }

}
