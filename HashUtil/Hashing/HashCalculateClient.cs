using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace HashUtil.Hashing
{
    public sealed class HashCalculateClient : HashClient
    {
        public HashCalculateClient() : base()
        {
            _result = new Dictionary<string, Hash>();
            Result = new ReadOnlyDictionary<string, Hash>(_result);
        }

        private readonly Dictionary<string, Hash> _result;
        public IReadOnlyDictionary<string, Hash> Result { get; }

        private void DoCalculate(Stream stream, IEnumerable<string> filter = null)
        {
            DoStatusChanged(HashStatus.Start);

            var algs = HashAlgorithms;
            if (filter != null)
            {
                foreach (var s in filter)
                {
                    if (!algs.Remove(s))
                    {
                        throw new FilterNameNotFoundException(s);
                    }
                }
            }

            CalculateManySinglePass(stream, algs.Select(kv => kv.Value).ToList());

            _result.Clear();
            foreach (var kv in algs)
            {
                _result.Add(kv.Key, new Hash(kv.Value.Hash));
            }

            DoStatusChanged(HashStatus.Success);
        }

        public void Calculate(Stream stream, IEnumerable<string> filter = null)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            DoCalculate(stream, filter);
        }

        public void Calculate(string fileName, IEnumerable<string> filter = null)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                DoCalculate(stream, filter);
            }
        }
    }
}
