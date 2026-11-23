# robbiblubber.org Spellbook Core Library

The robbiblubber.org Spellbook Core Library provides support for the Diabolic
Data Notation (ddn), encoding, hashing and encryption algorithms and a number
of utility classes that are widely used in robbiblubber.org software products.

The library targets .NET 8 and .NET 10 and contains no platform-dependent code.

The repository can be found at https://bitbucket.org/robbiblubber/robbiblubber.spellbook.net/.
API documentation is available from https://robbiblubber.lima-city.org/.
For general information see http://robbiblubber.org/.

The robbiblubber.org Spellbook Core Library is provided under the MIT license
and may be used without restrictions under the conditions of this license
agreement.


## ddn

ddn is a hierarchical key-value format. It is easy to read and write for
humans and is used for configuration files as well as for data interchange:

```
// application settings
title = Spellbook Sample;
started = 2026-10-19 08:15:30.000+02:00;
recent = c:\\data\\a.txt, c:\\data\\b.txt;

window
{
    size = 800, 600;
}

backup
{
    day = Friday;
}
```

Values are terminated by a semicolon, sections enclose their elements in curly
brackets. Special characters are masked with a backslash (`\;`, `\,`, `\=`,
`\{`, `\}`, `\\`, `\/`), `\n` and `\t` stand for line breaks and tabs, `\_`
for spaces at the beginning or end of a value and `\0` for NULL. Comments
start with `//` or are enclosed in `/* */`.
The complete format is described in the [ddn specification](ddn.md).


### Reading and writing

A document is opened with `Ddn.Open()` or created from text with
`Ddn.Create()`. Elements are addressed by paths; the indexer returns an
element, the implicit conversion operators and `Get<T>()` return its value:

```csharp
using Robbiblubber.Spellbook;

DdnElement cfg = Ddn.Open("settings.ddn", true);      // empty document if the file is missing

string title = cfg["title"];                           // "" if missing
int[] size = cfg["window/size"];                       // empty array if missing
string[] recent = cfg["recent"];
DayOfWeek day = cfg.Get("backup/day", DayOfWeek.Sunday);
DateTime started = cfg.Get<DateTime>("started");       // local time
int? retries = cfg["network/retries"];                 // NULL if missing or invalid

cfg["window"]["size"] = new[] { 1024, 768 };           // creates missing sections
cfg["backup/day"] = DayOfWeek.Monday;                  // written as name, read from name or number
cfg["recent"] = new List<string> { @"c:\data\c.txt" };
cfg.Save("settings.ddn");
```

Reading a path that does not exist never changes the document: the returned
element is added, together with its missing parent sections, only when a value
or a child element is written. `Exists` and `Contains()` tell whether an
element is part of the document.

Paths are relative to the element (`a/b`), absolute paths start at the root
(`/a/b`), `.` is the element itself and `..` its parent. `Element()` gets a
child by its plain name, for names that contain a slash.


### Values

Values are converted culture-invariantly. Supported types are strings,
characters, Booleans, all numeric types, enums, `DateTime`, `DateTimeOffset`,
`DateOnly`, `TimeOnly`, `TimeSpan`, `Guid`, types with a string type
converter (`Version`, `Uri`, ...) and their nullable forms. Arrays, lists,
sets and other collections of these types are written as comma-separated ddn
arrays.

- Timestamps are written as `yyyy-MM-dd HH:mm:ss.fff+HH:MM`; all parts except
  the date may be omitted when reading. A timestamp without time zone is a
  local time.
- Booleans are written as `true` and `false`; `yes`, `no` and numbers are read
  as well.
- Enums are written as names (flags as `A\, B`); names and numbers are read.
  Cast the value to `int` to write a number.
- `Get<T>()` returns the default value of the type for NULL, missing and
  invalid values, `Get(path, defaultValue)` returns the given default and
  `TryGet()` tells whether a value is present.


### Working with elements

