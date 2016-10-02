//Copyright © Joe Dluzen 2012
//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//
//2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
//   following disclaimer in the documentation and/or other materials provided with the distribution.
//
//3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote
//   products derived from this software without specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
//USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace HashUtil.Hashing.Algorithms
{
    public abstract class Sha3 : HashAlgorithm
    {
        #region Statics
        public static string DefaultHashName = "SHA512";

        protected static Dictionary<string, Func<Sha3>> HashNameMap;

        static Sha3()
        {
            HashNameMap = new Dictionary<string, Func<Sha3>>
            {
                //{ "SHA3-224", () => { return new SHA3224(); } }

            };
        }

        public new static Sha3 Create()
        {
            return Create(DefaultHashName);
        }

        public new static Sha3 Create(string hashName)
        {
            Func<Sha3> ctor;
            if (HashNameMap.TryGetValue(hashName, out ctor))
                return ctor();
            return null;
        }
        #endregion

        #region Implementation
        public const int KeccakB = 1600;
        public const int KeccakNumberOfRounds = 24;
        public const int KeccakLaneSizeInBits = 8 * 8;

        public readonly ulong[] RoundConstants;


        // ReSharper disable once InconsistentNaming (treated as private)
        protected ulong[] _State;
        protected byte[] Buffer;
        protected int BuffLength;

        // ReSharper disable once InconsistentNaming (treated as private)
        protected int _KeccakR;

        public int KeccakR
        {
            get
            {
                return _KeccakR;
            }
            protected set
            {
                _KeccakR = value;
            }
        }

        public int SizeInBytes => KeccakR / 8;

        public int HashByteLength => HashSizeValue / 8;

        public override bool CanReuseTransform => true;

        protected Sha3(int hashBitLength)
        {
            if (hashBitLength != 224 && hashBitLength != 256 && hashBitLength != 384 && hashBitLength != 512)
                throw new ArgumentException("hashBitLength must be 224, 256, 384, or 512", nameof(hashBitLength));
            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();
            HashSizeValue = hashBitLength;
            switch (hashBitLength)
            {
                case 224:
                    KeccakR = 1152;
                    break;
                case 256:
                    KeccakR = 1088;
                    break;
                case 384:
                    KeccakR = 832;
                    break;
                case 512:
                    KeccakR = 576;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hashBitLength), hashBitLength, "Should be 224, 256,384 or 512");
            }
            RoundConstants = new ulong[]
            {
                0x0000000000000001UL,
                0x0000000000008082UL,
                0x800000000000808aUL,
                0x8000000080008000UL,
                0x000000000000808bUL,
                0x0000000080000001UL,
                0x8000000080008081UL,
                0x8000000000008009UL,
                0x000000000000008aUL,
                0x0000000000000088UL,
                0x0000000080008009UL,
                0x000000008000000aUL,
                0x000000008000808bUL,
                0x800000000000008bUL,
                0x8000000000008089UL,
                0x8000000000008003UL,
                0x8000000000008002UL,
                0x8000000000000080UL,
                0x000000000000800aUL,
                0x800000008000000aUL,
                0x8000000080008081UL,
                0x8000000000008080UL,
                0x0000000080000001UL,
                0x8000000080008008UL
            };
        }

        protected ulong Rol(ulong a, int offset)
        {
            return (((a) << ((offset) % KeccakLaneSizeInBits)) ^ ((a) >> (KeccakLaneSizeInBits - ((offset) % KeccakLaneSizeInBits))));
        }

        protected void AddToBuffer(byte[] array, ref int offset, ref int count)
        {
            var amount = Math.Min(count, Buffer.Length - BuffLength);
            System.Buffer.BlockCopy(array, offset, Buffer, BuffLength, amount);
            offset += amount;
            BuffLength += amount;
            count -= amount;
        }

        public override byte[] Hash => HashValue;

        public override int HashSize => HashSizeValue;

        #endregion

        public override void Initialize()
        {
            BuffLength = 0;
            _State = new ulong[5 * 5];//1600 bits
            HashValue = null;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (ibStart < 0)
                throw new ArgumentOutOfRangeException(nameof(ibStart));
            if (cbSize > array.Length)
                throw new ArgumentOutOfRangeException(nameof(cbSize));
            if (ibStart + cbSize > array.Length)
                // ReSharper disable once NotResolvedInText (impossible to determine)
                throw new ArgumentOutOfRangeException("ibStart or cbSize");
        }
    }

    [SuppressMessage("ReSharper", "TooWideLocalVariableScope")]
    [SuppressMessage("ReSharper", "JoinDeclarationAndInitializer")]
    public partial class Sha3Managed : Sha3
    {
        public Sha3Managed(int hashBitLength)
            : base(hashBitLength)
        {
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            base.HashCore(array, ibStart, cbSize);
            if (cbSize == 0)
                return;
            var sizeInBytes = SizeInBytes;
            if (Buffer == null)
                Buffer = new byte[sizeInBytes];
            var stride = sizeInBytes >> 3;
            var utemps = new ulong[stride];
            if (BuffLength == sizeInBytes)
                throw new Exception("Unexpected error, the internal buffer is full");
            AddToBuffer(array, ref ibStart, ref cbSize);
            if (BuffLength == sizeInBytes)//buffer full
            {
                System.Buffer.BlockCopy(Buffer, 0, utemps, 0, sizeInBytes);
                KeccakF(utemps, stride);
                BuffLength = 0;
            }
            for (; cbSize >= sizeInBytes; cbSize -= sizeInBytes, ibStart += sizeInBytes)
            {
                System.Buffer.BlockCopy(array, ibStart, utemps, 0, sizeInBytes);
                KeccakF(utemps, stride);
            }
            if (cbSize > 0)//some left over
            {
                System.Buffer.BlockCopy(array, ibStart, Buffer, BuffLength, cbSize);
                BuffLength += cbSize;
            }
        }

        protected override byte[] HashFinal()
        {
            var sizeInBytes = SizeInBytes;
            var outb = new byte[HashByteLength];
            //    padding
            if (Buffer == null)
                Buffer = new byte[sizeInBytes];
            else
                Array.Clear(Buffer, BuffLength, sizeInBytes - BuffLength);
            Buffer[BuffLength++] = 1;
            Buffer[sizeInBytes - 1] |= 0x80;
            var stride = sizeInBytes >> 3;
            var utemps = new ulong[stride];
            System.Buffer.BlockCopy(Buffer, 0, utemps, 0, sizeInBytes);
            KeccakF(utemps, stride);
            System.Buffer.BlockCopy(_State, 0, outb, 0, HashByteLength);
            return outb;
        }

        // ReSharper disable once SuggestBaseTypeForParameter (performance optimization)
        private void KeccakF(ulong[] inb, int laneCount)
        {
            while (--laneCount >= 0)
                _State[laneCount] ^= inb[laneCount];
            ulong aba, abe, abi, abo, abu;
            ulong aga, age, agi, ago, agu;
            ulong aka, ake, aki, ako, aku;
            ulong ama, ame, ami, amo, amu;
            ulong asa, ase, asi, aso, asu;
            ulong bCa, bCe, bCi, bCo, bCu;
            ulong da, de, di, Do, du;
            ulong eba, ebe, ebi, ebo, ebu;
            ulong ega, ege, egi, ego, egu;
            ulong eka, eke, eki, eko, eku;
            ulong ema, eme, emi, emo, emu;
            ulong esa, ese, esi, eso, esu;
            var round = laneCount;

            //copyFromState(A, _State)
            aba = _State[0];
            abe = _State[1];
            abi = _State[2];
            abo = _State[3];
            abu = _State[4];
            aga = _State[5];
            age = _State[6];
            agi = _State[7];
            ago = _State[8];
            agu = _State[9];
            aka = _State[10];
            ake = _State[11];
            aki = _State[12];
            ako = _State[13];
            aku = _State[14];
            ama = _State[15];
            ame = _State[16];
            ami = _State[17];
            amo = _State[18];
            amu = _State[19];
            asa = _State[20];
            ase = _State[21];
            asi = _State[22];
            aso = _State[23];
            asu = _State[24];

            for (round = 0; round < KeccakNumberOfRounds; round += 2)
            {
                //    prepareTheta
                bCa = aba ^ aga ^ aka ^ ama ^ asa;
                bCe = abe ^ age ^ ake ^ ame ^ ase;
                bCi = abi ^ agi ^ aki ^ ami ^ asi;
                bCo = abo ^ ago ^ ako ^ amo ^ aso;
                bCu = abu ^ agu ^ aku ^ amu ^ asu;

                //thetaRhoPiChiIotaPrepareTheta(round  , A, E)
                da = bCu ^ Rol(bCe, 1);
                de = bCa ^ Rol(bCi, 1);
                di = bCe ^ Rol(bCo, 1);
                Do = bCi ^ Rol(bCu, 1);
                du = bCo ^ Rol(bCa, 1);

                aba ^= da;
                bCa = aba;
                age ^= de;
                bCe = Rol(age, 44);
                aki ^= di;
                bCi = Rol(aki, 43);
                amo ^= Do;
                bCo = Rol(amo, 21);
                asu ^= du;
                bCu = Rol(asu, 14);
                eba = bCa ^ ((~bCe) & bCi);
                eba ^= RoundConstants[round];
                ebe = bCe ^ ((~bCi) & bCo);
                ebi = bCi ^ ((~bCo) & bCu);
                ebo = bCo ^ ((~bCu) & bCa);
                ebu = bCu ^ ((~bCa) & bCe);

                abo ^= Do;
                bCa = Rol(abo, 28);
                agu ^= du;
                bCe = Rol(agu, 20);
                aka ^= da;
                bCi = Rol(aka, 3);
                ame ^= de;
                bCo = Rol(ame, 45);
                asi ^= di;
                bCu = Rol(asi, 61);
                ega = bCa ^ ((~bCe) & bCi);
                ege = bCe ^ ((~bCi) & bCo);
                egi = bCi ^ ((~bCo) & bCu);
                ego = bCo ^ ((~bCu) & bCa);
                egu = bCu ^ ((~bCa) & bCe);

                abe ^= de;
                bCa = Rol(abe, 1);
                agi ^= di;
                bCe = Rol(agi, 6);
                ako ^= Do;
                bCi = Rol(ako, 25);
                amu ^= du;
                bCo = Rol(amu, 8);
                asa ^= da;
                bCu = Rol(asa, 18);
                eka = bCa ^ ((~bCe) & bCi);
                eke = bCe ^ ((~bCi) & bCo);
                eki = bCi ^ ((~bCo) & bCu);
                eko = bCo ^ ((~bCu) & bCa);
                eku = bCu ^ ((~bCa) & bCe);

                abu ^= du;
                bCa = Rol(abu, 27);
                aga ^= da;
                bCe = Rol(aga, 36);
                ake ^= de;
                bCi = Rol(ake, 10);
                ami ^= di;
                bCo = Rol(ami, 15);
                aso ^= Do;
                bCu = Rol(aso, 56);
                ema = bCa ^ ((~bCe) & bCi);
                eme = bCe ^ ((~bCi) & bCo);
                emi = bCi ^ ((~bCo) & bCu);
                emo = bCo ^ ((~bCu) & bCa);
                emu = bCu ^ ((~bCa) & bCe);

                abi ^= di;
                bCa = Rol(abi, 62);
                ago ^= Do;
                bCe = Rol(ago, 55);
                aku ^= du;
                bCi = Rol(aku, 39);
                ama ^= da;
                bCo = Rol(ama, 41);
                ase ^= de;
                bCu = Rol(ase, 2);
                esa = bCa ^ ((~bCe) & bCi);
                ese = bCe ^ ((~bCi) & bCo);
                esi = bCi ^ ((~bCo) & bCu);
                eso = bCo ^ ((~bCu) & bCa);
                esu = bCu ^ ((~bCa) & bCe);

                //    prepareTheta
                bCa = eba ^ ega ^ eka ^ ema ^ esa;
                bCe = ebe ^ ege ^ eke ^ eme ^ ese;
                bCi = ebi ^ egi ^ eki ^ emi ^ esi;
                bCo = ebo ^ ego ^ eko ^ emo ^ eso;
                bCu = ebu ^ egu ^ eku ^ emu ^ esu;

                //thetaRhoPiChiIotaPrepareTheta(round+1, E, A)
                da = bCu ^ Rol(bCe, 1);
                de = bCa ^ Rol(bCi, 1);
                di = bCe ^ Rol(bCo, 1);
                Do = bCi ^ Rol(bCu, 1);
                du = bCo ^ Rol(bCa, 1);

                eba ^= da;
                bCa = eba;
                ege ^= de;
                bCe = Rol(ege, 44);
                eki ^= di;
                bCi = Rol(eki, 43);
                emo ^= Do;
                bCo = Rol(emo, 21);
                esu ^= du;
                bCu = Rol(esu, 14);
                aba = bCa ^ ((~bCe) & bCi);
                aba ^= RoundConstants[round + 1];
                abe = bCe ^ ((~bCi) & bCo);
                abi = bCi ^ ((~bCo) & bCu);
                abo = bCo ^ ((~bCu) & bCa);
                abu = bCu ^ ((~bCa) & bCe);

                ebo ^= Do;
                bCa = Rol(ebo, 28);
                egu ^= du;
                bCe = Rol(egu, 20);
                eka ^= da;
                bCi = Rol(eka, 3);
                eme ^= de;
                bCo = Rol(eme, 45);
                esi ^= di;
                bCu = Rol(esi, 61);
                aga = bCa ^ ((~bCe) & bCi);
                age = bCe ^ ((~bCi) & bCo);
                agi = bCi ^ ((~bCo) & bCu);
                ago = bCo ^ ((~bCu) & bCa);
                agu = bCu ^ ((~bCa) & bCe);

                ebe ^= de;
                bCa = Rol(ebe, 1);
                egi ^= di;
                bCe = Rol(egi, 6);
                eko ^= Do;
                bCi = Rol(eko, 25);
                emu ^= du;
                bCo = Rol(emu, 8);
                esa ^= da;
                bCu = Rol(esa, 18);
                aka = bCa ^ ((~bCe) & bCi);
                ake = bCe ^ ((~bCi) & bCo);
                aki = bCi ^ ((~bCo) & bCu);
                ako = bCo ^ ((~bCu) & bCa);
                aku = bCu ^ ((~bCa) & bCe);

                ebu ^= du;
                bCa = Rol(ebu, 27);
                ega ^= da;
                bCe = Rol(ega, 36);
                eke ^= de;
                bCi = Rol(eke, 10);
                emi ^= di;
                bCo = Rol(emi, 15);
                eso ^= Do;
                bCu = Rol(eso, 56);
                ama = bCa ^ ((~bCe) & bCi);
                ame = bCe ^ ((~bCi) & bCo);
                ami = bCi ^ ((~bCo) & bCu);
                amo = bCo ^ ((~bCu) & bCa);
                amu = bCu ^ ((~bCa) & bCe);

                ebi ^= di;
                bCa = Rol(ebi, 62);
                ego ^= Do;
                bCe = Rol(ego, 55);
                eku ^= du;
                bCi = Rol(eku, 39);
                ema ^= da;
                bCo = Rol(ema, 41);
                ese ^= de;
                bCu = Rol(ese, 2);
                asa = bCa ^ ((~bCe) & bCi);
                ase = bCe ^ ((~bCi) & bCo);
                asi = bCi ^ ((~bCo) & bCu);
                aso = bCo ^ ((~bCu) & bCa);
                asu = bCu ^ ((~bCa) & bCe);
            }

            //copyToState(_State, A)
            _State[0] = aba;
            _State[1] = abe;
            _State[2] = abi;
            _State[3] = abo;
            _State[4] = abu;
            _State[5] = aga;
            _State[6] = age;
            _State[7] = agi;
            _State[8] = ago;
            _State[9] = agu;
            _State[10] = aka;
            _State[11] = ake;
            _State[12] = aki;
            _State[13] = ako;
            _State[14] = aku;
            _State[15] = ama;
            _State[16] = ame;
            _State[17] = ami;
            _State[18] = amo;
            _State[19] = amu;
            _State[20] = asa;
            _State[21] = ase;
            _State[22] = asi;
            _State[23] = aso;
            _State[24] = asu;

        }
    }
}
