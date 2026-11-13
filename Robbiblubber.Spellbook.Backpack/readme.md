# robbiblubber.org Spellbook Backpack Library

The robbiblubber.org Spellbook Backpack Library appends a payload to a carrier
file and reads it back, for example to attach data to an executable that reads
it at runtime. It replaces the Prelude Library from the robbiblubber.org
Utility Suite.

The library targets .NET 8 and .NET 10 and contains no platform-dependent code.

The repository can be found at https://bitbucket.org/robbiblubber/robbiblubber.spellbook.net/.
API documentation is available from https://robbiblubber.lima-city.org/.
For general information see http://robbiblubber.org/.

The robbiblubber.org Spellbook Backpack Library is provided under the MIT
license and may be used without restrictions under the conditions of this
license agreement.


## Usage

```csharp
using Robbiblubber.Spellbook.Backpack;

Packer.Append("setup.exe", "data.zip", "setup-data");          // optional tag

// read the payload of the running executable
string exe = Environment.ProcessPath!;
if(Packer.HasPayload(exe, "setup-data"))
{
    Packer.Extract(exe, Path.Combine(Path.GetTempPath(), "data.zip"), "setup-data");
}

using Stream data = Packer.OpenPayload(exe, "setup-data");     // without a temporary file
```

- `Append()` and `AppendAsync()` append a file, a stream or a byte array. If
  appending fails, the carrier is restored to its original length.
- `HasPayload()` and `GetInfo()` read the footer (length, tag, carrier length,
  hash); `Verify()` checks the payload against its hash.
- `Extract()`, `TryExtract()` and `ExtractAsync()` write the payload to a file
  or a stream. A target file is only replaced when the payload is intact.
- `ReadPayload()` returns the payload as a byte array, `OpenPayload()` as a
  seekable read-only stream.
- `Remove()` removes the payload and restores the carrier.

Files are read with `FileShare.ReadWrite | FileShare.Delete`, so a running
executable can read its own payload.


## File format

A packed file consists of the unchanged carrier, the payload and a footer of
84 bytes at the end of the file:

| Offset | Length | Content                                      |
|-------:|-------:|----------------------------------------------|
|      0 |     32 | SHA-256 hash of the payload                  |
|     32 |      8 | payload length (Int64, little endian)        |
|     40 |     32 | tag (UTF-8, zero padded; all zero = no tag)  |
|     72 |      2 | format version (UInt16, little endian; 1)    |
|     74 |      2 | reserved (0)                                 |
|     76 |      8 | magic `RBBKPACK` (ASCII)                     |

Reading only needs the end of the file, so the size of the carrier does not
matter, and carrier and payload may contain any bytes. A file holds at most one
payload; damaged payloads are detected by the hash.
