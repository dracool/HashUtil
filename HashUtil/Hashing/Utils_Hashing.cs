using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashUtil.Hashing
{
    /// <summary>
    /// Contains methods to convert between <c>byte[]</c> and <c>string</c>
    /// </summary>
    internal static class HashingUtils
    {
        /// <summary>
        /// Converts this byte array to a lower case hex string
        /// </summary>
        /// <param name="ba">the byte array to convert</param>
        /// <returns>lower case hex string</returns>
        public static string ToHex(this byte[] ba)
        {
            return BytesToHex(ba);
        }

        /// <summary>
        /// Converts a byte array to a lower case hex string
        /// </summary>
        /// <param name="ba">the byte array to convert</param>
        /// <returns>lower case hex string</returns>
        public static string BytesToHex(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (var b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        /// <summary>
        /// Converts a <c>string </c>containing hex values to <c>byte[]</c>
        /// </summary>
        /// <param name="hex">the string containing only hex values</param>
        /// <returns>the byte[]</returns>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="hex"/> is null</exception>
        /// <exception cref="ArgumentException">thrown if <paramref name="hex"/>is empty or contains only whitespaces</exception>
        /// <exception cref="FormatException">thrown if <paramref name="hex"/> does not contain a hex string</exception>
        public static byte[] HexToBytes(string hex)
        {
            if (hex == null)
                throw new ArgumentNullException(nameof(hex));
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException(@"Value cannot be empty or whitespace.", nameof(hex));

            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
