using System;

namespace HashUtil
{
    /// <summary>
    /// The source a hash was found at
    /// </summary>
    [Serializable]
    public enum HashSource
    {
        /// <summary>
        /// The hash was supplied by command line argument
        /// </summary>
        Argument = 0,
        /// <summary>
        /// The hash was found in the system clipboard
        /// </summary>
        Clipboard = 1,
        /// <summary>
        /// The hash was found in a file
        /// </summary>
        /// <remarks>
        /// When this source is set <see cref="HashInfo.SourceHint"/>
        /// contains the full file path of the file it was found in
        /// </remarks>
        File = 2,
    }
}