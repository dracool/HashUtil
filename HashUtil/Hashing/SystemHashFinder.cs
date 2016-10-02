using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace HashUtil.Hashing
{
    public static class SystemHashFinder
    {
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
        
        private static IEnumerable<Hash> HashFromWords(string multilineString)
        {
            var words = multilineString.Split();
            return words
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => TryGetHash(s.Trim()))
                .Where(h => h != null)
                .ToList();
        }

        public static List<HashInfo> FindFromClipboard()
        {
            Hash hash;
            var text = string.Empty;

            List<HashInfo> results = new List<HashInfo>();

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

        private static string[] _extensions = new string[] { ".sha", ".md5", ".hash", ".crc", ".checksum" };

        public static List<HashInfo> FindFromFiles(string fileName)
        {
            return new List<HashInfo>();
        }

        public static List<HashInfo> ReadFromFile(string fileName)
        {
            throw new NotImplementedException();
        }

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
