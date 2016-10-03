using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using HashUtil.Graphical;

namespace HashUtil.Hashing
{
    public static class SystemHashFinder
    {
        /// <summary>
        /// Initializes the list of file extensions
        /// </summary>
        static SystemHashFinder()
        {
            Extensions = new[] {"sha1", "sha224", "sha256", "sha384", "sha512", "sha3", "md5", "crc", "sha", "checksum", "hash"}
                .Select(s => "." + s)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// List of file extensions that may contain hashes
        /// </summary>
        private static readonly IReadOnlyList<string> Extensions;

        /// <summary>
        /// Reads a checksum formatted line's hash and returns it
        /// </summary>
        /// <param name="singleLine">a single line string in checksum format</param>
        /// <returns><see cref="Hash"/> instance if the line was valid, null otherwise</returns>
        private static Hash HashFromChecksumLine(string singleLine)
        {
            return TryGetHash(singleLine
                .Split(new[] {" *"}, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault()
            );
        }
        /// <summary>
        /// Reads a checksum formatted string and returns each hash in it
        /// </summary>
        /// <param name="multilineString">the checksum formatted string</param>
        /// <returns>a <see cref="List{T}"/> containing all found hashes</returns>
        private static List<Hash> HashFromChecksumContent(string multilineString)
        {
            return multilineString
                .Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(HashFromChecksumLine)
                .Where(h => h!= null)
                .ToList();
        }

        /// <summary>
        /// tries to find hashes in all whitespace seperated parts of the string
        /// </summary>
        /// <param name="value">the string to find hashes in</param>
        /// <returns>an <see cref="IEnumerable{T}"/> that enumerates all found hashes</returns>
        private static IEnumerable<Hash> HashFromWords(string value)
        {
            return value
                .Split()
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => TryGetHash(s.Trim()))
                .Where(h => h != null)
                .ToList();
        }

/*
        private static IEnumerable<Hash> FindFromLines(string multilineString)
        {
            var result = HashFromChecksumContent(multilineString);
            return result.Count > 0 ? result : HashFromWords(multilineString);
        }
*/

        /// <summary>
        /// Converts a string to a hash
        /// </summary>
        /// <param name="possibleHash">the string to convert</param>
        /// <returns>a <see cref="Hash"/> if the parameter was valid, <c>null</c> otherwise</returns>
        public static Hash TryGetHash(string possibleHash)
        {
            try
            {
                return new Hash(possibleHash);
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Finds all hashes in the system clipboard
        /// </summary>
        /// <returns><see cref="List{T}"/> of found hashes</returns>
        public static List<HashInfo> FindFromClipboard()
        {
            Hash hash;
            string text;

            var results = new List<HashInfo>();

            if (!string.IsNullOrEmpty(text = Clipboard.GetText(TextDataFormat.UnicodeText)))
            {
                if ((hash = TryGetHash(text)) != null)
                {
                    results.Add(new HashInfo(hash, HashSource.Clipboard));
                }
                else
                {
                    results.AddRange(HashFromWords(text).Select(h => new HashInfo(h, HashSource.Clipboard)));
                }
            }
            else if (!string.IsNullOrEmpty(text = Clipboard.GetText(TextDataFormat.Text)))
            {
                if ((hash = TryGetHash(text)) != null)
                {
                    results.Add(new HashInfo(hash, HashSource.Clipboard));
                }
                else
                {
                    results.AddRange(HashFromWords(text).Select(h => new HashInfo(h, HashSource.Clipboard)));
                }
            }
            return results;
        }
        
        /// <summary>
        /// Finds hashes in files that are likely to contain hash data if they exist
        /// </summary>
        /// <param name="fileName">the file to generate permutations from</param>
        /// <returns>an <see cref="IEnumerable{T}"/> the enumerates all found hashes</returns>
        public static IEnumerable<HashInfo> FindFromFiles(string fileName)
        {
            //TODO: add file searching for hashes
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds all hashes in a single file if it exists
        /// </summary>
        /// <param name="fileName">the file to read from</param>
        /// <returns>a <see cref="List{T}"/> of hashes in the file</returns>
        public static List<HashInfo> ReadFromFile(string fileName)
        {
            //TODO: add searching in a single file
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds all hashes for <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName">the file to hash later</param>
        /// <returns>a <see cref="List{T}"/> of all found hashes</returns>
        public static List<HashInfo> FindAll(string fileName)
        {
            var result = new List<HashInfo>();

            result.AddRange(FindFromClipboard());

            if (!string.IsNullOrEmpty(fileName))
            {
                result.AddRange(FindFromFiles(fileName));
            }
            return result;
        }
    }
}
