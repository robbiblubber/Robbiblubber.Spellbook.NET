namespace Robbiblubber.Spellbook;

using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;



/// <summary>This class hashes passwords with Argon2 (RFC 9106, version 1.3).</summary>
/// <remarks>Hashes are written in the PHC string format <c>$argon2id$v=19$m=19456,t=2,p=1$salt$hash</c> with Base64 salt and hash,
///          as used by the reference implementation and most other libraries. The default parameters follow the OWASP
///          recommendations: Argon2id with 19 MiB memory, 2 iterations, 1 lane, a 16 byte salt and a 32 byte hash.</remarks>
public sealed class Argon2: IPasswordHasher
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Instance with default parameters.</summary>
	public static readonly Argon2 DEFAULT = new();


	/// <summary>Maximum memory size in KiB (4 GiB).</summary>
	public const int MAX_MEMORY = 4 * 1024 * 1024;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Argon2 version.</summary>
	private const int _VERSION = 0x13;

	/// <summary>Number of 64-bit words in a block.</summary>
	private const int _BLOCK_WORDS = 128;

	/// <summary>Number of slices in a lane.</summary>
	private const int _SYNC_POINTS = 4;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Word indexes of the rows and columns the permutation is applied to.</summary>
	private static readonly int[][] _ROUNDS = _GetRounds();



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Secret key.</summary>
	private readonly byte[] _Secret;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="type">Argon2 variant.</param>
	/// <param name="memory">Memory size in KiB; at least 8 times the parallelism, at most <see cref="MAX_MEMORY"/>.</param>
	/// <param name="iterations">Number of iterations (passes over the memory); at least 1.</param>
	/// <param name="parallelism">Degree of parallelism (number of lanes); from 1 to 255.</param>
	/// <param name="saltSize">Salt size in bytes; at least 8.</param>
	/// <param name="hashSize">Hash size in bytes; at least 16.</param>
	/// <param name="secret">Secret key (pepper) that is not stored with the hash; NULL for none. Hashes created with a
	///                      secret can only be verified with the same secret.</param>
	/// <exception cref="ArgumentException">Thrown when a parameter is invalid.</exception>
	public Argon2(Argon2Type type = Argon2Type.ARGON2ID, int memory = 19456, int iterations = 2, int parallelism = 1, int saltSize = 16, int hashSize = 32, byte[]? secret = null)
	{
		_Validate(type, memory, iterations, parallelism, hashSize);
		if(saltSize < 8) { throw new ArgumentException("The salt must have at least 8 bytes.", nameof(saltSize)); }
		if(hashSize < 16) { throw new ArgumentException("The hash must have at least 16 bytes.", nameof(hashSize)); }

		Type = type;
		Memory = memory;
		Iterations = iterations;
		Parallelism = parallelism;
		SaltSize = saltSize;
		HashSize = hashSize;

		_Secret = ((secret is null) ? [] : (byte[]) secret.Clone());
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the Argon2 variant.</summary>
	public Argon2Type Type
	{
		get;
	}


	/// <summary>Gets the memory size in KiB.</summary>
	public int Memory
	{
		get;
	}


	/// <summary>Gets the number of iterations.</summary>
	public int Iterations
	{
		get;
	}


	/// <summary>Gets the degree of parallelism.</summary>
	public int Parallelism
	{
		get;
	}


	/// <summary>Gets the salt size in bytes.</summary>
	public int SaltSize
	{
		get;
	}


	/// <summary>Gets the hash size in bytes.</summary>
	public int HashSize
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Derives a key from a password.</summary>
	/// <param name="type">Argon2 variant.</param>
	/// <param name="password">Password.</param>
	/// <param name="salt">Salt; at least 8 bytes.</param>
	/// <param name="memory">Memory size in KiB; at least 8 times the parallelism, at most <see cref="MAX_MEMORY"/>.</param>
	/// <param name="iterations">Number of iterations; at least 1.</param>
	/// <param name="parallelism">Degree of parallelism; from 1 to 255.</param>
	/// <param name="hashSize">Key size in bytes; at least 4.</param>
	/// <param name="secret">Secret key; NULL for none.</param>
	/// <param name="associatedData">Associated data; NULL for none.</param>
	/// <returns>Returns the derived key.</returns>
	/// <exception cref="ArgumentException">Thrown when a parameter is invalid.</exception>
	public static byte[] DeriveKey(Argon2Type type, byte[] password, byte[] salt, int memory, int iterations, int parallelism, int hashSize, byte[]? secret = null, byte[]? associatedData = null)
	{
		_Validate(type, memory, iterations, parallelism, hashSize);
		if(salt.Length < 8) { throw new ArgumentException("The salt must have at least 8 bytes.", nameof(salt)); }

		return _Compute(type, password, salt, (secret ?? []), (associatedData ?? []), memory, iterations, parallelism, hashSize);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Validates parameters.</summary>
	/// <param name="type">Argon2 variant.</param>
	/// <param name="memory">Memory size in KiB.</param>
	/// <param name="iterations">Number of iterations.</param>
	/// <param name="parallelism">Degree of parallelism.</param>
	/// <param name="hashSize">Hash size in bytes.</param>
	/// <exception cref="ArgumentException">Thrown when a parameter is invalid.</exception>
	private static void _Validate(Argon2Type type, int memory, int iterations, int parallelism, int hashSize)
	{
		if(!Enum.IsDefined(type)) { throw new ArgumentException("Unknown Argon2 variant.", nameof(type)); }
		if((parallelism < 1) || (parallelism > 255)) { throw new ArgumentException("The parallelism must be between 1 and 255.", nameof(parallelism)); }
		if((memory < (8 * parallelism)) || (memory > MAX_MEMORY)) { throw new ArgumentException("The memory must be between " + (8 * parallelism) + " and " + MAX_MEMORY + " KiB.", nameof(memory)); }
		if(iterations < 1) { throw new ArgumentException("The number of iterations must be at least 1.", nameof(iterations)); }
		if(hashSize < 4) { throw new ArgumentException("The hash must have at least 4 bytes.", nameof(hashSize)); }
	}


	/// <summary>Gets the word indexes of the rows and columns.</summary>
	/// <returns>Returns 8 rows followed by 8 columns of 16 word indexes each.</returns>
	private static int[][] _GetRounds()
	{
		int[][] rval = new int[16][];

		for(int i = 0; i < 8; i++)
		{
			rval[i] = new int[16];
			rval[i + 8] = new int[16];

			for(int k = 0; k < 16; k++) { rval[i][k] = ((16 * i) + k); }
			for(int k = 0; k < 8; k++)
			{
				rval[i + 8][2 * k] = ((2 * i) + (16 * k));
				rval[i + 8][(2 * k) + 1] = ((2 * i) + (16 * k) + 1);
			}
		}

		return rval;
	}


	/// <summary>Adds a 32-bit little-endian value to a hash.</summary>
	/// <param name="hash">Hash.</param>
	/// <param name="value">Value.</param>
	private static void _AddInt(_Blake2b hash, int value)
	{
		Span<byte> buf = stackalloc byte[4];
		BinaryPrimitives.WriteInt32LittleEndian(buf, value);
		hash._Update(buf);
	}


	/// <summary>Adds length-prefixed data to a hash.</summary>
	/// <param name="hash">Hash.</param>
	/// <param name="data">Data.</param>
	private static void _AddData(_Blake2b hash, ReadOnlySpan<byte> data)
	{
		_AddInt(hash, data.Length);
		hash._Update(data);
	}


	/// <summary>Variable-length hash function H'.</summary>
	/// <param name="length">Output length in bytes.</param>
	/// <param name="data">Data.</param>
	/// <returns>Returns the hash.</returns>
	private static byte[] _HashLong(int length, ReadOnlySpan<byte> data)
	{
		_Blake2b hash = new(Math.Min(length, 64));
		_AddInt(hash, length);
		hash._Update(data);

		byte[] v = hash._Final();
		if(length <= 64) { return v; }

		byte[] rval = new byte[length];
		v.AsSpan(0, 32).CopyTo(rval);

		int pos = 32;
		while((length - pos) > 64)
		{
			v = _Blake2b._Hash(64, v);
			v.AsSpan(0, 32).CopyTo(rval.AsSpan(pos));
			pos += 32;
		}

		_Blake2b._Hash(length - pos, v).CopyTo(rval.AsSpan(pos));
		return rval;
	}


	/// <summary>Mixing function with multiplication.</summary>
	/// <param name="a">Word a.</param>
	/// <param name="b">Word b.</param>
	/// <param name="c">Word c.</param>
	/// <param name="d">Word d.</param>
	private static void _GB(ref ulong a, ref ulong b, ref ulong c, ref ulong d)
	{
		a = a + b + (2 * (ulong) (uint) a * (uint) b);
		d = BitOperations.RotateRight(d ^ a, 32);
		c = c + d + (2 * (ulong) (uint) c * (uint) d);
		b = BitOperations.RotateRight(b ^ c, 24);
		a = a + b + (2 * (ulong) (uint) a * (uint) b);
		d = BitOperations.RotateRight(d ^ a, 16);
		c = c + d + (2 * (ulong) (uint) c * (uint) d);
		b = BitOperations.RotateRight(b ^ c, 63);
	}


	/// <summary>Applies the permutation to 16 words of a block.</summary>
	/// <param name="r">Block.</param>
	/// <param name="x">Word indexes.</param>
	private static void _P(ulong[] r, int[] x)
	{
		_GB(ref r[x[0]], ref r[x[4]], ref r[x[8]],  ref r[x[12]]);
		_GB(ref r[x[1]], ref r[x[5]], ref r[x[9]],  ref r[x[13]]);
		_GB(ref r[x[2]], ref r[x[6]], ref r[x[10]], ref r[x[14]]);
		_GB(ref r[x[3]], ref r[x[7]], ref r[x[11]], ref r[x[15]]);
		_GB(ref r[x[0]], ref r[x[5]], ref r[x[10]], ref r[x[15]]);
		_GB(ref r[x[1]], ref r[x[6]], ref r[x[11]], ref r[x[12]]);
		_GB(ref r[x[2]], ref r[x[7]], ref r[x[8]],  ref r[x[13]]);
		_GB(ref r[x[3]], ref r[x[4]], ref r[x[9]],  ref r[x[14]]);
	}


	/// <summary>Compression function G; computes a new block from the previous block and a reference block.</summary>
	/// <param name="prev">Previous block.</param>
	/// <param name="refBlock">Reference block.</param>
	/// <param name="next">Block that receives the result.</param>
	/// <param name="xor">Determines if the result is combined with the existing content of the block.</param>
	/// <param name="r">Work block.</param>
	/// <param name="tmp">Second work block.</param>
	private static void _FillBlock(ReadOnlySpan<ulong> prev, ReadOnlySpan<ulong> refBlock, Span<ulong> next, bool xor, ulong[] r, ulong[] tmp)
	{
		for(int i = 0; i < _BLOCK_WORDS; i++) { r[i] = (prev[i] ^ refBlock[i]); }

		if(xor)
		{
			for(int i = 0; i < _BLOCK_WORDS; i++) { tmp[i] = (r[i] ^ next[i]); }
		}
		else { Array.Copy(r, tmp, _BLOCK_WORDS); }

		foreach(int[] i in _ROUNDS) { _P(r, i); }

		for(int i = 0; i < _BLOCK_WORDS; i++) { next[i] = (tmp[i] ^ r[i]); }
	}


	/// <summary>Computes the reference block index within a lane.</summary>
	/// <param name="pass">Pass.</param>
	/// <param name="slice">Slice.</param>
	/// <param name="index">Block index within the segment.</param>
	/// <param name="rand">Pseudo-random value (lower 32 bits).</param>
	/// <param name="sameLane">Determines if the reference block is in the current lane.</param>
	/// <param name="segmentLength">Segment length in blocks.</param>
	/// <param name="laneLength">Lane length in blocks.</param>
	/// <returns>Returns the block index.</returns>
	private static int _IndexAlpha(int pass, int slice, int index, uint rand, bool sameLane, int segmentLength, int laneLength)
	{
		long area;
		if(pass == 0)
		{
			if(slice == 0) { area = (index - 1); }
			else if(sameLane) { area = ((slice * segmentLength) + index - 1); }
			else { area = ((slice * segmentLength) + ((index == 0) ? -1 : 0)); }
		}
		else if(sameLane) { area = (laneLength - segmentLength + index - 1); }
		else { area = (laneLength - segmentLength + ((index == 0) ? -1 : 0)); }

		ulong rel = (((ulong) rand * rand) >> 32);
		rel = ((ulong) area - 1 - (((ulong) area * rel) >> 32));

		ulong start = (((pass == 0) || (slice == (_SYNC_POINTS - 1))) ? 0 : (ulong) ((slice + 1) * segmentLength));

		return (int) ((start + rel) % (ulong) laneLength);
	}


	/// <summary>Fills a segment.</summary>
	/// <param name="memory">Memory.</param>
	/// <param name="type">Argon2 variant.</param>
	/// <param name="iterations">Number of iterations.</param>
	/// <param name="lanes">Number of lanes.</param>
	/// <param name="segmentLength">Segment length in blocks.</param>
	/// <param name="pass">Pass.</param>
	/// <param name="lane">Lane.</param>
	/// <param name="slice">Slice.</param>
	private static void _FillSegment(ulong[] memory, Argon2Type type, int iterations, int lanes, int segmentLength, int pass, int lane, int slice)
	{
		int laneLength = (segmentLength * _SYNC_POINTS);
		ulong[] r = new ulong[_BLOCK_WORDS];
		ulong[] tmp = new ulong[_BLOCK_WORDS];

		bool independent = ((type == Argon2Type.ARGON2I) || ((type == Argon2Type.ARGON2ID) && (pass == 0) && (slice < (_SYNC_POINTS / 2))));
		ulong[] zero = [];
		ulong[] input = [];
		ulong[] address = [];

		if(independent)
		{
			zero = new ulong[_BLOCK_WORDS];
			input = new ulong[_BLOCK_WORDS];
			address = new ulong[_BLOCK_WORDS];

			input[0] = (ulong) pass;
			input[1] = (ulong) lane;
			input[2] = (ulong) slice;
			input[3] = (ulong) (laneLength * lanes);
			input[4] = (ulong) iterations;
			input[5] = (ulong) type;
		}

		void nextAddresses()
		{
			input[6]++;
			_FillBlock(zero, input, address, false, r, tmp);
			_FillBlock(zero, address, address, false, r, tmp);
		}

		int start = 0;
		if((pass == 0) && (slice == 0))
		{
			start = 2;
			if(independent) { nextAddresses(); }
		}

		int curr = ((lane * laneLength) + (slice * segmentLength) + start);
		int prev = (((curr % laneLength) == 0) ? (curr + laneLength - 1) : (curr - 1));

		for(int i = start; i < segmentLength; i++, curr++, prev++)
		{
			if((curr % laneLength) == 1) { prev = (curr - 1); }

			ulong rand;
			if(independent)
			{
				if((i % _BLOCK_WORDS) == 0) { nextAddresses(); }
				rand = address[i % _BLOCK_WORDS];
			}
			else { rand = memory[prev * _BLOCK_WORDS]; }

			int refLane = (((pass == 0) && (slice == 0)) ? lane : (int) ((rand >> 32) % (ulong) lanes));
			int refIndex = _IndexAlpha(pass, slice, i, (uint) rand, (refLane == lane), segmentLength, laneLength);

			_FillBlock(memory.AsSpan(prev * _BLOCK_WORDS, _BLOCK_WORDS), memory.AsSpan(((refLane * laneLength) + refIndex) * _BLOCK_WORDS, _BLOCK_WORDS),
					   memory.AsSpan(curr * _BLOCK_WORDS, _BLOCK_WORDS), (pass > 0), r, tmp);
		}
	}


	/// <summary>Computes an Argon2 hash.</summary>
	/// <param name="type">Argon2 variant.</param>
	/// <param name="password">Password.</param>
	/// <param name="salt">Salt.</param>
	/// <param name="secret">Secret key.</param>
	/// <param name="associatedData">Associated data.</param>
	/// <param name="memory">Memory size in KiB.</param>
	/// <param name="iterations">Number of iterations.</param>
	/// <param name="parallelism">Degree of parallelism.</param>
	/// <param name="hashSize">Hash size in bytes.</param>
	/// <returns>Returns the hash.</returns>
	private static byte[] _Compute(Argon2Type type, ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, ReadOnlySpan<byte> secret, ReadOnlySpan<byte> associatedData,
								   int memory, int iterations, int parallelism, int hashSize)
	{
		_Blake2b h = new(64);
		_AddInt(h, parallelism);
		_AddInt(h, hashSize);
		_AddInt(h, memory);
		_AddInt(h, iterations);
		_AddInt(h, _VERSION);
		_AddInt(h, (int) type);
		_AddData(h, password);
		_AddData(h, salt);
		_AddData(h, secret);
		_AddData(h, associatedData);

		byte[] seed = new byte[72];
		h._Final().CopyTo(seed, 0);

		int lanes = parallelism;
		int segmentLength = (memory / (lanes * _SYNC_POINTS));
		int laneLength = (segmentLength * _SYNC_POINTS);
		ulong[] mem = new ulong[lanes * laneLength * _BLOCK_WORDS];

		try
		{
			for(int l = 0; l < lanes; l++)
			{
				BinaryPrimitives.WriteInt32LittleEndian(seed.AsSpan(68), l);
				for(int k = 0; k < 2; k++)
				{
					BinaryPrimitives.WriteInt32LittleEndian(seed.AsSpan(64), k);
					byte[] block = _HashLong(1024, seed);

					int offset = (((l * laneLength) + k) * _BLOCK_WORDS);
					for(int i = 0; i < _BLOCK_WORDS; i++) { mem[offset + i] = BinaryPrimitives.ReadUInt64LittleEndian(block.AsSpan(i * 8)); }
					CryptographicOperations.ZeroMemory(block);
				}
			}

			for(int pass = 0; pass < iterations; pass++)
			{
				for(int slice = 0; slice < _SYNC_POINTS; slice++)
				{
					if(lanes == 1)
					{
						_FillSegment(mem, type, iterations, lanes, segmentLength, pass, 0, slice);
					}
					else { Parallel.For(0, lanes, l => _FillSegment(mem, type, iterations, lanes, segmentLength, pass, l, slice)); }
				}
			}

			byte[] final = new byte[1024];
			for(int i = 0; i < _BLOCK_WORDS; i++)
			{
				ulong v = 0;
				for(int l = 0; l < lanes; l++) { v ^= mem[(((l * laneLength) + laneLength - 1) * _BLOCK_WORDS) + i]; }
				BinaryPrimitives.WriteUInt64LittleEndian(final.AsSpan(i * 8), v);
			}

			byte[] rval = _HashLong(hashSize, final);
			CryptographicOperations.ZeroMemory(final);

			return rval;
		}
		finally
		{
			Array.Clear(mem);
			CryptographicOperations.ZeroMemory(seed);
		}
	}


	/// <summary>Parses a password hash.</summary>
	/// <param name="hash">Password hash.</param>
	/// <param name="type">Argon2 variant.</param>
	/// <param name="memory">Memory size in KiB.</param>
	/// <param name="iterations">Number of iterations.</param>
	/// <param name="parallelism">Degree of parallelism.</param>
	/// <param name="salt">Salt.</param>
	/// <param name="key">Derived key.</param>
	/// <returns>Returns TRUE if the hash is a valid Argon2 hash, otherwise returns FALSE.</returns>
	private static bool _TryParse(string hash, out Argon2Type type, out int memory, out int iterations, out int parallelism, out byte[] salt, out byte[] key)
	{
		type = default;
		memory = iterations = parallelism = 0;
		salt = key = [];

		string[] parts = hash.Split('$');
		if((parts.Length != 6) || (parts[0].Length != 0) || (parts[2] != "v=19")) { return false; }

		switch(parts[1])
		{
			case "argon2d": type = Argon2Type.ARGON2D; break;
			case "argon2i": type = Argon2Type.ARGON2I; break;
			case "argon2id": type = Argon2Type.ARGON2ID; break;
			default: return false;
		}

		string[] p = parts[3].Split(',');
		if((p.Length != 3) || (!_TryParseInt(p[0], "m=", out memory)) || (!_TryParseInt(p[1], "t=", out iterations)) || (!_TryParseInt(p[2], "p=", out parallelism))) { return false; }

		try
		{
			salt = Base64.ToBytes(parts[4]);
			key = Base64.ToBytes(parts[5]);
		}
		catch(FormatException) { return false; }

		return ((parallelism >= 1) && (parallelism <= 255) && (memory >= (8 * parallelism)) && (memory <= MAX_MEMORY) && (iterations >= 1) &&
				(salt.Length >= 8) && (key.Length >= 4));
	}


	/// <summary>Parses a parameter.</summary>
	/// <param name="s">String.</param>
	/// <param name="prefix">Expected prefix.</param>
	/// <param name="value">Value.</param>
	/// <returns>Returns TRUE if the parameter has been parsed, otherwise returns FALSE.</returns>
	private static bool _TryParseInt(string s, string prefix, out int value)
	{
		value = 0;
		return (s.StartsWith(prefix, StringComparison.Ordinal) && int.TryParse(s[prefix.Length..], NumberStyles.None, CultureInfo.InvariantCulture, out value));
	}


	/// <summary>Gets the PHC name of a variant.</summary>
	/// <param name="type">Argon2 variant.</param>
	/// <returns>Returns the name.</returns>
	private static string _GetName(Argon2Type type)
	{
		return type switch
		{
			Argon2Type.ARGON2D => "argon2d",
			Argon2Type.ARGON2I => "argon2i",
			_                  => "argon2id"
		};
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Derives a key from a password with the parameters and secret of this instance.</summary>
	/// <param name="password">Password.</param>
	/// <param name="salt">Salt; at least 8 bytes.</param>
	/// <param name="associatedData">Associated data; NULL for none.</param>
	/// <returns>Returns a key of <see cref="HashSize"/> bytes.</returns>
	/// <exception cref="ArgumentException">Thrown when the salt is too short.</exception>
	public byte[] DeriveKey(byte[] password, byte[] salt, byte[]? associatedData = null)
	{
		if(salt.Length < 8) { throw new ArgumentException("The salt must have at least 8 bytes.", nameof(salt)); }

		return _Compute(Type, password, salt, _Secret, (associatedData ?? []), Memory, Iterations, Parallelism, HashSize);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IPasswordHasher                                                                                      //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public string Hash(string password)
	{
		byte[] pw = Encoding.UTF8.GetBytes(password);
		byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

		try
		{
			byte[] key = _Compute(Type, pw, salt, _Secret, [], Memory, Iterations, Parallelism, HashSize);

			return ("$" + _GetName(Type) + "$v=19$m=" + Memory.ToString(CultureInfo.InvariantCulture) + ",t=" + Iterations.ToString(CultureInfo.InvariantCulture) +
					",p=" + Parallelism.ToString(CultureInfo.InvariantCulture) + "$" + Base64.FromBytes(salt) + "$" + Base64.FromBytes(key));
		}
		finally { CryptographicOperations.ZeroMemory(pw); }
	}


	/// <inheritdoc/>
	public bool Verify(string password, string hash)
	{
		if(!_TryParse(hash, out Argon2Type type, out int memory, out int iterations, out int parallelism, out byte[] salt, out byte[] key)) { return false; }

		byte[] pw = Encoding.UTF8.GetBytes(password);
		try
		{
			return CryptographicOperations.FixedTimeEquals(_Compute(type, pw, salt, _Secret, [], memory, iterations, parallelism, key.Length), key);
		}
		finally { CryptographicOperations.ZeroMemory(pw); }
	}


	/// <inheritdoc/>
	public bool NeedsRehash(string hash)
	{
		if(!_TryParse(hash, out Argon2Type type, out int memory, out int iterations, out int parallelism, out byte[] salt, out byte[] key)) { return true; }

		return ((type != Type) || (memory < Memory) || (iterations < Iterations) || (parallelism != Parallelism) || (salt.Length < SaltSize) || (key.Length < HashSize));
	}
}
