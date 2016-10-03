using System;

namespace HashUtil
{
    /// <summary>
    /// The mode the application is running in
    /// </summary>
    [Serializable]
    public enum HashingMode
    {
        /// <summary>
        /// Displays an interface to let users choose data and mode
        /// </summary>
        Select = 0,
        /// <summary>
        /// Calculates hashes that could match a list of given hashes
        /// </summary>
        Match = 1,
        /// <summary>
        /// Calculates all known hashes
        /// </summary>
        //TODO: Add support for hash subsets
        Calculate = 2,
    }
}