```csharp
DdnElement doc = Ddn.Create("a = 1; s { x = 1; t { } }");

foreach(DdnElement i in doc["s"].Sections) { Console.WriteLine(i.Path); }   // /s/t
foreach(DdnElement i in doc.Values) { Console.WriteLine(i.Name + " = " + i.Value); }

doc["s/x"].Name = "y";                  // rename
doc["s/y"].MoveTo(doc["other"]);        // move, creating the target section
doc["s"].CopyTo(doc["backup"]);         // deep copy
doc.Remove("a");
doc.Merge(Ddn.Open("defaults.ddn"));    // values of the defaults replace existing values

DdnElement copy = doc.Clone();
```

Child elements keep the order in which they have been added or read. When
parsing, a value that is defined twice replaces the earlier value and a section
that is defined twice is merged; `ParsedDuplicates` lists these elements.


### Formatting and encoding

`Ddn.Options` controls the output: `MultiLine`, `SectionSpacing`, `Ordered`
(alphabetical with numeric comparison of numbers, values before sections),
`IgnoreNullElements`, `Indent`, `NewLine` and `Append`. `Ddn.Options.SINGLE_LINE`
writes a document in one line, which is useful for data interchange.

With an `Encoder` the whole document is encoded, compressed or encrypted:

```csharp
Ddn.Options secret = new(new AESGCM("my secret"));

cfg.Save("settings.dat", secret);
DdnElement read = Ddn.Open("settings.dat", secret);

string compact = cfg.ToString(GZip.BASE64);
```


### Serialization

`Ddn.Serialize()` writes an object into a section, `Ddn.Deserialize<T>()`
creates an object from a section or fills an existing one. Public properties
with a setter (of any accessibility) and public get-only auto-properties are
serialized; nested objects become sections, lists use the item index as the
element name and dictionaries use the key. Lists of scalar values are written
as ddn arrays.

```csharp
public record Size(int Width, int Height);

public class Connection
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1521;
    [Ddn.Ignore] public string? Password { get; set; }
}

public class Settings
{
    public string User { get; set; } = "";
    public DayOfWeek Theme { get; set; } = DayOfWeek.Monday;
    public Size Window { get; set; } = new(640, 480);
    public List<string> Recent { get; set; } = [];
    public Dictionary<string, Connection> Connections { get; } = [];
}

Settings settings = new() { User = "debbie", Window = new Size(800, 600), Recent = ["a.txt", "b.txt"] };
settings.Connections["main"] = new Connection() { Host = "db.local", Port = 5432 };

Ddn.Serialize(settings).Save("settings.ddn");
Settings copy = Ddn.Deserialize<Settings>(Ddn.Open("settings.ddn"))!;
```

The file looks like this:

```
Recent = a.txt, b.txt;
Theme = Monday;
User = debbie;

Connections
{
    main
    {
        Host = db.local;
        Port = 5432;
    }
}

Window
{
    Height = 600;
    Width = 800;
}
```

Elements that are missing keep the values the object already has, so default
values survive and old files keep working when new properties are added.
Objects without a parameterless constructor, such as records, are created
through the constructor whose parameters match the members.

```csharp
Settings? existing = new();
Ddn.Deserialize(ref existing, Ddn.Create("User = admin;"));    // Theme stays Monday
```

`DdnElement.Set()` and `Get<T>()` serialize complex types as well:

```csharp
DdnElement cfg = Ddn.Create();
cfg["app/settings"].Set(settings);

Connection c = cfg.Get<Connection>("app/settings/Connections/main");
```

Attributes control what is serialized:

- `[Ddn.Ignore]` excludes a property or field.
- `[Ddn.Serialize]` includes a field, a non-public property or a method, and
  can set the element name and the direction (`INBOUND`, `OUTBOUND`). A method
  without parameters provides a value, a method with one parameter receives it.
- `[Ddn.Serializable]` on a class selects properties, fields and non-public
  members for the whole class.

Type names are not written by default. For polymorphic members they can be
enabled with `SerializationOptions`; `AUTO` writes a `$type` element only where
the object type differs from the declared type, `ALL` writes it for every
object. Type names are read unless `TypeNames` is `NONE`, and the type must be
assignable to the declared type.

