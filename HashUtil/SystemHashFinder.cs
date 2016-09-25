using System;
using System.Diagnostics;
using System.Windows;

namespace HashUtil
{
    static class SystemHashFinder
    {
        private static bool TryConvertToBytes(string possibleHash, out byte[] data)
        {
            try
            {
                data = HashUtils.HexToBytes(possibleHash);
                return true;
            }
            catch(Exception)
            {
                data = new byte[0];
                return false;
            }
        }

        public static bool Find(out byte[] hash, out HashSource source)
        {   
            var text = string.Empty;

            if(!string.IsNullOrEmpty(text = Clipboard.GetText(TextDataFormat.UnicodeText)))
            {
                Trace.WriteLine("Found UnicodeText in Clipboard!");
                if (TryConvertToBytes(text, out hash))
                {
                    Trace.WriteLine($"Found Hash: {text}");
                    source = HashSource.Clipbard;
                    return true;
                }
                else
                {
                    Trace.WriteLine("No valid Hash in UnicodeText!");
                }
            }
                
            if (!string.IsNullOrEmpty(text = Clipboard.GetText(TextDataFormat.Text)))
            {
                Trace.WriteLine("Found Text in Clipboard!");
                if (TryConvertToBytes(text, out hash))
                {
                    Trace.WriteLine($"Found Hash: {text}");
                    source = HashSource.Clipbard;
                    return true;
                }
                else
                {
                    Trace.WriteLine("No valid Hash in Text!");
                }
            }

            if (!string.IsNullOrEmpty(text = Clipboard.GetText(TextDataFormat.Rtf)))
            {
                Trace.WriteLine("Found RtfText in Clipboard!");
                if (TryConvertToBytes(text, out hash))
                {
                    Trace.WriteLine($"Found Hash: {text}");
                    source = HashSource.Clipbard;
                    return true;
                }
                else
                {
                    Trace.WriteLine("No valid Hash in RtfText!");
                }
            }

            hash = new byte[0];
            source = HashSource.None;
            return false;
        }
    }
}
