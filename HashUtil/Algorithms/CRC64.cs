﻿// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Originally published at http://damieng.com/blog/2007/11/19/calculating-crc-64-in-c-and-net
// Crc64Ecma Copyright (c) Mirco Gericke under the License
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace HashUtil.Algorithms
{
    /// <summary>
    /// Implements a 64-bit CRC hash algorithm for a given polynomial.
    /// </summary>
    /// <remarks>
    /// For ISO 3309 compliant 64-bit CRC's use Crc64Iso.
    /// </remarks>
    public class Crc64 : HashAlgorithm
    {
        public const ulong DefaultSeed = 0x0;

        readonly ulong[] table;

        readonly ulong seed;
        ulong hash;

        public Crc64(ulong polynomial)
            : this(polynomial, DefaultSeed)
        {
        }

        public Crc64(ulong polynomial, ulong seed)
        {
            table = InitializeTable(polynomial);
            this.seed = hash = seed;
        }

        public override void Initialize()
        {
            hash = seed;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            hash = CalculateHash(hash, table, array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt64ToBigEndianBytes(hash);
            HashValue = hashBuffer;
            return hashBuffer;
        }

        public override int HashSize { get { return 64; } }

        protected static ulong CalculateHash(ulong seed, ulong[] table, IList<byte> buffer, int start, int size)
        {
            var hash = seed;
            for (var i = start; i < start + size; i++)
                unchecked
                {
                    hash = (hash >> 8) ^ table[(buffer[i] ^ hash) & 0xff];
                }
            return hash;
        }

        static byte[] UInt64ToBigEndianBytes(ulong value)
        {
            var result = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        static ulong[] InitializeTable(ulong polynomial)
        {
            if (polynomial == Crc64Iso.Iso3309Polynomial && Crc64Iso.Table != null)
                return Crc64Iso.Table;

            var createTable = CreateTable(polynomial);

            if (polynomial == Crc64Iso.Iso3309Polynomial)
                Crc64Iso.Table = createTable;

            return createTable;
        }

        protected static ulong[] CreateTable(ulong polynomial)
        {
            var createTable = new ulong[256];
            for (var i = 0; i < 256; ++i)
            {
                var entry = (ulong)i;
                for (var j = 0; j < 8; ++j)
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry = entry >> 1;
                createTable[i] = entry;
            }
            return createTable;
        }
    }

    public class Crc64Iso : Crc64
    {
        internal static ulong[] Table;

        public const ulong Iso3309Polynomial = 0xD800000000000000;

        public Crc64Iso()
            : base(Iso3309Polynomial)
        {
        }

        public Crc64Iso(ulong seed)
            : base(Iso3309Polynomial, seed)
        {
        }

        public static ulong Compute(byte[] buffer)
        {
            return Compute(DefaultSeed, buffer);
        }

        public static ulong Compute(ulong seed, byte[] buffer)
        {
            if (Table == null)
                Table = CreateTable(Iso3309Polynomial);

            return CalculateHash(seed, Table, buffer, 0, buffer.Length);
        }

        public static new Crc64Iso Create()
        {
            return new Crc64Iso();
        }
    }

    public class Crc64Ecma : Crc64
    {
        internal static ulong[] Table;

        public const ulong EcmaPolynomial = 0xC96C5795D7870F42;

        public Crc64Ecma()
            : base(EcmaPolynomial)
        {
        }

        public Crc64Ecma(ulong seed)
            : base(EcmaPolynomial, seed)
        {
        }

        public static ulong Compute(byte[] buffer)
        {
            return Compute(DefaultSeed, buffer);
        }

        public static ulong Compute(ulong seed, byte[] buffer)
        {
            if (Table == null)
                Table = CreateTable(EcmaPolynomial);

            return CalculateHash(seed, Table, buffer, 0, buffer.Length);
        }

        public static new Crc64Ecma Create()
        {
            return new Crc64Ecma();
        }
    }
}