```csharp
Ddn.SerializationOptions options = new() { TypeNames = Ddn.TypeNameHandling.AUTO };

DdnElement data = Ddn.Serialize(zoo, options);
Zoo copy = Ddn.Deserialize<Zoo>(data, options)!;
```

Custom serializers handle types that need a special format. They derive from
`Ddn.Serializer<T>` or are registered with delegates; serializers marked with
`[Ddn.CustomSerializer]` are registered by `Ddn.Discover()`. The context
passed to a serializer serializes nested values and applies the default member
serialization.

```csharp
[Ddn.CustomSerializer]
public class RgbSerializer: Ddn.Serializer<Rgb>
{
    public override void Serialize(Rgb value, DdnElement elm, Ddn.SerializationContext context)
    {
        elm.Set($"#{value.R:X2}{value.G:X2}{value.B:X2}");
    }

    public override Rgb? Deserialize(DdnElement elm, Rgb? existing, Ddn.SerializationContext context)
    {
        string s = elm.Get<string>();
        return new Rgb() { R = Convert.ToByte(s[1..3], 16), G = Convert.ToByte(s[3..5], 16), B = Convert.ToByte(s[5..7], 16) };
    }
}

Ddn.Discover(typeof(Program).Assembly);

Ddn.RegisterSerializer<Size>((v, elm, ctx) => elm.Set(v.Width + "x" + v.Height),
                             (elm, existing, ctx) => { string[] p = elm.Get<string>().Split('x'); return new Size(int.Parse(p[0]), int.Parse(p[1])); });
```

Circular references, dictionary keys that are not scalar values and
multi-dimensional arrays throw a `Ddn.SerializeException`.


## Encoding

Encoders implement `IEncoder` and convert bytes, strings and numbers to text
and back:

- `Base62` and `InvertedBase62` use digits and letters only, so the result can
  be used in file names, URLs and identifiers. `BaseN` encodes to any base from
  2 to 256 with a custom alphabet.
- `Base64` writes standard Base64 without padding by default; `Base64.URL`
  writes the URL-safe alphabet. Both alphabets are read.
- `Hex` writes lower case hexadecimal digits, `Hex.UPPER` upper case.
- `PlainText` leaves text unchanged.
- `GZip` compresses data and encodes it with another encoder.

Each encoder has a default instance (`DEFAULT`) and static shortcuts:

```csharp
string id = Base62.FromString("Hello");           // "5TP3P3v"
string text = Base62.Decode(id);                  // "Hello"
string n = Base62.FromInt(123456789);             // "Okm0t"
int i = Base62.ToInt(n);

byte[] key = Hex.ToBytes("00ff10");
string url = Base64.URL.EncodeBytes(key);         // "AP8Q"
string packed = GZip.BASE62.EncodeString(longText);
```

Extension methods provide the same for strings, byte arrays, characters and
numbers:

```csharp
string s = "Hello".ToBase64(pad: true);           // "SGVsbG8="
short k = ((short) 42).ToBase62().FromBase62<short>();
string small = longText.Compress();
string back = small.Decompress();
```

Invalid input throws a `FormatException`. Leading zero bytes are kept, so
binary data such as hashes or encrypted data survives a round trip.

Own encoders implement `IEncoder` with `Encoding`, `Encode(byte[])` and
`Decode(byte[])`, or derive from `EncoderBase`; all other methods have default
implementations.


## Hashing

`MD5`, `SHA1`, `SHA256`, `SHA384` and `SHA512` implement `IHash`. Each has
instances for hexadecimal (default), Base64, Base62 and inverted Base62
output:

```csharp
string h = SHA256.GetHash("abc");                     // "ba7816bf…"
string b = SHA256.BASE62.HashString("abc");           // "iDUK5mbj…"
bool ok = SHA256.BASE62.Verify("abc", b);             // constant-time comparison

string file = SHA256.GetFileHash("setup.exe");        // file contents
string text = MD5.GetFileHash("readme.md", true);     // text with LF line breaks
string data = SHA512.HEX.HashStream(stream);
```

