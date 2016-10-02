using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HashUtil.Hashing
{
    [Serializable]
    public class Hash : IEquatable<Hash>, IEquatable<string>, IEquatable<byte[]>, ICloneable, IEnumerable<byte>, IEnumerable<char>
    {
        #region constructors
        public Hash(string hash)
        {
            Bytes = HashingUtils.HexToBytes(hash);
            Hex = hash;
        }

        public Hash(byte[] hash)
        {
            Hex = HashingUtils.BytesToHex(hash);
            Bytes = (byte[])hash.Clone();
        }
        #endregion

        #region operators: conversion
        public static implicit operator string(Hash value)
        {
            return value?.Hex ?? string.Empty;
        }

        public static implicit operator byte[] (Hash value)
        {
            return value?.Bytes;
        }
        #endregion

        #region operators: comparison(Hash)
        public static bool operator ==(Hash left, Hash right)
        {
            var val = left?.Equals(right) ?? object.Equals(right, null);
            return val;
        }

        public static bool operator !=(Hash left, Hash right)
        {
            return !(left == right);
        }
        #endregion

        #region operators: comparison(string)
        public static bool operator ==(Hash left, string right)
        {
            return left?.Equals(right) ?? right == null;
        }

        public static bool operator !=(Hash left, string right)
        {
            return !(left?.Equals(right) ?? right == null);
        }

        public static bool operator ==(string left, Hash right)
        {
            return right?.Equals(left) ?? left == null;
        }

        public static bool operator !=(string left, Hash right)
        {
            return !(right?.Equals(left) ?? left == null);
        }
        #endregion

        #region operators: comparison(byte[])
        public static bool operator ==(Hash left, byte[] right)
        {
            return left?.Equals(right) ?? right == null;
        }

        public static bool operator !=(Hash left, byte[] right)
        {
            return !(left?.Equals(right) ?? right == null);
        }

        public static bool operator ==(byte[] left, Hash right)
        {
            return right?.Equals(left) ?? left == null;
        }

        public static bool operator !=(byte[] left, Hash right)
        {
            return !(right?.Equals(left) ?? left == null);
        }
        #endregion

        #region properties
        public string Hex { get; }
        public byte[] Bytes { get; }

        public int Length => Bytes.Length * 8;
        #endregion

        #region Object overrides
        public override string ToString()
        {
            return Hex;
        }

        public override int GetHashCode()
        {
            return Hex.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Hash);
        }
        #endregion

        #region IEquatable<*>
        public bool Equals(Hash other)
        {
            return Equals(other?.Bytes);
        }

        public bool Equals(string other)
        {
            return Hex?.Equals(other) ?? false;
        }

        public bool Equals(byte[] other)
        {
            return other?.SequenceEqual(Bytes) ?? false;
        }
        #endregion

        #region ICloneable
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
