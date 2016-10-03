using HashUtil.Hashing;

namespace HashUtil
{
    /// <summary>
    /// Contains a found hash and it's source
    /// </summary>
    public class HashInfo
    {
        /// <summary>
        /// Create a new <see cref="HashInfo"/> instance
        /// </summary>
        /// <param name="hash">the <see cref="Hashing.Hash"/></param>
        /// <param name="source">it's source type</param>
        /// <remarks><see cref="SourceHint"/> is null</remarks>
        public HashInfo(Hash hash, HashSource source)
        {
            Source = source;
            Hash = hash;
        }

        /// <summary>
        /// Create a new <see cref="HashInfo"/> instance
        /// </summary>
        /// <param name="hash">the <see cref="Hashing.Hash"/></param>
        /// <param name="source">it's source type</param>
        /// <param name="sourceHint">a hint to the exact source</param>
        public HashInfo(Hash hash, HashSource source, string sourceHint) : this(hash, source)
        {
            SourceHint = sourceHint;
        }
        /// <summary>
        /// The <see cref="Hashing.Hash"/> found
        /// </summary>
        public Hash Hash { get; }
        /// <summary>
        /// The <see cref="HashSource"/> the <see cref="Hash"/> was found at
        /// </summary>
        public HashSource Source { get; }
        /// <summary>
        /// Contains info about the exact source of the hash
        /// </summary>
        public string SourceHint { get; }
    }
}