These hashes are fast, which makes them unsuitable for passwords. `Pbkdf2`
hashes passwords with a random salt and many iterations (OWASP defaults) and
stores everything needed for verification in one string:

```csharp
string stored = Pbkdf2.DEFAULT.Hash(password);        // "$pbkdf2-sha256$i=600000$…$…"

if(Pbkdf2.DEFAULT.Verify(password, stored))
{
    if(Pbkdf2.DEFAULT.NeedsRehash(stored)) { stored = Pbkdf2.DEFAULT.Hash(password); }
}
```

`Argon2` (RFC 9106) is the modern alternative: it is memory-hard, which makes
attacks with GPUs or special hardware expensive. It implements `IPasswordHasher`
as well and writes the PHC format used by the reference implementation, so
hashes can be exchanged with other libraries (e.g. libsodium, argon2-cffi).
The defaults are the OWASP recommendation (Argon2id, 19 MiB, 2 iterations):

```csharp
string stored = Argon2.DEFAULT.Hash(password);        // "$argon2id$v=19$m=19456,t=2,p=1$…$…"
bool ok = Argon2.DEFAULT.Verify(password, stored);

Argon2 strong = new(Argon2Type.ARGON2ID, memory: 65536, iterations: 3, parallelism: 4);
Argon2 peppered = new(secret: pepper);                // secret key, not stored with the hash

byte[] key = Argon2.DeriveKey(Argon2Type.ARGON2ID, pw, salt, 65536, 3, 4, 32);
```

Existing SHA-256 password hashes can be migrated on the next login:

```csharp
if(SHA256.BASE62.Verify(password, stored)) { stored = Argon2.DEFAULT.Hash(password); }
```


## Encryption

`AESGCM` encrypts data with AES-GCM. Every message gets a random nonce and
an authentication tag, so modified data or a wrong key is detected. The key is
given directly or derived from a password (PBKDF2-SHA256, 600,000 iterations):

```csharp
byte[] key = AESGCM.GenerateKey();                 // 32 random bytes
byte[] enc = AESGCM.Encrypt(data, key);
byte[] dec = AESGCM.Decrypt(enc, key);             // CryptographicException if modified

string s = AESGCM.Encrypt("text", password);       // Base64
string t = AESGCM.Decrypt(s, password);

AESGCM cipher = new(password, Base62.DEFAULT);     // derives the key once
string a = cipher.EncodeString("text");
cfg.Save("settings.dat", new Ddn.Options(cipher));    // encrypted ddn file
```

The format is documented on the class and can be read with any AES-GCM
implementation (WebCrypto, OpenSSL, Java, Python).

`FunnelCake` obfuscates data with a secret by XOR with a key stream. It is
simple and easy to port, but it is not secure encryption: the key stream is
the same for every message, and modifications are not detected. Use it to
keep casual readers out of configuration files; its output is unchanged from
earlier versions.

```csharp
string o = FunnelCake.BASE62.EncodeString("text");
string p = FunnelCake.Encrypt("text", "secret", Base64.DEFAULT);
```

Both implement `IEncrypter`, which is an `IEncoder` with raw `Encrypt()` and
`Decrypt()` methods.


## String Reader

`StringRdr` reads a string character by character, which is handy for small
parsers. `Position` is the index of the current character (-1 before the
first `Read()`):

```csharp
StringRdr re = new("sum(12, x)");

string name = re.ReadWhile(char.IsLetter);           // "sum"
if(re.TryRead("("))
{
    string arg = re.ReadTo(",", ")");                // "12", Delimiter ","
    re.SkipWhiteSpace();
    char c = re.Peek();                              // 'x', not read yet
}
```


## String Operations

`StringOp` provides extension methods for strings. Numbers and Boolean values
are parsed with the invariant culture unless a format provider is given; text
is compared ordinally.

```csharp
string pw = StringOp.Random(16, StringOp.PASSWORD_ALPHABET);  // cryptographically secure
string id = StringOp.Unique();                                // 24 random alphanumeric characters

string hex = 255L.ToString(16);                      // "ff"
long n = "FF".ToInt64(16);                           // 255; FormatException for invalid digits

bool b = "Yes".ToBoolean();                          // true, t, yes, y, on, numbers != 0
bool m = "file.txt".Matches(false, "*.TXT", "*.doc");
string ext = "archive.tar.gz".AfterFirst('.');       // "tar.gz"
string r = "Hello hello".ReplaceAll("HELLO", "bye", false);
```


