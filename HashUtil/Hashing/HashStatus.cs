using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashUtil.Hashing
{
    /// <summary>
    /// The Status a hashing operating is currently in
    /// </summary>
    [Serializable]
    public enum HashStatus
    {
        /// <summary>
        /// preparing algorithm choices
        /// </summary>
        Start,
        /// <summary>
        /// calculating hashes from data
        /// </summary>
        Calculating,
        /// <summary>
        /// at least one hash was success
        /// </summary>
        Success,
        /// <summary>
        /// no successful hash was found
        /// </summary>
        Failure,
    }
}
