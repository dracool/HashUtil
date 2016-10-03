using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashUtil
{
    /// <summary>
    /// Contains all data necesary to run the application
    /// </summary>
    public class ExecutionInfo
    {
        /// <summary>
        /// Creates a new <see cref="ExecutionInfo"/>
        /// </summary>
        public ExecutionInfo()
        {
            Hashes = new List<HashInfo>();
        }
        /// <summary>
        /// Creates a new <see cref="ExecutionInfo"/> and initializes the Hashes
        /// </summary>
        /// <param name="hashes">the hashes to initialize the object with</param>
        public ExecutionInfo(IEnumerable<HashInfo> hashes) : this()
        {
            Hashes.AddRange(hashes);
        }

        /// <summary>
        /// The full FilePath of the file to hash
        /// </summary>
        /// <remarks>This value is ignored if <see cref="Mode"/> is <see cref="HashingMode.Select"/></remarks>
        public string FilePath { get; set; }
        
        /// <summary>
        /// List of <see cref="HashInfo"/> to compare against
        /// </summary>
        /// <remarks>This value is ignored if <see cref="Mode"/> is not <see cref="HashingMode.Match"/></remarks>
        public List<HashInfo> Hashes { get; }
        
        /// <summary>
        /// The mode the application should run in
        /// </summary>
        public HashingMode Mode { get; set; }
    }
}
