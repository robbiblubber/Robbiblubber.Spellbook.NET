namespace Robbiblubber.Spellbook.Windows;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;



/// <summary>This class reads and writes .ini files.</summary>
/// <remarks><para>The file is read once and kept in memory. With <see cref="AutoSave"/> (default) every change is written to
///          the file immediately; otherwise changes are written by <see cref="Save"/>. Comments, blank lines, the order of the
///          entries and the formatting of unchanged lines are preserved.</para>
///          <para>The syntax follows the Windows profile functions (GetPrivateProfileString): section and key names are not
///          case-sensitive, the first occurrence of a section or key wins, white space around names and values is ignored and
///          quotes around a value are removed. Lines starting with ";" or "#" are comments.</para>
///          <para>The encoding is detected from the byte order mark; files without byte order mark are read as UTF-8 if they
///          are valid UTF-8, otherwise in the ANSI code page of the system. Files are written in the encoding they were read
///          in; new files are written in UTF-8 without byte order mark.</para></remarks>
/// <example><code>
/// IniFile ini = new("settings.ini");
/// int width = ini.GetInt32("window", "width", 800);
/// ini.SetValue("window", "width", 1024);
/// </code></example>
[SupportedOSPlatform("windows")]
public sealed partial class IniFile
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static members                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Strict UTF-8 encoding used to detect UTF-8 files without byte order mark.</summary>
	private static readonly UTF8Encoding _STRICT_UTF8 = new(false, true);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Sections in file order; the first section holds the lines before the first section header.</summary>
	private readonly List<_Section> _Sections = new();

	/// <summary>Lock object.</summary>
	private readonly object _Lock = new();

	/// <summary>Line break.</summary>
	private string _NewLine = "\r\n";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class and reads the file if it exists.</summary>
	/// <param name="fileName">File name; relative paths are relative to the current directory.</param>
	/// <exception cref="ArgumentException">Thrown when the file name is empty.</exception>
	public IniFile(string fileName)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

		FileName = Path.GetFullPath(fileName);
		Reload();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Decodes file content.</summary>
	/// <param name="data">Data.</param>
	/// <param name="encoding">Detected encoding.</param>
	/// <returns>Returns the text.</returns>
	private static string _Decode(byte[] data, out Encoding encoding)
	{
		if((data.Length >= 3) && (data[0] == 0xEF) && (data[1] == 0xBB) && (data[2] == 0xBF))
		{
			encoding = new UTF8Encoding(true);
			return Encoding.UTF8.GetString(data, 3, (data.Length - 3));
		}
		if((data.Length >= 2) && (data[0] == 0xFF) && (data[1] == 0xFE))
		{
			encoding = new UnicodeEncoding(false, true);
			return Encoding.Unicode.GetString(data, 2, (data.Length - 2));
		}
		if((data.Length >= 2) && (data[0] == 0xFE) && (data[1] == 0xFF))
		{
			encoding = new UnicodeEncoding(true, true);
			return Encoding.BigEndianUnicode.GetString(data, 2, (data.Length - 2));
		}

		try
		{
			string rval = _STRICT_UTF8.GetString(data);
			encoding = new UTF8Encoding(false);
			return rval;
		}
		catch(DecoderFallbackException)
		{
			encoding = _Ansi();
			return encoding.GetString(data);
		}
	}


	/// <summary>Gets the ANSI code page of the system.</summary>
	/// <returns>Returns the code page.</returns>
	[LibraryImport("kernel32.dll")]
	private static partial uint GetACP();


	/// <summary>Gets the ANSI encoding of the system.</summary>
	/// <returns>Returns the encoding.</returns>
	private static Encoding _Ansi()
	{
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		try
		{
			int cp = (OperatingSystem.IsWindows() ? (int) GetACP() : 1252);
			return Encoding.GetEncoding(cp);
		}
		catch(Exception ex) when((ex is ArgumentException) || (ex is NotSupportedException) || (ex is DllNotFoundException)) { return Encoding.Latin1; }
	}


	/// <summary>Checks a section name.</summary>
	/// <param name="section">Section name.</param>
	/// <exception cref="ArgumentException">Thrown when the name is empty or contains a line break or "]".</exception>
	private static void _CheckSection(string section)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(section);
		if(section.Contains(']') || section.Contains('\r') || section.Contains('\n'))
		{
			throw new ArgumentException("The section name must not contain line breaks or \"]\".", nameof(section));
		}
	}


	/// <summary>Checks a key name.</summary>
	/// <param name="key">Key name.</param>
	/// <exception cref="ArgumentException">Thrown when the name is empty, contains a line break or "=" or starts with a
	///                                      comment or section character.</exception>
	private static void _CheckKey(string key)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(key);
		if(key.Contains('=') || key.Contains('\r') || key.Contains('\n'))
		{
			throw new ArgumentException("The key must not contain line breaks or \"=\".", nameof(key));
		}

		char first = key.TrimStart()[0];
		if((first == ';') || (first == '#') || (first == '['))
		{
			throw new ArgumentException("The key must not start with \";\", \"#\" or \"[\".", nameof(key));
		}
	}


	/// <summary>Formats a value for the file.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns the value as written to the file; values with leading or trailing white space or enclosing quotes
	///          are quoted, so they are read back unchanged.</returns>
	/// <exception cref="ArgumentException">Thrown when the value contains a line break.</exception>
	private static string _FormatValue(string value)
	{
		if(value.Contains('\r') || value.Contains('\n')) { throw new ArgumentException("The value must not contain line breaks.", nameof(value)); }

		return (((value.Trim() != value) || _IsQuoted(value)) ? $"\"{value}\"" : value);
	}


	/// <summary>Determines if a value is enclosed in matching quotes.</summary>
	/// <param name="value">Value.</param>
	/// <returns>Returns TRUE if the value starts and ends with the same quote character, otherwise returns FALSE.</returns>
	private static bool _IsQuoted(string value)
	{
		return ((value.Length >= 2) && ((value[0] == '"') || (value[0] == '\'')) && (value[^1] == value[0]));
	}


	/// <summary>Parses a line that is not a section header.</summary>
	/// <param name="text">Line text.</param>
	/// <returns>Returns the line.</returns>
	private static _Line _Parse(string text)
	{
		string t = text.Trim();
		if((t.Length == 0) || (t[0] == ';') || (t[0] == '#')) { return new(text, null, null); }

		int n = t.IndexOf('=');
		if(n <= 0) { return new(text, null, null); }

		string value = t[(n + 1)..].Trim();
		if(_IsQuoted(value)) { value = value[1..^1]; }

		return new(text, t[..n].Trim(), value);
	}


	/// <summary>Determines if a line is blank.</summary>
	/// <param name="line">Line.</param>
	/// <returns>Returns TRUE if the line is empty or white space, otherwise returns FALSE.</returns>
	private static bool _IsBlank(_Line line)
	{
		return string.IsNullOrWhiteSpace(line.Text);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private methods                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Finds a section.</summary>
	/// <param name="section">Section name.</param>
	/// <returns>Returns the section, or NULL if it does not exist.</returns>
	private _Section? _Find(string section)
	{
		section = section.Trim();
		return _Sections.FirstOrDefault(m => (m.Name is not null) && string.Equals(m.Name, section, StringComparison.OrdinalIgnoreCase));
	}


	/// <summary>Finds a key line.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <returns>Returns the line, or NULL if the key does not exist.</returns>
	private _Line? _Find(string section, string key)
	{
		key = key.Trim();
		return _Find(section)?.Lines.FirstOrDefault(m => string.Equals(m.Key, key, StringComparison.OrdinalIgnoreCase));
	}


	/// <summary>Adds a new section at the end of the file.</summary>
	/// <param name="section">Section name.</param>
	/// <returns>Returns the section.</returns>
	private _Section _AddSection(string section)
	{
		_Section last = _Sections[^1];
		bool empty = ((last.Name is null) && last.Lines.All(_IsBlank));
		if((!empty) && ((last.Lines.Count == 0) || (!_IsBlank(last.Lines[^1])))) { last.Lines.Add(new("", null, null)); }

		_Section rval = new(section.Trim(), null);
		_Sections.Add(rval);

		return rval;
	}


	/// <summary>Saves the file if automatic saving is enabled.</summary>
	private void _Changed()
	{
		if(AutoSave) { Save(); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the full path of the file.</summary>
	public string FileName
	{
		get;
	}


	/// <summary>Gets or sets if every change is written to the file immediately.</summary>
	public bool AutoSave
	{
		get; set;
	} = true;


	/// <summary>Gets or sets the encoding used for saving; it is detected when the file is read.</summary>
	public Encoding Encoding
	{
		get; set;
	} = new UTF8Encoding(false);



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public methods                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Reads the file again and discards unsaved changes.</summary>
	/// <remarks>A missing file results in an empty configuration.</remarks>
	public void Reload()
	{
		lock(_Lock)
		{
			_Sections.Clear();
			_Sections.Add(new(null, null));
			_NewLine = "\r\n";

			if(!File.Exists(FileName)) { return; }

			string text = _Decode(File.ReadAllBytes(FileName), out Encoding enc);
			Encoding = enc;
			if(text.Contains('\n') && (!text.Contains("\r\n"))) { _NewLine = "\n"; }

			string[] lines = text.Split('\n').Select(m => m.TrimEnd('\r')).ToArray();
			if((lines.Length > 0) && (lines[^1].Length == 0)) { lines = lines[..^1]; }

			foreach(string i in lines)
			{
				string t = i.Trim();
				if(t.StartsWith('['))
				{
					int n = t.IndexOf(']');
					_Sections.Add(new(((n < 0) ? t[1..] : t[1..n]).Trim(), i));
				}
				else { _Sections[^1].Lines.Add(_Parse(i)); }
			}
		}
	}


	/// <summary>Writes the file.</summary>
	/// <remarks>The directory is created if it does not exist. The content is written to a temporary file first, which then
	///          replaces the file.</remarks>
	public void Save()
	{
		lock(_Lock)
		{
			StringBuilder s = new();
			foreach(_Section i in _Sections)
			{
				if(i.Name is not null) { s.Append(i.Header ?? $"[{i.Name}]").Append(_NewLine); }
				foreach(_Line j in i.Lines) { s.Append(j.Text).Append(_NewLine); }
			}

			string dir = Path.GetDirectoryName(FileName)!;
			Directory.CreateDirectory(dir);

			string temp = Path.Combine(dir, $"~{Guid.NewGuid():N}.tmp");
			try
			{
				File.WriteAllText(temp, s.ToString(), Encoding);
				File.Move(temp, FileName, true);
			}
			finally
			{
				if(File.Exists(temp)) { File.Delete(temp); }
			}
		}
	}


	/// <summary>Returns the names of all sections.</summary>
	/// <returns>Returns the section names in file order, without duplicates.</returns>
	public string[] GetSections()
	{
		lock(_Lock)
		{
			return _Sections.Where(m => m.Name is not null).Select(m => m.Name!).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
		}
	}


	/// <summary>Returns the names of all keys of a section.</summary>
	/// <param name="section">Section name.</param>
	/// <returns>Returns the key names in file order, without duplicates; an empty array if the section does not exist.</returns>
	public string[] GetKeys(string section)
	{
		_CheckSection(section);
		lock(_Lock)
		{
			return (_Find(section)?.Lines.Where(m => m.Key is not null).Select(m => m.Key!).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() ?? []);
		}
	}


	/// <summary>Determines if a section exists.</summary>
	/// <param name="section">Section name.</param>
	/// <returns>Returns TRUE if the section exists, otherwise returns FALSE.</returns>
	public bool ContainsSection(string section)
	{
		_CheckSection(section);
		lock(_Lock) { return (_Find(section) is not null); }
	}


	/// <summary>Determines if a key exists.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <returns>Returns TRUE if the key exists, otherwise returns FALSE.</returns>
	public bool ContainsKey(string section, string key)
	{
		_CheckSection(section);
		_CheckKey(key);
		lock(_Lock) { return (_Find(section, key) is not null); }
	}


	/// <summary>Reads a string value.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="value">Value, or NULL if the key does not exist.</param>
	/// <returns>Returns TRUE if the key exists, otherwise returns FALSE.</returns>
	public bool TryGetString(string section, string key, [NotNullWhen(true)] out string? value)
	{
		_CheckSection(section);
		_CheckKey(key);
		lock(_Lock)
		{
			value = _Find(section, key)?.Value;
			return (value is not null);
		}
	}


	/// <summary>Reads a string value.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="defaultValue">Default value.</param>
	/// <returns>Returns the value, or the default value if the key does not exist.</returns>
	public string GetString(string section, string key, string defaultValue = "")
	{
		return (TryGetString(section, key, out string? rval) ? rval : defaultValue);
	}


	/// <summary>Reads an integer value.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="defaultValue">Default value.</param>
	/// <returns>Returns the value, or the default value if the key does not exist or is not a valid number.</returns>
	public int GetInt32(string section, string key, int defaultValue = 0)
	{
		return ((TryGetString(section, key, out string? v) && int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int rval)) ? rval : defaultValue);
	}


	/// <summary>Reads a short integer value.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="defaultValue">Default value.</param>
	/// <returns>Returns the value, or the default value if the key does not exist or is not a valid number.</returns>
	public short GetInt16(string section, string key, short defaultValue = 0)
	{
		return ((TryGetString(section, key, out string? v) && short.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out short rval)) ? rval : defaultValue);
	}


	/// <summary>Reads a long integer value.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="defaultValue">Default value.</param>
	/// <returns>Returns the value, or the default value if the key does not exist or is not a valid number.</returns>
	public long GetInt64(string section, string key, long defaultValue = 0)
	{
		return ((TryGetString(section, key, out string? v) && long.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out long rval)) ? rval : defaultValue);
	}


	/// <summary>Reads a floating point value with "." as decimal separator.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="defaultValue">Default value.</param>
	/// <returns>Returns the value, or the default value if the key does not exist or is not a valid number.</returns>
	public double GetDouble(string section, string key, double defaultValue = 0)
	{
		return ((TryGetString(section, key, out string? v) && double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out double rval)) ? rval : defaultValue);
	}


	/// <summary>Reads a boolean value.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="defaultValue">Default value.</param>
	/// <returns>Returns the value, or the default value if the key does not exist or is not a valid boolean value.</returns>
	/// <remarks>"true", "yes", "on" and numbers other than 0 are TRUE; "false", "no", "off" and 0 are FALSE (not
	///          case-sensitive).</remarks>
	public bool GetBoolean(string section, string key, bool defaultValue = false)
	{
		if(!TryGetString(section, key, out string? v)) { return defaultValue; }
		if(long.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out long n)) { return (n != 0); }

		return v.ToLowerInvariant() switch
		{
			"true" or "yes" or "on" => true,
			"false" or "no" or "off" => false,
			_ => defaultValue
		};
	}


	/// <summary>Reads a binary value (Base64).</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <returns>Returns the value, or an empty array if the key does not exist or is not valid Base64.</returns>
	public byte[] GetBinary(string section, string key)
	{
		if(!TryGetString(section, key, out string? v)) { return []; }

		try
		{
			return Convert.FromBase64String(v);
		}
		catch(FormatException) { return []; }
	}


	/// <summary>Writes a string value.</summary>
	/// <param name="section">Section name; the section is created if it does not exist.</param>
	/// <param name="key">Key name.</param>
	/// <param name="value">Value; NULL deletes the key.</param>
	/// <exception cref="ArgumentException">Thrown when a name is invalid or the value contains a line break.</exception>
	public void SetValue(string section, string key, string? value)
	{
		_CheckSection(section);
		_CheckKey(key);
		if(value is null) { DeleteKey(section, key); return; }

		string text = $"{key.Trim()}={_FormatValue(value)}";
		lock(_Lock)
		{
			_Line? line = _Find(section, key);
			if(line is not null)
			{
				if(line.Value == value) { return; }

				line.Text = text;
				line.Value = value;
			}
			else
			{
				_Section sec = (_Find(section) ?? _AddSection(section));

				int n = (sec.Lines.FindLastIndex(m => m.Key is not null) + 1);
				if(n == 0)
				{
					n = sec.Lines.Count;
					while((n > 0) && _IsBlank(sec.Lines[n - 1])) { n--; }
				}
				sec.Lines.Insert(n, new(text, key.Trim(), value));
			}

			_Changed();
		}
	}


	/// <summary>Writes an integer value.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="value">Value.</param>
	public void SetValue(string section, string key, int value)
	{
		SetValue(section, key, value.ToString(CultureInfo.InvariantCulture));
	}


	/// <summary>Writes a long integer value.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="value">Value.</param>
	public void SetValue(string section, string key, long value)
	{
		SetValue(section, key, value.ToString(CultureInfo.InvariantCulture));
	}


	/// <summary>Writes a floating point value with "." as decimal separator.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="value">Value.</param>
	public void SetValue(string section, string key, double value)
	{
		SetValue(section, key, value.ToString("R", CultureInfo.InvariantCulture));
	}


	/// <summary>Writes a boolean value as "true" or "false".</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="value">Value.</param>
	/// <remarks>To store a boolean value as "1" or "0" use <see cref="SetIntBoolean"/>.</remarks>
	public void SetValue(string section, string key, bool value)
	{
		SetValue(section, key, (value ? "true" : "false"));
	}


	/// <summary>Writes a binary value (Base64).</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="value">Value.</param>
	public void SetValue(string section, string key, byte[] value)
	{
		ArgumentNullException.ThrowIfNull(value);
		SetValue(section, key, Convert.ToBase64String(value));
	}


	/// <summary>Writes a part of a binary value (Base64).</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="value">Value.</param>
	/// <param name="offset">Offset of the first byte.</param>
	/// <param name="length">Number of bytes.</param>
	public void SetValue(string section, string key, byte[] value, int offset, int length)
	{
		ArgumentNullException.ThrowIfNull(value);
		SetValue(section, key, Convert.ToBase64String(value, offset, length));
	}


	/// <summary>Writes a boolean value as "1" or "0".</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <param name="value">Value.</param>
	public void SetIntBoolean(string section, string key, bool value)
	{
		SetValue(section, key, (value ? "1" : "0"));
	}


	/// <summary>Deletes a key.</summary>
	/// <param name="section">Section name.</param>
	/// <param name="key">Key name.</param>
	/// <returns>Returns TRUE if the key has been deleted, FALSE if it did not exist.</returns>
	/// <remarks>All occurrences of the key in the section are deleted.</remarks>
	public bool DeleteKey(string section, string key)
	{
		_CheckSection(section);
		_CheckKey(key);
		key = key.Trim();

		lock(_Lock)
		{
			_Section? sec = _Find(section);
			if((sec is null) || (sec.Lines.RemoveAll(m => string.Equals(m.Key, key, StringComparison.OrdinalIgnoreCase)) == 0)) { return false; }

			_Changed();
			return true;
		}
	}


	/// <summary>Deletes a section with all its keys and comments.</summary>
	/// <param name="section">Section name.</param>
	/// <returns>Returns TRUE if the section has been deleted, FALSE if it did not exist.</returns>
	/// <remarks>All occurrences of the section are deleted.</remarks>
	public bool DeleteSection(string section)
	{
		_CheckSection(section);
		section = section.Trim();

		lock(_Lock)
		{
			if(_Sections.RemoveAll(m => (m.Name is not null) && string.Equals(m.Name, section, StringComparison.OrdinalIgnoreCase)) == 0) { return false; }

			_Changed();
			return true;
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] object                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override string ToString()
	{
		return FileName;
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _Section                                                                                                 //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class represents a section.</summary>
	private sealed class _Section
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="name">Section name, or NULL for the lines before the first section.</param>
		/// <param name="header">Original header line, or NULL for a new section.</param>
		public _Section(string? name, string? header)
		{
			Name = name;
			Header = header;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the section name, or NULL for the lines before the first section.</summary>
		public string? Name
		{
			get;
		}


		/// <summary>Gets the original header line, or NULL for a new section.</summary>
		public string? Header
		{
			get;
		}


		/// <summary>Gets the lines of the section.</summary>
		public List<_Line> Lines
		{
			get;
		} = new();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [class] _Line                                                                                                    //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class represents a line.</summary>
	private sealed class _Line
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="text">Line text.</param>
		/// <param name="key">Key name, or NULL for comments, blank and invalid lines.</param>
		/// <param name="value">Value, or NULL for comments, blank and invalid lines.</param>
		public _Line(string text, string? key, string? value)
		{
			Text = text;
			Key = key;
			Value = value;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public properties                                                                                            //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets or sets the line text.</summary>
		public string Text
		{
			get; set;
		}


		/// <summary>Gets the key name, or NULL for comments, blank and invalid lines.</summary>
		public string? Key
		{
			get;
		}


		/// <summary>Gets or sets the value, or NULL for comments, blank and invalid lines.</summary>
		public string? Value
		{
			get; set;
		}
	}
}
