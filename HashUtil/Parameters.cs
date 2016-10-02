using HashUtil.Hashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashUtil
{
    public enum HashingMode
    {
        Select = 0,
        Match = 1,
        Calculate = 2,
    }

    public enum HashSource
    {
        None = 0,
        Argument = 1,
        Clipboard = 2,
        File = 3,
    }

    public class HashInfo
    {
        public HashInfo(Hash hash, HashSource source)
        {
            Source = source;
            Hash = hash;
        }

        public HashInfo(Hash hash, HashSource source, string sourceHint) : this(hash, source)
        {
            SourceHint = sourceHint;
        }

        public Hash Hash { get; }
        public HashSource Source { get; }
        public string SourceHint { get; }
    }

    public class ExecutionInfo
    {
        public ExecutionInfo()
        {
            Hashes = new List<HashInfo>();
        }

        public ExecutionInfo(IEnumerable<HashInfo> hashes) : this()
        {
            Hashes.AddRange(hashes);
        }

        public string FilePath { get; set; }
        public List<HashInfo> Hashes { get; }
        public HashingMode Mode { get; set; }
    }
}
