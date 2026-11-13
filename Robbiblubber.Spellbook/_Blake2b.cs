namespace Robbiblubber.Spellbook;

using System.Buffers.Binary;
using System.Numerics;



/// <summary>This class implements the BLAKE2b hash function (RFC 7693) without key, as required by Argon2.</summary>
internal sealed class _Blake2b
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Initialization vector.</summary>
	private static readonly ulong[] _IV =
	[
		0x6a09e667f3bcc908UL, 0xbb67ae8584caa73bUL, 0x3c6ef372fe94f82bUL, 0xa54ff53a5f1d36f1UL,
		0x510e527fade682d1UL, 0x9b05688c2b3e6c1fUL, 0x1f83d9abfb41bd6bUL, 0x5be0cd19137e2179UL
	];


	/// <summary>Message word permutations.</summary>
	private static readonly byte[][] _SIGMA =
	[
		[  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 ],
		[ 14, 10,  4,  8,  9, 15, 13,  6,  1, 12,  0,  2, 11,  7,  5,  3 ],
		[ 11,  8, 12,  0,  5,  2, 15, 13, 10, 14,  3,  6,  7,  1,  9,  4 ],
		[  7,  9,  3,  1, 13, 12, 11, 14,  2,  6,  5, 10,  4,  0, 15,  8 ],
		[  9,  0,  5,  7,  2,  4, 10, 15, 14,  1, 11, 12,  6,  8,  3, 13 ],
		[  2, 12,  6, 10,  0, 11,  8,  3,  4, 13,  7,  5, 15, 14,  1,  9 ],
		[ 12,  5,  1, 15, 14, 13,  4, 10,  0,  7,  6,  3,  9,  2,  8, 11 ],
		[ 13, 11,  7, 14, 12,  1,  3,  9,  5,  0, 15,  4,  8,  6,  2, 10 ],
		[  6, 15, 14,  9, 11,  3,  0,  8, 12,  2, 13,  7,  1,  4, 10,  5 ],
		[ 10,  2,  8,  4,  7,  6,  1,  5, 15, 11,  9, 14,  3, 12, 13,  0 ]
	];



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>State.</summary>
	private readonly ulong[] _H = new ulong[8];

	/// <summary>Input buffer.</summary>
	private readonly byte[] _Buffer = new byte[128];

	/// <summary>Message words.</summary>
	private readonly ulong[] _M = new ulong[16];

	/// <summary>Working vector.</summary>
	private readonly ulong[] _V = new ulong[16];

	/// <summary>Number of bytes in the buffer.</summary>
	private int _Count;

	/// <summary>Number of bytes processed.</summary>
	private ulong _Total;

	/// <summary>Output length in bytes.</summary>
	private readonly int _OutputLength;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="outputLength">Output length in bytes; from 1 to 64.</param>
	internal _Blake2b(int outputLength)
	{
		_OutputLength = outputLength;

		Array.Copy(_IV, _H, 8);
		_H[0] ^= 0x01010000UL ^ (ulong) outputLength;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal static methods                                                                                          //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Hashes data.</summary>
	/// <param name="outputLength">Output length in bytes; from 1 to 64.</param>
	/// <param name="data">Data.</param>
	/// <returns>Returns the hash.</returns>
	internal static byte[] _Hash(int outputLength, ReadOnlySpan<byte> data)
	{
		_Blake2b rval = new(outputLength);
		rval._Update(data);

		return rval._Final();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Compresses the buffer.</summary>
	/// <param name="last">Determines if this is the last block.</param>
	private void _Compress(bool last)
	{
		ulong[] m = _M;
		ulong[] v = _V;

		for(int i = 0; i < 16; i++) { m[i] = BinaryPrimitives.ReadUInt64LittleEndian(_Buffer.AsSpan(i * 8)); }

		Array.Copy(_H, 0, v, 0, 8);
		Array.Copy(_IV, 0, v, 8, 8);
		v[12] ^= _Total;
		if(last) { v[14] = ~v[14]; }

		for(int r = 0; r < 12; r++)
		{
			byte[] s = _SIGMA[r % 10];

			_G(v, 0, 4,  8, 12, m[s[0]],  m[s[1]]);
			_G(v, 1, 5,  9, 13, m[s[2]],  m[s[3]]);
			_G(v, 2, 6, 10, 14, m[s[4]],  m[s[5]]);
			_G(v, 3, 7, 11, 15, m[s[6]],  m[s[7]]);
			_G(v, 0, 5, 10, 15, m[s[8]],  m[s[9]]);
			_G(v, 1, 6, 11, 12, m[s[10]], m[s[11]]);
			_G(v, 2, 7,  8, 13, m[s[12]], m[s[13]]);
			_G(v, 3, 4,  9, 14, m[s[14]], m[s[15]]);
		}

		for(int i = 0; i < 8; i++) { _H[i] ^= v[i] ^ v[i + 8]; }
	}


	/// <summary>Mixing function.</summary>
	/// <param name="v">Working vector.</param>
	/// <param name="a">Index a.</param>
	/// <param name="b">Index b.</param>
	/// <param name="c">Index c.</param>
	/// <param name="d">Index d.</param>
	/// <param name="x">Message word x.</param>
	/// <param name="y">Message word y.</param>
	private static void _G(ulong[] v, int a, int b, int c, int d, ulong x, ulong y)
	{
		v[a] = v[a] + v[b] + x;
		v[d] = BitOperations.RotateRight(v[d] ^ v[a], 32);
		v[c] = v[c] + v[d];
		v[b] = BitOperations.RotateRight(v[b] ^ v[c], 24);
		v[a] = v[a] + v[b] + y;
		v[d] = BitOperations.RotateRight(v[d] ^ v[a], 16);
		v[c] = v[c] + v[d];
		v[b] = BitOperations.RotateRight(v[b] ^ v[c], 63);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// internal methods                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Adds data to the hash.</summary>
	/// <param name="data">Data.</param>
	internal void _Update(ReadOnlySpan<byte> data)
	{
		while(data.Length > 0)
		{
			if(_Count == 128)
			{
				_Total += 128;
				_Compress(false);
				_Count = 0;
			}

			int n = Math.Min(128 - _Count, data.Length);
			data[..n].CopyTo(_Buffer.AsSpan(_Count));
			_Count += n;
			data = data[n..];
		}
	}


	/// <summary>Finishes the hash.</summary>
	/// <returns>Returns the hash.</returns>
	internal byte[] _Final()
	{
		_Total += (ulong) _Count;
		Array.Clear(_Buffer, _Count, 128 - _Count);
		_Compress(true);

		byte[] rval = new byte[_OutputLength];
		Span<byte> h = stackalloc byte[64];
		for(int i = 0; i < 8; i++) { BinaryPrimitives.WriteUInt64LittleEndian(h[(i * 8)..], _H[i]); }
		h[.._OutputLength].CopyTo(rval);

		return rval;
	}
}