## Utility Classes

- `CalendarOp` reads and writes timestamps (`2024-05-01 12:00:00.500+02:00`,
  the ddn format) and converts Unix time:

  ```csharp
  string ts = DateTime.Now.ToTimestamp();
  DateTime t = "2024-05-01T12:00:00.123Z".ParseTimestamp();   // local time
  if("2024-05-01".TryParseTimestamp(out DateTimeOffset o)) { … }
  long unix = DateTime.UtcNow.ToUnixTimestamp();
  ```
- `CollectionOp` picks random elements and shuffles (`Random()`,
  `RandomOrDefault()`, `Random(count)`, `Shuffled()`, `Shuffle()`).
- `VersionOp` reads application and assembly versions; `StreamOp` reads
  streams to the end.
- `PathOp` provides standard paths for configuration and temporary files, and
  `FileOp` copies, deletes and purges files and directories. All paths are
  built with `Path.Combine` and work on every platform:

  ```csharp
  PathOp.Initialize("robbiblubber.org/MyApp");
  string cfg = PathOp.UserConfigurationPath;          // e.g. %LOCALAPPDATA%\robbiblubber.org\MyApp
  string tmp = FileOp.CreateTempPath();                // below Path.GetTempPath()/robbiblubber.org
  FileOp.CopyDirectory(source, target);                // IOException if target is inside source
  FileOp.CleanupTemporaryPaths();                      // removes temporary entries older than one hour
  ```
- `BufferedFileWriter` collects text in memory and appends it to a file in
  regular intervals, for example for log files. Nothing is lost:
  `Stop()`, `Flush()` and `Dispose()` write all pending text.

  ```csharp
  using BufferedFileWriter log = new(PathOp.GetTempFile("log"), 250);
  log.WriteError += (s, e) => Console.Error.WriteLine(e.GetException().Message);
  log.Start();
  log.WriteLine("started");
  ```
- `SingleInstance` lets only one instance of an application run for the
  current user. Further starts pass their arguments and working directory to
  the first instance over a named pipe that accepts only the current user:

  ```csharp
  using SingleInstance instance = new("robbiblubber.org/MyTool");
  if(!instance.IsFirstInstance && instance.Forward(args)) { return; }

  instance.ArgumentsReceived += (s, e) => Open(e.Arguments.Select(e.GetFullPath));   // thread pool thread
  instance.StartListening();
  ```
- `PluginLoader` loads plug-ins from a directory (by default including
  subdirectories). Each plug-in directory gets its own load context, so
  plug-ins find their own dependencies; the application's assemblies are
  shared, so plug-in types implement the application's interfaces:

  ```csharp
  PluginLoader plugins = new(Path.Combine(PathOp.ApplicationDirectory, "plugins"));
  List<IExporter> exporters = plugins.CreateInstances<IExporter>();
  foreach(PluginError e in plugins.Errors) { … }      // what could not be loaded, and why
  ```
- `Disposal` releases objects as completely as possible, e.g. interop
  objects: it calls `Close` (also with optional parameters, late bound for
  COM objects) and then disposes. Failures do not stop the other objects.
  `Recycle()` forces a full garbage collection; `ReleaseComObjects()`
  (Windows only) releases COM objects completely so that e.g. Office
  applications can exit:

  ```csharp
  Disposal.Dispose(workbook, stream, socket);          // best effort
  bool ok = Disposal.TryDispose(workbook, stream);     // FALSE if anything failed
  Disposal.ReleaseComObjects(sheet, workbook, excel);
  ```
- `ClassOp` finds implementations in loaded assemblies
  (`GetImplementations<T>()`) and reads or writes properties and fields by
  name, including private members of base classes (`obj.__Get<T>("name")`,
  `obj.__Set("name", value)`).
