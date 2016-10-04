using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HashUtil.Hashing
{
    /// <summary>
    /// Represents a hash value in byte and string form
    /// </summary>
    [Serializable]
    public class Hash : IEquatable<Hash>, IEquatable<string>, IEquatable<byte[]>, ICloneable, IEnumerable<byte>, IEnumerable<char>
    {
        #region constructors
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="hash"></param>
        public Hash(string hash)
        {
            Bytes = HashingUtils.HexToBytes(hash);
            Hex = hash;
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="hash">the hash to use</param>
        public Hash(byte[] hash)
        {
            Hex = HashingUtils.BytesToHex(hash);
            Bytes = (byte[])hash.Clone();
        }
        #endregion

        #region operators: conversion
        /// <summary>
        /// converts the value to string
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator string(Hash value)
        {
            return value?.Hex ?? string.Empty;
        }

        /// <summary>
        /// converts the value to byte[]
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator byte[] (Hash value)
        {
            return value?.Bytes;
        }
        #endregion

        #region operators: comparison(Hash)
        /// <summary>
        /// compares to <see cref="Hash"/>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Hash left, Hash right)
        {
            var val = left?.Equals(right) ?? object.Equals(right, null);
            return val;
        }

        /// <summary>
        /// compares to <see cref="Hash"/>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Hash left, Hash right)
        {
            return !(left == right);
        }
        #endregion

        #region operators: comparison(string)
        /// <summary>
        /// compares to string
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Hash left, string right)
        {
            return left?.Equals(right) ?? right == null;
        }

        /// <summary>
        /// compares to string
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Hash left, string right)
        {
            return !(left?.Equals(right) ?? right == null);
        }

        /// <summary>
        /// compares to string
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(string left, Hash right)
        {
            return right?.Equals(left) ?? left == null;
        }

        /// <summary>
        /// compares to string
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(string left, Hash right)
        {
            return !(right?.Equals(left) ?? left == null);
        }
        #endregion

        #region operators: comparison(byte[])
        /// <summary>
        /// compares to bytes
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Hash left, byte[] right)
        {
            return left?.Equals(right) ?? right == null;
        }

        /// <summary>
        /// compares to bytes
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Hash left, byte[] right)
        {
            return !(left?.Equals(right) ?? right == null);
        }

        /// <summary>
        /// compares to bytes
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(byte[] left, Hash right)
        {
            return right?.Equals(left) ?? left == null;
        }
        /// <summary>
        /// compares to bytes
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(byte[] left, Hash right)
        {
            return !(right?.Equals(left) ?? left == null);
        }
        #endregion

        #region properties
        
        /// <summary>
        /// representation of the hash as lowercase string
        /// </summary>
        public string Hex { get; }
        
        /// <summary>
        /// representation of the hash as bytes
        /// </summary>
        public byte[] Bytes { get; }

        /// <summary>
        /// Length of the hash in bit
        /// </summary>
        public int Length => Bytes.Length * 8;
        #endregion

        #region Object overrides
        /// <summary>
        /// returns the string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Hex;
        }

        /// <summary>
        /// returns the hash representation
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Hex.GetHashCode();
        }

        /// <summary>
        /// equality comparison
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Hash) 
                || Equals(obj as string) 
                || Equals(obj as byte[]);
        }
        #endregion

        #region IEquatable<*>
        /// <summary>
        /// Compares the equality of this hash with another
        /// </summary>
        /// <param name="other">the Hash to compare to</param>
        /// <returns>true if the hashes are equal, false otherwise</returns>
        public bool Equals(Hash other)
        {
            return Equals(other?.Bytes);
        }

        /// <summary>
        /// Compares the equality of string representation with another string
        /// </summary>
        /// <param name="other">the string to compare to</param>
        /// <returns>true if the strings are equal, false otherwise</returns>
        public bool Equals(string other)
        {
            return Hex?.Equals(other, StringComparison.OrdinalIgnoreCase) ?? false;
        }

        /// <summary>
        /// Compares the equality of the bytes with a byte array
        /// </summary>
        /// <param name="other">the byte array to compare against</param>
        /// <returns>true if the byte sequence is equal, false otherwise</returns>
        public bool Equals(byte[] other)
        {
            return other?.SequenceEqual(Bytes) ?? false;
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Returns a copy of the hash
        /// </summary>
        /// <returns></returns>
        public Hash Clone()
        {
            return new Hash(Bytes);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region IEnumerable
        /// <summary>
        /// Returns a <see cref="IEnumerator{T}"/> that enumerates over the bytes of the hash
        /// </summary>
        /// <remarks>contains an explicit definition for char enumerables</remarks>
        public IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)Bytes).GetEnumerator();
        }

        IEnumerator<char> IEnumerable<char>.GetEnumerator()
        {
            return ((IEnumerable<char>)Hex).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<byte>)Bytes).GetEnumerator();
        }
        #endregion
    }
}
