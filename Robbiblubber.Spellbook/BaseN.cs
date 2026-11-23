namespace Robbiblubber.Spellbook;

using System.Numerics;
using System.Text;



/// <summary>This class implements an encoder for an arbitrary base.</summary>
/// <remarks>The data is interpreted as a big-endian number and written as digits of the base. Leading zero bytes are written as
///          leading zero digits (the first character of the alphabet), so that they survive a round trip. Numbers
///          (<see cref="EncodeInt(int)"/>, <see cref="EncodeLong(long)"/>, <see cref="EncodeDouble(double)"/>) are written without
///          leading zero digits.</remarks>
public class BaseN: EncoderBase, IEncoder
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public constants                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Default alphabet; it contains 92 characters.</summary>
	public const string DEFAULT_ALPHABET = @" !#$%&()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_abcdefghijklmnopqrstuvwxyz{|}~";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Digit values by character; -1 for characters that are not digits.</summary>
	private readonly short[] _Digits;

	/// <summary>Number of digits that are processed at once.</summary>
	private readonly int _ChunkDigits;

	/// <summary>Base to the power of the number of chunk digits.</summary>
	private readonly uint _ChunkBase;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	/// <param name="base">Base; from 2 to 256.</param>
	/// <param name="alphabet">Alphabet; the first characters are used as digits. NULL for the default alphabet.</param>
	/// <param name="encoding">Encoding that converts between strings and bytes; NULL for UTF-8.</param>
	/// <exception cref="ArgumentException">Thrown when the base is out of range or the alphabet is too short or contains
	///                                     duplicate characters.</exception>
	public BaseN(int @base, string? alphabet = null, Encoding? encoding = null): base(encoding)
	{
		alphabet ??= DEFAULT_ALPHABET;

		if((@base < 2) || (@base > 256)) { throw new ArgumentException("The base must be between 2 and 256.", nameof(@base)); }
		if(alphabet.Length < @base) { throw new ArgumentException("The alphabet must contain at least " + @base + " characters.", nameof(alphabet)); }

		Base = @base;
		Alphabet = alphabet[..@base];

		_Digits = new short[Alphabet.Max() + 1];
		Array.Fill(_Digits, (short) -1);

		for(int i = 0; i < Alphabet.Length; i++)
		{
			if(_Digits[Alphabet[i]] >= 0) { throw new ArgumentException("The alphabet contains duplicate characters.", nameof(alphabet)); }
			_Digits[Alphabet[i]] = (short) i;
		}

		ulong chunk = 1;
		while((chunk * (ulong) @base) <= uint.MaxValue)
		{
			chunk *= (ulong) @base;
			_ChunkDigits++;
		}
		_ChunkBase = (uint) chunk;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the base.</summary>
	public int Base
	{
		get;
	}


	/// <summary>Gets the alphabet; the character at index i stands for the digit i.</summary>
	public string Alphabet
	{
		get;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the value of a digit character.</summary>
	/// <param name="c">Character.</param>
	/// <param name="pos">Position in the string.</param>
	/// <returns>Returns the digit value.</returns>
	/// <exception cref="FormatException">Thrown when the character is not a digit.</exception>
	private int _GetDigit(char c, int pos)
	{
		int rval = ((c < _Digits.Length) ? _Digits[c] : -1);
		if(rval < 0) { throw new FormatException("Invalid character '" + c + "' at position " + pos + "."); }

		return rval;
	}


	/// <summary>Gets the powers of the chunk base used for divide-and-conquer conversion.</summary>
	/// <param name="level">Highest level required.</param>
	/// <returns>Returns the powers; the power at index i is the chunk base to the power of 2^i.</returns>
	private List<BigInteger> _GetPowers(int level)
	{
		List<BigInteger> rval = new() { _ChunkBase };
		while(rval.Count <= level) { rval.Add(rval[^1] * rval[^1]); }

		return rval;
	}


	/// <summary>Writes the digits of a number.</summary>
	/// <param name="value">Value; less than the power one level above.</param>
	/// <param name="level">Level; -1 for a value below the chunk base.</param>
	/// <param name="pad">Determines if the digits are padded to the full length of the level.</param>
	/// <param name="powers">Powers of the chunk base.</param>
	/// <param name="rval">String builder that receives the digits.</param>
	private void _WriteDigits(BigInteger value, int level, bool pad, List<BigInteger> powers, StringBuilder rval)
	{
		if(level < 0)
		{
			uint v = (uint) value;
			Span<char> digits = stackalloc char[_ChunkDigits];
			int n = 0;

			while((n < _ChunkDigits) && (pad || (v > 0)))
			{
				digits[_ChunkDigits - 1 - n++] = Alphabet[(int) (v % (uint) Base)];
				v /= (uint) Base;
			}
			rval.Append(digits[(_ChunkDigits - n)..]);
			return;
		}

		BigInteger q = BigInteger.DivRem(value, powers[level], out BigInteger r);
		if(pad || (!q.IsZero))
		{
			_WriteDigits(q, level - 1, pad, powers, rval);
			pad = true;
		}
		_WriteDigits(r, level - 1, pad, powers, rval);
	}


	/// <summary>Converts bytes to digits.</summary>
	/// <param name="data">Data, interpreted as a big-endian number.</param>
	/// <param name="zeros">Determines if leading zero bytes are written as zero digits. If FALSE, a value of zero is written as
	///                     a single zero digit.</param>
	/// <returns>Returns the digit string.</returns>
	private string _ToDigits(byte[] data, bool zeros)
	{
		int lead = 0;
		while((lead < data.Length) && (data[lead] == 0)) { lead++; }

		if(lead == data.Length)
		{
			if(zeros) { return new string(Alphabet[0], lead); }
			return ((lead == 0) ? string.Empty : Alphabet[0].ToString());
		}

		BigInteger value = new(data.AsSpan(lead), true, true);
		StringBuilder rval = new(lead + (int) Math.Ceiling((data.Length - lead) * 8 / Math.Log2(Base)) + 1);
		if(zeros) { rval.Append(Alphabet[0], lead); }

		int level = -1;
		long bits = value.GetBitLength();
		double chunkBits = Math.Log2(_ChunkBase);
		while((chunkBits * (1L << (level + 1))) < bits) { level++; }

		List<BigInteger> powers = _GetPowers(Math.Max(level, 0));
		while((level >= 0) && (value < powers[level])) { level--; }

		_WriteDigits(value, level, false, powers, rval);
		return rval.ToString();
	}


	/// <summary>Converts digits to bytes.</summary>
	/// <param name="s">Digit string.</param>
	/// <returns>Returns the big-endian bytes; each leading zero digit becomes a leading zero byte.</returns>
	/// <exception cref="FormatException">Thrown when the string contains characters that are not digits.</exception>
	private byte[] _FromDigits(string s)
	{
		int lead = 0;
		while((lead < s.Length) && (_GetDigit(s[lead], lead) == 0)) { lead++; }
		if(lead == s.Length) { return new byte[lead]; }

		List<BigInteger> parts = new(((s.Length - lead) / _ChunkDigits) + 1);
		for(int end = s.Length; end > lead; end -= _ChunkDigits)
		{
			int start = Math.Max(lead, end - _ChunkDigits);
			uint chunk = 0;
			for(int i = start; i < end; i++) { chunk = (chunk * (uint) Base) + (uint) _GetDigit(s[i], i); }

			parts.Add(chunk);
		}

		BigInteger factor = _ChunkBase;
		while(parts.Count > 1)
		{
			List<BigInteger> next = new((parts.Count + 1) / 2);
			for(int i = 0; i < parts.Count; i += 2)
			{
				next.Add(((i + 1) < parts.Count) ? ((parts[i + 1] * factor) + parts[i]) : parts[i]);
			}

			parts = next;
			if(parts.Count > 1) { factor *= factor; }
		}

		byte[] value = parts[0].ToByteArray(true, true);
		byte[] rval = new byte[lead + value.Length];
		Array.Copy(value, 0, rval, lead, value.Length);

		return rval;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Converts bytes to an encoded string as a number.</summary>
	/// <param name="value">Bytes, interpreted as a big-endian number.</param>
	/// <returns>Returns the encoded string without leading zero digits; a value of zero is written as a single zero digit.</returns>
	/// <remarks>Leading zero bytes are lost, so the bytes cannot be restored exactly. This is how earlier versions encoded all
	///          data; hashes use it to stay compatible.</remarks>
	public string EncodeNumber(IEnumerable<byte> value)
	{
		return _ToDigits(value.ToArray(), false);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] EncoderBase                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override string EncodeBytes(IEnumerable<byte> value)
	{
		return _ToDigits(value.ToArray(), true);
	}


	/// <inheritdoc/>
	public override string EncodeInt(int value)
	{
		return _ToDigits(BitConverter.GetBytes(value), false);
	}


	/// <inheritdoc/>
	public override string EncodeLong(long value)
	{
		return _ToDigits(BitConverter.GetBytes(value), false);
	}


	/// <inheritdoc/>
	public override string EncodeDouble(double value)
	{
		return _ToDigits(BitConverter.GetBytes(value), false);
	}


	/// <inheritdoc/>
	public override byte[] DecodeToBytes(string s)
	{
		return _FromDigits(s);
	}


	/// <inheritdoc/>
	public override byte[] Encode(byte[] data)
	{
		return Encoding.GetBytes(EncodeBytes(data));
	}


	/// <inheritdoc/>
	public override byte[] Decode(byte[] data)
	{
		return DecodeToBytes(Encoding.GetString(data));
	}
}
